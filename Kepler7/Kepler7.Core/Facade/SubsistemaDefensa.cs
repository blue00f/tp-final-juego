using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.Visitor;
using ArmaduraEslabon = Kepler7.Core.ChainOfResponsibility.Armadura;

namespace Kepler7.Core.Facade;

internal sealed class SubsistemaDefensa
{
    public const int EscudoBaseCapitan = 20;

    private readonly IAleatorio _aleatorio;

    public SubsistemaDefensa(IAleatorio aleatorio)
    {
        _aleatorio = aleatorio;
    }

    public List<string> CalibrarDefensas(Escuadron escuadron, Inventario inventario, ModificadoresMision mods)
    {
        var lineas = new List<string> { "[DEFENSA] formación de combate y cadenas de defensa calibradas:" };
        foreach (var miembro in escuadron.Miembros)
        {
            miembro.Posicion = Escuadron.PosicionInicial(miembro.Rol);
            ArmarCadena(miembro, inventario, mods);
            miembro.AtaqueBase = (int)Math.Round(miembro.AtaqueNatural * mods.MultiplicadorAtaque);
            lineas.Add($"     {miembro.Nombre} (ataque {miembro.AtaqueBase}): {miembro.Defensa.Describir()}");
        }

        // El clon conserva la cadena que le dio el laboratorio; los bonos de la misión
        if (mods.ClonReserva != null)
        {
            lineas.Add($"     (reserva) {mods.ClonReserva.Nombre}: {mods.ClonReserva.Defensa.Describir()}");
        }
        return lineas;
    }

    public void CalibrarBase(Escuadron escuadron, Inventario inventario)
    {
        foreach (var miembro in escuadron.Miembros)
        {
            ArmarCadena(miembro, inventario, new ModificadoresMision());
        }
    }

    private void ArmarCadena(MiembroEscuadron miembro, Inventario inventario, ModificadoresMision mods)
    {
        var cadena = miembro.Defensa;
        cadena.Reiniciar();
        double armadura = inventario.ArmaduraDe(miembro.Rol)?.Reduccion ?? 0;
        double esquivaBase = miembro.Rol switch
        {
            Rol.Comandante => 0.10,
            Rol.Medico => 0.15,
            Rol.Francotirador => 0.25,
            _ => 0.0
        };
        double esquiva = esquivaBase + mods.EsquivaExtra;
        int escudo = mods.EscudoInicial + (miembro.EsCapitan ? EscudoBaseCapitan : 0);

        if (esquiva > 0)
        {
            cadena.Agregar(new Esquiva(Math.Min(0.6, esquiva), _aleatorio));
        }
        if (miembro.Rol == Rol.Soldado)
        {
            cadena.Agregar(new Bloqueo(8));
        }
        if (armadura > 0)
        {
            cadena.Agregar(new ArmaduraEslabon(armadura));
        }
        if (escudo > 0)
        {
            cadena.Agregar(new Escudo(escudo));
        }
    }

    public string EscudoDeEmergencia(MiembroEscuadron capitan, int puntos)
    {
        var escudo = capitan.Defensa.Buscar<Escudo>();
        if (escudo == null)
        {
            capitan.Defensa.Agregar(new Escudo(puntos));
        }
        else
        {
            escudo.Recargar(puntos);
        }
        return $"[DEFENSA] escudo de emergencia sobre el Capitán (+{puntos} pts).";
    }

    public string Standby() => "[DEFENSA] sistemas en standby.";
}
