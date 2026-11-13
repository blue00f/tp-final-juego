namespace Kepler7.Core.Juego;

public sealed record ResumenCombate(
    ResultadoCombate Resultado,
    int Sector,
    bool EsJefe,
    string Enemigo,
    int Rondas,
    int DanioInfligido,
    int DanioRecibido,
    int CuracionTotal,
    int ComandosEjecutados,
    int AccionesDeshechas,
    int BajasPropias,
    int XpGanada,
    IReadOnlyCollection<string> PatronesObservados);
