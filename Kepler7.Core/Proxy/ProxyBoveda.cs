namespace Kepler7.Core.Proxy;

public sealed class ProxyBoveda : IBoveda
{
    private static readonly RolAcceso[] RolesAutorizados = { RolAcceso.Capitan, RolAcceso.OficialSeguridad };

    private readonly Func<BovedaDatos> _crearBoveda;
    private BovedaDatos? _real;

    public ProxyBoveda(Func<BovedaDatos> crearBoveda, RegistroAccesos registro)
    {
        _crearBoveda = crearBoveda;
        Registro = registro;
    }

    public RegistroAccesos Registro { get; }

    public Usuario? Operador { get; set; }

    public bool BovedaAbierta => _real != null;

    public static bool EstaAutorizado(RolAcceso rol) => RolesAutorizados.Contains(rol);

    public string LeerArchivo(string id)
    {
        Verificar("Leer", id);
        return Real().LeerArchivo(id);
    }

    public void EscribirArchivo(string id, string contenido)
    {
        Verificar("Escribir", id);
        Real().EscribirArchivo(id, contenido);
    }

    public IReadOnlyList<string> ListarArchivos() => _real?.ListarArchivos() ?? BovedaDatos.IndicePublico;

    private void Verificar(string operacion, string id)
    {
        var operador = Operador ?? throw new AccesoDenegadoException("No hay operador en la terminal.");
        if (!EstaAutorizado(operador.Rol))
        {
            Registro.Registrar(new IntentoDenegado(DateTime.Now, operador.Nombre, operador.Rol, operacion, id));
            throw new AccesoDenegadoException(
                $"ACCESO DENEGADO: {operador.Nombre} ({operador.Rol}) no puede {operacion.ToLowerInvariant()} {id}. Intento registrado.");
        }
    }

    private BovedaDatos Real()
    {
        _real ??= _crearBoveda();
        return _real;
    }
}
