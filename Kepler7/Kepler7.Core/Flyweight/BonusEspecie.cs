namespace Kepler7.Core.Flyweight;

public sealed record BonusEspecie(
    string Descripcion,
    double MultiplicadorAtaque = 1.0,
    int EscudoInicial = 0,
    double EsquivaExtra = 0,
    double MultiplicadorCuracion = 1.0,
    double MultiplicadorXp = 1.0);
