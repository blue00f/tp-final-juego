namespace Kepler7.Core.ChainOfResponsibility;

internal sealed record FotoCadena(List<IManejadorDanio> Eslabones, Dictionary<Escudo, int> PuntosEscudo);
