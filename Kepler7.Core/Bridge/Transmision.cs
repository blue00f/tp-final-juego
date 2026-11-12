namespace Kepler7.Core.Bridge;

public sealed record Transmision(
    PaqueteMensaje Paquete,
    string Efecto,
    double Integridad,
    bool Interceptada,
    EfectoTransmision Bonificacion);
