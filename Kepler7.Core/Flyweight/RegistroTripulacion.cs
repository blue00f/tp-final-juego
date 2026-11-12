namespace Kepler7.Core.Flyweight;

public sealed class RegistroTripulacion
{
    public const int Total = 10_000;
    public const string RangoSeguridad = "Oficial de Seguridad";

    private static readonly string[] Nombres =
    {
        "Ada", "Bruno", "Ciro", "Dana", "Elio", "Fara", "Gael", "Hana", "Ivo", "Juno", "Kai", "Lara", "Milo",
        "Nora", "Otto", "Pia", "Quin", "Rhea", "Saúl", "Tess", "Ugo", "Vera", "Wen", "Xio", "Yael", "Zoe"
    };

    private static readonly string[] Apellidos =
    {
        "Vance", "Okoro", "Sato", "Ibarra", "Novak", "Reyes", "Lund", "Kaur", "Moreau", "Duarte", "Brandt", "Halley",
        "Kepler", "Orion", "Zhou", "Ferro", "Nyx", "Solano", "Varga", "Quill"
    };

    private static readonly (string Rango, int Peso)[] Rangos =
    {
        ("Técnico", 40), ("Científico", 22), ("Ingeniero", 20), ("Piloto", 10), ("Teniente", 6), (RangoSeguridad, 2)
    };

    private readonly List<Tripulante> _tripulantes = new(Total);

    public RegistroTripulacion(FabricaEspecies fabrica, int semilla = 2026)
    {
        Fabrica = fabrica;
        var random = new Random(semilla);
        int pesoTotal = Rangos.Sum(r => r.Peso);
        for (int id = 1; id <= Total; id++)
        {
            string nombre = $"{Nombres[random.Next(Nombres.Length)]} {Apellidos[random.Next(Apellidos.Length)]}";
            string especie = FabricaEspecies.Nombres[random.Next(FabricaEspecies.Nombres.Count)];
            string rango = ElegirRango(random.Next(pesoTotal));
            _tripulantes.Add(new Tripulante(id, nombre, rango, random.Next(12), fabrica.ObtenerEspecie(especie)));
        }
    }

    public FabricaEspecies Fabrica { get; }

    private static string ElegirRango(int tirada)
    {
        string elegido = Rangos[^1].Rango;
        int acumulado = 0;
        foreach (var (rango, peso) in Rangos)
        {
            acumulado += peso;
            if (tirada < acumulado)
            {
                elegido = rango;
                break;
            }
        }
        return elegido;
    }

    public IReadOnlyList<Tripulante> Tripulantes => _tripulantes;

    public int EspeciesDistintasReferenciadas =>
        _tripulantes.Select(t => t.Especie).Distinct().Count(); // Especie no redefine Equals: compara referencias

    public Tripulante? Buscar(int identificador) =>
        identificador >= 1 && identificador <= _tripulantes.Count ? _tripulantes[identificador - 1] : null;

    public IEnumerable<Tripulante> Filtrar(string? especie, string? rango) =>
        _tripulantes.Where(t => (especie == null || t.Especie.Nombre == especie) && (rango == null || t.Rango == rango));

    public int Contar(string especie) => _tripulantes.Count(t => t.Especie.Nombre == especie);

    public ComparacionMemoria CompararMemoria()
    {
        long conFlyweight = Fabrica.Especies.Sum(TamanioEstimado);

        long antes = GC.GetTotalMemory(true);
        var copias = new IEspecie[_tripulantes.Count];
        for (int i = 0; i < copias.Length; i++)
        {
            copias[i] = new EspecieSinCompartir(_tripulantes[i].Especie);
        }
        long despues = GC.GetTotalMemory(true);
        long sinFlyweight = Math.Max(despues - antes, copias.Sum(TamanioEstimado));
        GC.KeepAlive(copias);

        return new ComparacionMemoria(_tripulantes.Count, Fabrica.Cantidad, conFlyweight, copias.Length, sinFlyweight);
    }

    private static long TamanioEstimado(IEspecie especie)
    {
        const int encabezadoObjeto = 24;
        long textos = (especie.Nombre.Length + especie.Color.Length + especie.PlanetaOrigen.Length + especie.Dieta.Length) * 2 + 4 * 22;
        return encabezadoObjeto + textos + especie.Retrato.Length + 24 + 64;
    }
}
