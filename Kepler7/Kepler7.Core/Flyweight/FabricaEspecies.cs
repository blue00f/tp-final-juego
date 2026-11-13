namespace Kepler7.Core.Flyweight;

public sealed class FabricaEspecies
{
    private readonly Dictionary<string, IEspecie> _especies = new();

    public static IReadOnlyList<string> Nombres { get; } = new[] { "Humano", "Kepleriano", "Zorvan", "Lumari", "Ferrox" };

    public int Cantidad => _especies.Count;

    public int Solicitudes { get; private set; }

    public IReadOnlyCollection<IEspecie> Especies => _especies.Values;

    public IEspecie ObtenerEspecie(string nombre)
    {
        Solicitudes++;
        if (!_especies.TryGetValue(nombre, out var especie))
        {
            especie = Crear(nombre);
            _especies[nombre] = especie;
        }
        return especie;
    }

    private static IEspecie Crear(string nombre) => nombre switch
    {
        "Humano" => new Especie("Humano", "#4FC3F7", "Tierra", "Omnívora",
            new BonusEspecie("Paramédicos: curación +20%", MultiplicadorCuracion: 1.20)),
        "Kepleriano" => new Especie("Kepleriano", "#81C784", "Kepler-7b", "Algas fotosintéticas",
            new BonusEspecie("Ingenieros de escudos: escudo inicial +12", EscudoInicial: 12)),
        "Zorvan" => new Especie("Zorvan", "#E57373", "Zorva Prime", "Carnívora",
            new BonusEspecie("Guerreros natos: ataque +10%", MultiplicadorAtaque: 1.10)),
        "Lumari" => new Especie("Lumari", "#BA68C8", "Nebulosa Lumen", "Energía estelar",
            new BonusEspecie("Precognición: esquiva +8%", EsquivaExtra: 0.08)),
        "Ferrox" => new Especie("Ferrox", "#FFB74D", "Ferrus IV", "Minerales",
            new BonusEspecie("Recuperan tecnología alienígena: XP +20%", MultiplicadorXp: 1.20)),
        _ => throw new ArgumentException($"Especie desconocida: {nombre}")
    };
}
