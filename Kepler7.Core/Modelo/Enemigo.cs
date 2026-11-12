using Kepler7.Core.Strategy;

namespace Kepler7.Core.Modelo;

public sealed class Enemigo : Personaje
{
    private readonly Dictionary<TipoAccion, int> _enfriamientos = new();

    public Enemigo(string nombre, string descripcion, int vidaMaxima, int ataqueBase, IEstrategiaIA estrategia)
        : base(nombre, vidaMaxima, ataqueBase, Posicion.Vanguardia)
    {
        Descripcion = descripcion;
        Estrategia = estrategia;
    }

    public string Descripcion { get; }

    public AparienciaAlien Apariencia { get; init; } = new("#B25CFF", 3, 6, 1.0);

    public bool EsJefe { get; init; }

    public IEstrategiaIA Estrategia { get; private set; }

    public Personaje? UltimoAgresor { get; internal set; }

    public AccionEnemiga? UltimaDecision { get; internal set; }

    public void CambiarEstrategia(IEstrategiaIA nuevaEstrategia)
    {
        Estrategia = nuevaEstrategia;
    }

    public bool Disponible(TipoAccion habilidad)
    {
        return !_enfriamientos.TryGetValue(habilidad, out int rondas) || rondas <= 0;
    }

    public int RondasDeEnfriamiento(TipoAccion habilidad)
    {
        return _enfriamientos.TryGetValue(habilidad, out int rondas) ? Math.Max(0, rondas) : 0;
    }

    internal void IniciarEnfriamiento(TipoAccion habilidad, int rondas)
    {
        _enfriamientos[habilidad] = rondas;
    }

    internal void AvanzarEnfriamientos()
    {
        foreach (var clave in _enfriamientos.Keys.ToList())
        {
            _enfriamientos[clave] = Math.Max(0, _enfriamientos[clave] - 1);
        }
    }

    internal override FotoPersonaje Fotografiar()
    {
        var foto = base.Fotografiar();
        foto.Enfriamientos = new Dictionary<TipoAccion, int>(_enfriamientos);
        foto.UltimoAgresor = UltimoAgresor;
        return foto;
    }

    internal override void Restaurar(FotoPersonaje foto)
    {
        base.Restaurar(foto);
        _enfriamientos.Clear();
        if (foto.Enfriamientos != null)
        {
            foreach (var par in foto.Enfriamientos)
            {
                _enfriamientos[par.Key] = par.Value;
            }
        }
        UltimoAgresor = foto.UltimoAgresor;
    }
}
