using System.Text.Json;

namespace Kepler7.Core.Proxy;

public sealed class BovedaDatos : IBoveda
{
    public const string ArchivoIntel = "INTEL-LEVIATAN";
    public const string ArchivoClaves = "CLAVES-COM";

    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };
    private readonly Dictionary<string, string> _archivos;
    private readonly string? _ruta;

    public BovedaDatos(string? carpeta)
    {
        _ruta = carpeta == null ? null : Path.Combine(carpeta, "boveda.json");
        _archivos = Cargar() ?? ArchivosIniciales();
    }

    public string LeerArchivo(string id)
    {
        if (!_archivos.TryGetValue(id, out var contenido))
        {
            throw new KeyNotFoundException($"No existe el archivo {id}.");
        }
        return contenido;
    }

    public void EscribirArchivo(string id, string contenido)
    {
        _archivos[id] = contenido;
        Guardar();
    }

    public IReadOnlyList<string> ListarArchivos() => _archivos.Keys.OrderBy(k => k).ToList();

    public static IReadOnlyList<string> IndicePublico { get; } = ArchivosIniciales().Keys.OrderBy(k => k).ToList();

    private static Dictionary<string, string> ArchivosIniciales() => new()
    {
        [ArchivoIntel] =
            "Informe forense del Leviatán del Vacío:\n" +
            "Su caparazón tiene una grieta en la placa dorsal. Concentrando fuego allí, " +
            "su ARMADURA y su ESCUDO dejan de funcionar.\n" +
            "→ Efecto: el jefe final pierde los eslabones Armadura y Escudo de su cadena.",
        [ArchivoClaves] =
            "Claves de cifrado AES de fábrica (comprometidas: los aliens las capturaron).\n" +
            "→ Escribí este archivo para rotarlas: AES pasa a 0% de intercepción.",
        ["PROTOCOLO-CLONACION"] =
            "Protocolo 7-B: solo se clonan tripulantes con autorización del Capitán. " +
            "Los clones conservan la memoria muscular del original pero no sus recuerdos.",
        ["BITACORA-CAPITAN"] = "Día 1 de la crisis. Los conductos están infestados. Que la estación resista."
    };

    private Dictionary<string, string>? Cargar()
    {
        Dictionary<string, string>? archivos = null;
        if (_ruta != null && File.Exists(_ruta))
        {
            try
            {
                archivos = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(_ruta));
            }
            catch (JsonException)
            {
                archivos = null;
            }
        }
        return archivos;
    }

    private void Guardar()
    {
        if (_ruta != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
            File.WriteAllText(_ruta, JsonSerializer.Serialize(_archivos, Opciones));
        }
    }
}
