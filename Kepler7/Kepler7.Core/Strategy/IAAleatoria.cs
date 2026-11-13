using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Strategy;

public sealed class IAAleatoria : IEstrategiaIA
{
    public string Nombre => "IA Aleatoria";
    public string Descripcion => "Elige acción y objetivo al azar.";

    public AccionEnemiga DecidirAccion(Enemigo enemigo, Contexto contexto)
    {
        var opciones = new[] { TipoAccion.Atacar, TipoAccion.Envenenar, TipoAccion.Aturdir, TipoAccion.Defender }
            .Where(t => t == TipoAccion.Atacar || enemigo.Disponible(t))
            .ToList();
        var vivos = contexto.AliadosVivos.ToList();

        var tipo = opciones[contexto.Aleatorio.Entre(0, opciones.Count)];
        MiembroEscuadron? objetivo = tipo == TipoAccion.Defender ? null : vivos[contexto.Aleatorio.Entre(0, vivos.Count)];
        string razon = objetivo == null
            ? "Impulso errático: se cubre sin motivo aparente."
            : $"Impulso errático: {tipo} contra {objetivo.Nombre}.";
        return new AccionEnemiga(tipo, objetivo, razon);
    }
}
