using Kepler7.Core.Modelo;

namespace Kepler7.Core.ChainOfResponsibility;

public sealed class CadenaDefensa
{
    private readonly List<IManejadorDanio> _eslabones = new();
    private readonly Vida _vida;

    public CadenaDefensa(Personaje duenio)
    {
        _vida = new Vida(duenio);
        _eslabones.Add(_vida);
        Reconstruir();
    }

    public IReadOnlyList<IManejadorDanio> Eslabones => _eslabones;

    public IManejadorDanio Primero => _eslabones[0];

    public void Agregar(IManejadorDanio eslabon)
    {
        _eslabones.Insert(_eslabones.Count - 1, eslabon);
        Reconstruir();
    }

    public bool Quitar(IManejadorDanio eslabon)
    {
        bool quitado = false;
        if (eslabon != _vida)
        {
            quitado = _eslabones.Remove(eslabon);
            Reconstruir();
        }
        return quitado;
    }

    public bool Mover(int indice, int desplazamiento)
    {
        int destino = indice + desplazamiento;
        int ultimoMovible = _eslabones.Count - 2;
        bool valido = indice >= 0 && indice <= ultimoMovible && destino >= 0 && destino <= ultimoMovible;
        if (valido)
        {
            var eslabon = _eslabones[indice];
            _eslabones.RemoveAt(indice);
            _eslabones.Insert(destino, eslabon);
            Reconstruir();
        }
        return valido;
    }

    internal void Reiniciar()
    {
        _eslabones.Clear();
        _eslabones.Add(_vida);
        Reconstruir();
    }

    internal void CopiarDe(CadenaDefensa otra)
    {
        Reiniciar();
        foreach (var eslabon in otra.Eslabones.Where(e => e is not Vida))
        {
            Agregar(eslabon.Clonar());
        }
    }

    public T? Buscar<T>() where T : class, IManejadorDanio
    {
        return _eslabones.OfType<T>().FirstOrDefault();
    }

    public void Procesar(Danio danio)
    {
        Primero.Manejar(danio);

        var agotados = _eslabones.OfType<Escudo>().Where(e => e.Agotado).ToList();
        foreach (var escudo in agotados)
        {
            _eslabones.Remove(escudo);
            danio.Nota(escudo.Nombre, "agotado: se retira de la cadena");
        }
        if (agotados.Count > 0)
        {
            Reconstruir();
        }
    }

    public int Estimar(int danioEntrante)
    {
        int valor = danioEntrante;
        foreach (var eslabon in _eslabones)
        {
            valor = eslabon.Estimar(valor);
        }
        return valor;
    }

    public string Describir()
    {
        return string.Join(" → ", _eslabones.Select(e => e.Descripcion));
    }

    private void Reconstruir()
    {
        for (int i = 0; i < _eslabones.Count; i++)
        {
            var siguiente = i + 1 < _eslabones.Count ? _eslabones[i + 1] : null;
            _eslabones[i].EstablecerSiguiente(siguiente);
        }
    }

    internal FotoCadena Capturar()
    {
        var puntos = _eslabones.OfType<Escudo>().ToDictionary(e => e, e => e.Puntos);
        return new FotoCadena(new List<IManejadorDanio>(_eslabones), puntos);
    }

    internal void Restaurar(FotoCadena foto)
    {
        _eslabones.Clear();
        _eslabones.AddRange(foto.Eslabones);
        foreach (var par in foto.PuntosEscudo)
        {
            par.Key.Puntos = par.Value;
        }
        Reconstruir();
    }
}
