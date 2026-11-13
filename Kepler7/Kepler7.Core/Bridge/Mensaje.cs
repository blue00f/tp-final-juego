namespace Kepler7.Core.Bridge;

public abstract class Mensaje
{
    protected Mensaje(ICifrado cifrado, string contenido)
    {
        Cifrado = cifrado;
        Contenido = contenido;
    }

    public ICifrado Cifrado { get; set; }

    public string Contenido { get; }

    public abstract string Tipo { get; }
    public abstract string Efecto { get; }

    protected abstract string Formatear();

    internal abstract void AplicarEfecto(EfectoTransmision efecto, double integridad);

    public PaqueteMensaje Empaquetar()
    {
        string plano = Formatear();
        string cifrado = Cifrado.Cifrar(plano);
        string recibido = Cifrado.Descifrar(cifrado);
        return new PaqueteMensaje(Tipo, Cifrado.Nombre, plano, cifrado, recibido);
    }
}
