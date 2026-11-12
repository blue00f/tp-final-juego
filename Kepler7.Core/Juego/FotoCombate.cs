using Kepler7.Core.Modelo;
using Kepler7.Core.Strategy;

namespace Kepler7.Core.Juego;

internal sealed record FotoCombate(
    List<FotoPersonaje> Personajes,
    List<MiembroEscuadron> Formacion,
    EstadisticasCombate Estadisticas,
    ResultadoCombate Resultado,
    FaseCombate Fase,
    bool EmergenciaUsada,
    bool AdaptacionRealizada,
    bool FrenesiJefe,
    MiembroEscuadron? Reserva,
    IEstrategiaIA Estrategia);
