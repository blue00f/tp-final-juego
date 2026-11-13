namespace Kepler7.Core.Visitor;

public static class CatalogoVisitantes
{
    public static IReadOnlyList<Func<IVisitor>> Todos { get; } = new Func<IVisitor>[]
    {
        () => new CalculadorPrecio(),
        () => new ContadorArmas(),
        () => new AplicadorVeneno()
    };
}
