namespace Kepler7.Core.Proxy;

public interface IBoveda
{
    string LeerArchivo(string id);

    void EscribirArchivo(string id, string contenido);

    IReadOnlyList<string> ListarArchivos();
}
