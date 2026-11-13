using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Core.Strategy;

public sealed class IACalculadora : IEstrategiaIA
{
    public string Nombre => "IA Calculadora";
    public string Descripcion => "Estima el daño tras las defensas y busca el golpe más rentable.";

    public AccionEnemiga DecidirAccion(Enemigo enemigo, Contexto contexto)
    {
        int ataque = enemigo.Atacar();
        var evaluaciones = contexto.AliadosVivos
            .Select(m => (Miembro: m, Danio: contexto.EstimarDanio(m, ataque)))
            .ToList();

        var letales = evaluaciones.Where(e => e.Danio >= e.Miembro.Vida).ToList();
        var medico = contexto.AliadosVivos.FirstOrDefault(m => m.Rol == Rol.Medico);
        AccionEnemiga decision;

        if (letales.Count > 0)
        {
            var elegido = letales.OrderByDescending(e => e.Miembro.EsCapitan).ThenBy(e => e.Miembro.Vida).First();
            decision = new AccionEnemiga(TipoAccion.Atacar, elegido.Miembro,
                $"Calculó {elegido.Danio} de daño contra {elegido.Miembro.Nombre} (vida {elegido.Miembro.Vida}): golpe letal.");
        }
        else if (medico != null && enemigo.Disponible(TipoAccion.Aturdir) && medico.Estado.Tipo != TipoEstado.Aturdido)
        {
            decision = new AccionEnemiga(TipoAccion.Aturdir, medico,
                $"Aturde a {medico.Nombre} para cortar las curaciones del escuadrón.");
        }
        else
        {
            var mejor = evaluaciones.OrderByDescending(e => (double)e.Danio / e.Miembro.Vida).First();
            bool conviene = enemigo.Disponible(TipoAccion.Envenenar) && mejor.Miembro.Estado.Tipo == TipoEstado.Normal
                            && mejor.Miembro.Vida > 40;
            var tipo = conviene ? TipoAccion.Envenenar : TipoAccion.Atacar;
            decision = new AccionEnemiga(tipo, mejor.Miembro,
                $"Mejor relación daño/vida: {mejor.Miembro.Nombre} ({mejor.Danio} estimado sobre {mejor.Miembro.Vida} de vida) → {tipo}.");
        }
        return decision;
    }
}
