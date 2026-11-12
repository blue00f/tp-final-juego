using Kepler7.Core.Flyweight;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.Progreso;
using Kepler7.Core.State;

namespace Kepler7.Core.Facade;

internal sealed class SubsistemaSoporteVital
{
    public string RestaurarTripulacion(Escuadron escuadron)
    {
        escuadron.RestaurarTitulares();
        foreach (var miembro in escuadron.Miembros)
        {
            Estabilizar(miembro);
        }
        return "[SOPORTE VITAL] tripulación estabilizada (vida completa, estados limpios).";
    }

    public string? PrepararDestacamento(IEspecie? especie, ModificadoresMision mods)
    {
        string? linea = null;
        if (especie != null)
        {
            var b = especie.Bonus;
            mods.Sumar(b.MultiplicadorAtaque - 1, b.EscudoInicial, b.EsquivaExtra, b.MultiplicadorCuracion - 1, b.MultiplicadorXp - 1);
            mods.PatronesAplicados.Add(Patrones.Flyweight);
            linea = $"[SOPORTE VITAL] destacamento {especie.Nombre} a bordo ({especie.PlanetaOrigen}): {b.Descripcion}.";
        }
        return linea;
    }

    public string? PrepararClon(MiembroEscuadron? clon, ModificadoresMision mods)
    {
        string? linea = null;
        if (clon != null)
        {
            Estabilizar(clon);
            mods.ClonReserva = clon;
            mods.PatronesAplicados.Add(Patrones.Prototype);
            linea = $"[SOPORTE VITAL] cápsula de criogenia: {clon.Nombre} relevará al primer tripulante que caiga.";
        }
        return linea;
    }

    public string InyectarEstimulante(MiembroEscuadron capitan, int cantidad)
    {
        int curado = capitan.Curar(cantidad);
        return $"[SOPORTE VITAL] estimulante de combate al Capitán (+{curado} vida).";
    }

    public string ModoMinimo() => "[SOPORTE VITAL] modo de consumo mínimo.";

    private static void Estabilizar(MiembroEscuadron miembro)
    {
        miembro.Vida = miembro.VidaMaxima;
        miembro.CambiarEstado(new Normal());
        miembro.YaActuo = false;
        miembro.AtaqueBase = miembro.AtaqueNatural;
    }
}
