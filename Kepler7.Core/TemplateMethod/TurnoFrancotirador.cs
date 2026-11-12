using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.TemplateMethod;

public sealed class TurnoFrancotirador : TurnoUnidad
{
    public override string NombreTurno => "Turno de Francotirador";

    protected override void Actuar(Personaje unidad, AccionTurno accion, Contexto contexto)
    {
        var tirador = (MiembroEscuadron)unidad;
        switch (accion.Tipo)
        {
            case TipoAccion.Atacar:
                double probabilidadCritico = tirador.Posicion == Posicion.Retaguardia ? 0.35 : 0.2;
                bool critico = contexto.Aleatorio.Probabilidad(probabilidadCritico);
                string nombre = critico ? "¡Disparo crítico!" : "Disparo";
                AtaqueDeMiembro(tirador, accion, contexto, critico ? 2.0 : 1.0, nombre);
                break;
            case TipoAccion.Mover:
                MoverUnidad(tirador, contexto);
                break;
            default:
                contexto.Registrar(CategoriaRegistro.Sistema, $"{tirador.Nombre} ajusta la mira.");
                break;
        }
    }
}
