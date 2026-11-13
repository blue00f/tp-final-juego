namespace Kepler7.Core.ChainOfResponsibility;

public interface IManejadorDanio
{
    string Nombre { get; }

    string Descripcion { get; }

    IManejadorDanio? Siguiente { get; }

    IManejadorDanio EstablecerSiguiente(IManejadorDanio? siguiente);

    void Manejar(Danio danio);

    int Estimar(int danioEntrante);

    IManejadorDanio Clonar();
}
