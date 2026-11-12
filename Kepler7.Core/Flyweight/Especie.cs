namespace Kepler7.Core.Flyweight;

public sealed class Especie : IEspecie
{
    internal Especie(string nombre, string color, string planetaOrigen, string dieta, BonusEspecie bonus)
    {
        Nombre = nombre;
        Color = color;
        PlanetaOrigen = planetaOrigen;
        Dieta = dieta;
        Bonus = bonus;
        Retrato = GenerarRetrato(nombre);
    }

    public string Nombre { get; }
    public string Color { get; }
    public string PlanetaOrigen { get; }
    public string Dieta { get; }
    public byte[] Retrato { get; }
    public BonusEspecie Bonus { get; }

    internal static byte[] GenerarRetrato(string semilla)
    {
        var retrato = new byte[32 * 32 * 4];
        new Random(semilla.GetHashCode()).NextBytes(retrato);
        return retrato;
    }
}
