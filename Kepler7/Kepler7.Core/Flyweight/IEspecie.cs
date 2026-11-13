namespace Kepler7.Core.Flyweight;

public interface IEspecie
{
    string Nombre { get; }

    string Color { get; }

    string PlanetaOrigen { get; }
    string Dieta { get; }

    byte[] Retrato { get; }

    BonusEspecie Bonus { get; }
}
