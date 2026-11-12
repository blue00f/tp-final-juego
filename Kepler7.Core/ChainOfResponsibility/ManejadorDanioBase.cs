namespace Kepler7.Core.ChainOfResponsibility;

public abstract class ManejadorDanioBase : IManejadorDanio
{
    public abstract string Nombre { get; }
    public abstract string Descripcion { get; }

    public IManejadorDanio? Siguiente { get; private set; }

    public IManejadorDanio EstablecerSiguiente(IManejadorDanio? siguiente)
    {
        Siguiente = siguiente;
        return siguiente ?? this;
    }

    public abstract void Manejar(Danio danio);

    public abstract int Estimar(int danioEntrante);

    public abstract IManejadorDanio Clonar();

    protected void TransferirAlSiguiente(Danio danio)
    {
        if (danio.Restante > 0 && Siguiente != null)
        {
            Siguiente.Manejar(danio);
        }
    }
}
