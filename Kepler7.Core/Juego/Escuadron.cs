using Kepler7.Core.Modelo;

namespace Kepler7.Core.Juego;

public sealed class Escuadron
{
    private readonly List<MiembroEscuadron> _miembros;
    private readonly List<MiembroEscuadron> _titulares;

    public Escuadron(IEnumerable<MiembroEscuadron> miembros)
    {
        _miembros = miembros.ToList();
        _titulares = new List<MiembroEscuadron>(_miembros);
        if (_miembros.Count(m => m.EsCapitan) != 1)
        {
            throw new ArgumentException("El escuadrón debe tener exactamente un Capitán (rol Comandante).");
        }
    }

    public IReadOnlyList<MiembroEscuadron> Miembros => _miembros;

    public IReadOnlyList<MiembroEscuadron> Titulares => _titulares;

    internal void Reemplazar(MiembroEscuadron caido, MiembroEscuadron reemplazo)
    {
        int indice = _miembros.IndexOf(caido);
        if (indice < 0 || caido.EsCapitan || reemplazo.EsCapitan)
        {
            throw new InvalidOperationException("Reemplazo inválido.");
        }
        _miembros[indice] = reemplazo;
    }

    internal void RestaurarTitulares()
    {
        _miembros.Clear();
        _miembros.AddRange(_titulares);
    }

    internal void RestaurarFormacion(IEnumerable<MiembroEscuadron> formacion)
    {
        var copia = formacion.ToList();
        _miembros.Clear();
        _miembros.AddRange(copia);
    }

    public MiembroEscuadron Capitan => _miembros.First(m => m.EsCapitan);

    public MiembroEscuadron? Buscar(Rol rol) => _miembros.FirstOrDefault(m => m.Rol == rol);

    public static Posicion PosicionInicial(Rol rol) => rol switch
    {
        Rol.Comandante => Posicion.Vanguardia,
        Rol.Soldado => Posicion.Vanguardia,
        _ => Posicion.Retaguardia
    };
}
