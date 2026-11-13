namespace Kepler7.Core.Comun;

public sealed class AleatorioSistema : IAleatorio
{
    private readonly Random _random;

    public AleatorioSistema(int? semilla = null)
    {
        _random = semilla.HasValue ? new Random(semilla.Value) : new Random();
    }

    public double SiguienteDoble() => _random.NextDouble();

    public int Entre(int minimo, int maximoExclusivo) => _random.Next(minimo, maximoExclusivo);

    public bool Probabilidad(double p) => _random.NextDouble() < p;
}
