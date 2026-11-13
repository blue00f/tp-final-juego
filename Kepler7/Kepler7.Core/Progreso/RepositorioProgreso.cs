using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kepler7.Core.Progreso;

public sealed class RepositorioProgreso
{
    private static readonly JsonSerializerOptions Opciones = new() { WriteIndented = true };

    public RepositorioProgreso(string carpeta)
    {
        Carpeta = carpeta;
    }

    public string Carpeta { get; }

    public string RutaArchivo => Path.Combine(Carpeta, "progreso.json");

    public ProgresoJugador Cargar()
    {
        ProgresoJugador progreso = new();
        if (File.Exists(RutaArchivo))
        {
            try
            {
                string json = File.ReadAllText(RutaArchivo);
                progreso = JsonSerializer.Deserialize<ProgresoJugador>(json, Opciones) ?? new ProgresoJugador();
            }
            catch (JsonException)
            {
                progreso = new ProgresoJugador();
            }
        }
        return progreso;
    }

    public void Guardar(ProgresoJugador progreso)
    {
        Directory.CreateDirectory(Carpeta);
        string json = JsonSerializer.Serialize(progreso, Opciones);
        File.WriteAllText(RutaArchivo, json);
    }

    public void Reiniciar()
    {
        foreach (var archivo in new[] { RutaArchivo, Path.Combine(Carpeta, "boveda.json") })
        {
            if (File.Exists(archivo))
            {
                File.Delete(archivo);
            }
        }
    }
}
