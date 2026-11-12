namespace Kepler7.Core.Comun;

public sealed class Registro
{
    private readonly List<EntradaRegistro> _entradas = new();

    public IReadOnlyList<EntradaRegistro> Entradas => _entradas;

    public void Agregar(int ronda, CategoriaRegistro categoria, string texto)
    {
        _entradas.Add(new EntradaRegistro(ronda, categoria, texto));
    }

    public IEnumerable<EntradaRegistro> Filtrar(CategoriaRegistro categoria)
    {
        return _entradas.Where(e => e.Categoria == categoria);
    }
}
