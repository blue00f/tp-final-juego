using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Core.Strategy;

public sealed class IAAgresiva : IEstrategiaIA
{
    public string Nombre => "IA Agresiva";
    public string Descripcion => "Ataca siempre al Capitán, envenenándolo cuando puede.";

    public AccionEnemiga DecidirAccion(Enemigo enemigo, Contexto contexto)
    {
        var capitan = contexto.Capitan;
        AccionEnemiga decision;
        if (enemigo.Disponible(TipoAccion.Envenenar) && capitan.Estado.Tipo != TipoEstado.Envenenado)
        {
            decision = new AccionEnemiga(TipoAccion.Envenenar, capitan, $"Escupe ácido al Capitán {capitan.Nombre} para debilitarlo.");
        }
        else
        {
            decision = new AccionEnemiga(TipoAccion.Atacar, capitan, $"Se lanza directo contra el Capitán {capitan.Nombre}.");
        }
        return decision;
    }
}
