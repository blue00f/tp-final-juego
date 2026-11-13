namespace Kepler7.Core.Strategy;

public static class CatalogoEstrategias
{
    public static IReadOnlyList<Func<IEstrategiaIA>> Todas { get; } = new Func<IEstrategiaIA>[]
    {
        () => new IAAgresiva(),
        () => new IADefensiva(),
        () => new IAAleatoria(),
        () => new IACalculadora()
    };
}
