using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Core.TemplateMethod;

public sealed class TurnoSoldado : TurnoUnidad
{
    public override string NombreTurno => "Turno de Soldado";

    protected override void Iniciar(Personaje unidad, Contexto contexto)
    {
        base.Iniciar(unidad, contexto);
        bool entraEnFuria = unidad.EstaVivo && unidad.Estado.Tipo == TipoEstado.Normal && unidad.PorcentajeVida < 0.35;
        if (entraEnFuria)
        {
            contexto.AplicarEstado(unidad, new Berserk(2), "vida baja: furia de combate");
        }
    }

    protected override void Actuar(Personaje unidad, AccionTurno accion, Contexto contexto)
    {
        var soldado = (MiembroEscuadron)unidad;
        switch (accion.Tipo)
        {
            case TipoAccion.Atacar:
                AtaqueDeMiembro(soldado, accion, contexto, 1.0, "Ráfaga");
                var objetivo = accion.Objetivo as Enemigo ?? contexto.EnemigosVivos.FirstOrDefault();
                bool aturde = objetivo is { EstaVivo: true } && objetivo.Estado.Tipo == TipoEstado.Normal
                              && contexto.Aleatorio.Probabilidad(0.2);
                if (aturde)
                {
                    contexto.AplicarEstado(objetivo!, new Aturdido(1), "culatazo del soldado");
                }
                break;
            case TipoAccion.Mover:
                MoverUnidad(soldado, contexto);
                break;
            default:
                contexto.Registrar(CategoriaRegistro.Sistema, $"{soldado.Nombre} mantiene la posición.");
                break;
        }
    }
}
