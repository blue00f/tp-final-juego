using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Strategy;

public sealed class IADefensiva : IEstrategiaIA
{
    public string Nombre => "IA Defensiva";
    public string Descripcion => "Con menos de 60% de vida se cubre; si no, contraataca a su último agresor.";

    public AccionEnemiga DecidirAccion(Enemigo enemigo, Contexto contexto)
    {
        AccionEnemiga decision;
        if (enemigo.PorcentajeVida < 0.6 && enemigo.Disponible(TipoAccion.Defender))
        {
            decision = new AccionEnemiga(TipoAccion.Defender, null,
                $"Está herido ({enemigo.PorcentajeVida:P0}): endurece su caparazón.");
        }
        else
        {
            var agresor = enemigo.UltimoAgresor as MiembroEscuadron;
            var objetivo = agresor is { EstaVivo: true }
                ? agresor
                : contexto.AliadosVivos.OrderBy(m => m.Posicion).First();
            string motivo = objetivo == agresor ? "contraataca a su último agresor" : "golpea a quien tiene adelante";
            decision = new AccionEnemiga(TipoAccion.Atacar, objetivo, $"Se repliega y {motivo}: {objetivo.Nombre}.");
        }
        return decision;
    }
}
