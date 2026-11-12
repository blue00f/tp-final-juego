using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Strategy;

public interface IEstrategiaIA
{
    string Nombre { get; }
    string Descripcion { get; }

    AccionEnemiga DecidirAccion(Enemigo enemigo, Contexto contexto);
}
