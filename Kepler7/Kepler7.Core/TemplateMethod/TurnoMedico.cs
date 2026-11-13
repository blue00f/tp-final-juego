using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Core.TemplateMethod;

public sealed class TurnoMedico : TurnoUnidad
{
    public const int Curacion = 32;

    public override string NombreTurno => "Turno de Médico";

    protected override void Actuar(Personaje unidad, AccionTurno accion, Contexto contexto)
    {
        var medico = (MiembroEscuadron)unidad;
        switch (accion.Tipo)
        {
            case TipoAccion.Curar when accion.Objetivo != null:
                var paciente = accion.Objetivo;
                if (paciente.Estado.Tipo == TipoEstado.Envenenado)
                {
                    contexto.AplicarEstado(paciente, new Normal(), "antídoto del médico");
                }
                int cantidad = (int)Math.Round(Curacion * contexto.Modificadores.MultiplicadorCuracion);
                int curado = contexto.Sanar(medico, paciente, cantidad);
                contexto.Registrar(CategoriaRegistro.Sistema, $"✚ {medico.Nombre} cura {curado} a {paciente.Nombre}.");
                break;
            case TipoAccion.Atacar:
                AtaqueDeMiembro(medico, accion, contexto, 1.0, "Dardo sedante");
                break;
            case TipoAccion.Mover:
                MoverUnidad(medico, contexto);
                break;
            default:
                contexto.Registrar(CategoriaRegistro.Sistema, $"{medico.Nombre} espera.");
                break;
        }
    }
}
