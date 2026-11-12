namespace Kepler7.Core.Builder;

public static class CatalogoRecetas
{
    public static IReadOnlyList<Func<INaveBuilder>> Todas { get; } = new Func<INaveBuilder>[]
    {
        () => new RecetaExploradora(),
        () => new RecetaCombate(),
        () => new RecetaCarga()
    };

    public static INaveBuilder? Buscar(string nombreReceta)
    {
        return Todas.Select(f => f()).FirstOrDefault(r => r.NombreReceta == nombreReceta);
    }
}
