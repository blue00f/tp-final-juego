using System.Text.Json;
using System.Text.Json.Serialization;
using Kepler7.Core.Juego;

namespace Kepler7.Core.Progreso;

public sealed class ProgresoJugador
{
    public const int XpPorNivel = 150;

    public int Experiencia { get; set; }
    public int AliensDerrotados { get; set; }
    public int Victorias { get; set; }
    public int Derrotas { get; set; }
    public List<string> PatronesDominados { get; set; } = new();
    public List<RegistroMision> Historial { get; set; } = new();
    public DateTime? UltimaPartida { get; set; }

    public string? NaveReceta { get; set; }
    public string? NavePlan { get; set; }
    public string? Destacamento { get; set; }
    public int? OficialId { get; set; }
    public bool InteligenciaJefe { get; set; }
    public bool ClavesRotadas { get; set; }

    [JsonIgnore]
    public int Nivel => 1 + Experiencia / XpPorNivel;

    [JsonIgnore]
    public int XpEnNivelActual => Experiencia % XpPorNivel;

    [JsonIgnore]
    public bool CampaniaCompletada => AliensDerrotados >= CatalogoAliens.Cantidad;

    [JsonIgnore]
    public string Rango => Nivel switch
    {
        1 => "Cadete",
        2 => "Alférez",
        3 => "Teniente",
        4 => "Capitán de corbeta",
        5 => "Capitán de fragata",
        6 => "Capitán de navío",
        _ => "Comandante de flota"
    };

    public bool Dominar(string patron)
    {
        bool nuevo = !PatronesDominados.Contains(patron);
        if (nuevo)
        {
            PatronesDominados.Add(patron);
        }
        return nuevo;
    }

    public List<string> RegistrarResultado(ResumenCombate resumen)
    {
        Experiencia += resumen.XpGanada;
        if (resumen.Resultado == ResultadoCombate.Victoria)
        {
            Victorias++;
            if (resumen.Sector == AliensDerrotados + 1)
            {
                AliensDerrotados++;
            }
        }
        else
        {
            Derrotas++;
        }

        Historial.Add(new RegistroMision(resumen.Sector, resumen.Enemigo, resumen.Resultado, resumen.Rondas,
            resumen.DanioInfligido, resumen.XpGanada, DateTime.Now));

        var nuevos = resumen.PatronesObservados.Where(Dominar).ToList();
        UltimaPartida = DateTime.Now;
        return nuevos;
    }
}
