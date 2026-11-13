namespace Kepler7.Core.Juego;

public sealed record RegistroMision(
    int Sector,
    string Enemigo,
    ResultadoCombate Resultado,
    int Rondas,
    int DanioInfligido,
    int XpGanada,
    DateTime Fecha);
