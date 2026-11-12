namespace Kepler7.Core.Proxy;

public sealed record IntentoDenegado(DateTime Fecha, string Usuario, RolAcceso Rol, string Operacion, string Archivo);
