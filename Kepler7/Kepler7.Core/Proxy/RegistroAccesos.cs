using System.Text.Json;

namespace Kepler7.Core.Proxy;

public sealed class RegistroAccesos
{
    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };
    private readonly List<IntentoDenegado> _intentos;
    private readonly string? _ruta;

    public RegistroAccesos(string? carpeta)
    {
        _ruta = carpeta == null ? null : Path.Combine(carpeta, "accesos_denegados.json");
        _intentos = Cargar();
    }

    public IReadOnlyList<IntentoDenegado> Intentos => _intentos;

    internal void Registrar(IntentoDenegado intento)
    {
        _intentos.Add(intento);
        if (_ruta != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
            File.WriteAllText(_ruta, JsonSerializer.Serialize(_intentos, Opciones));
        }
    }

    private List<IntentoDenegado> Cargar()
    {
        var intentos = new List<IntentoDenegado>();
        if (_ruta != null && File.Exists(_ruta))
        {
            try
            {
                intentos = JsonSerializer.Deserialize<List<IntentoDenegado>>(File.ReadAllText(_ruta)) ?? intentos;
            }
            catch (JsonException)
            {
            }
        }
        return intentos;
    }
}
