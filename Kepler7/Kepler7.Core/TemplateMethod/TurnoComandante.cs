using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.TemplateMethod;

public sealed class TurnoComandante : TurnoUnidad
{
    public const int PuntosGenerador = 25;

    public override string NombreTurno => "Turno de Comandante";

    protected override void Actuar(Personaje unidad, AccionTurno accion, Contexto contexto)
    {
        var capitan = (MiembroEscuadron)unidad;
        switch (accion.Tipo)
        {
            case TipoAccion.Atacar:
                AtaqueDeMiembro(capitan, accion, contexto, 1.0, "Orden de fuego");
                break;
            case TipoAccion.Construir when accion.Objetivo != null:
                DesplegarEscudo(accion.Objetivo, contexto);
                break;
            case TipoAccion.Mover:
                MoverUnidad(capitan, contexto);
                break;
            default:
                contexto.Registrar(CategoriaRegistro.Sistema, $"{capitan.Nombre} evalúa el campo.");
                break;
        }
    }

    private static void DesplegarEscudo(Personaje aliado, Contexto contexto)
    {
        var escudo = aliado.Defensa.Buscar<Escudo>();
        if (escudo == null)
        {
            aliado.Defensa.Agregar(new Escudo(PuntosGenerador));
            contexto.Registrar(CategoriaRegistro.Danio, $"◈ Se agrega un eslabón Escudo a la cadena de {aliado.Nombre}: {aliado.Defensa.Describir()}");
        }
        else
        {
            escudo.Recargar(PuntosGenerador);
            contexto.Registrar(CategoriaRegistro.Danio, $"◈ Escudo de {aliado.Nombre} recargado a {escudo.Puntos} pts.");
        }
        contexto.AgregarEvento(TipoEvento.Escudo, aliado, PuntosGenerador, $"+{PuntosGenerador} ◈");
    }
}
