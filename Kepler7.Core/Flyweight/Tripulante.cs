namespace Kepler7.Core.Flyweight;

public sealed class Tripulante
{
    public Tripulante(int identificador, string nombre, string rango, int sector, IEspecie especie)
    {
        Identificador = identificador;
        Nombre = nombre;
        Rango = rango;
        Sector = sector;
        Especie = especie;
    }

    public int Identificador { get; }
    public string Nombre { get; }
    public string Rango { get; }

    public int Sector { get; }

    public IEspecie Especie { get; }

    public string Codigo => $"K7-{Identificador:00000}";
}
