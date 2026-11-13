namespace Kepler7.Core.Flyweight;

internal sealed class EspecieSinCompartir : IEspecie
{
    public EspecieSinCompartir(IEspecie original)
    {
        Nombre = new string(original.Nombre.ToCharArray());
        Color = new string(original.Color.ToCharArray());
        PlanetaOrigen = new string(original.PlanetaOrigen.ToCharArray());
        Dieta = new string(original.Dieta.ToCharArray());
        Retrato = (byte[])original.Retrato.Clone();
        Bonus = original.Bonus with { };
    }

    public string Nombre { get; }
    public string Color { get; }
    public string PlanetaOrigen { get; }
    public string Dieta { get; }
    public byte[] Retrato { get; }
    public BonusEspecie Bonus { get; }
}
