namespace Kepler7.Core.Proxy;

public sealed class AccesoDenegadoException : Exception
{
    public AccesoDenegadoException(string mensaje) : base(mensaje) { }
}
