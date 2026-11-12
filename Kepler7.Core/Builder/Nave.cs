namespace Kepler7.Core.Builder;

public sealed class Nave
{
    private readonly List<Componente> _componentes = new();

    public Nave(string receta)
    {
        Receta = receta;
        Nombre = receta;
    }

    public string Receta { get; }
    public string Nombre { get; internal set; }
    public string Plan { get; internal set; } = "";

    public Componente? Casco { get; internal set; }
    public Componente? Motor { get; internal set; }
    public Componente? Escudo { get; internal set; }
    public Componente? Armas { get; internal set; }
    public Componente? Radar { get; internal set; }
    public Componente? InteligenciaArtificial { get; internal set; }
    public Componente? TanquesCombustible { get; internal set; }

    public double MultiplicadorAtaque { get; internal set; } = 1.0;

    public int EscudoInicial { get; internal set; }

    public double EsquivaExtra { get; internal set; }

    public double MultiplicadorXp { get; internal set; } = 1.0;

    public double MultiplicadorCuracion { get; internal set; } = 1.0;

    public IReadOnlyList<Componente> Componentes => _componentes;

    internal void Instalar(Componente componente)
    {
        _componentes.Add(componente);
    }

    public string ResumenEfectos()
    {
        var partes = new List<string>();
        if (MultiplicadorAtaque > 1.0)
        {
            partes.Add($"ataque +{MultiplicadorAtaque - 1:P0}");
        }
        if (EscudoInicial > 0)
        {
            partes.Add($"escudo inicial +{EscudoInicial}");
        }
        if (EsquivaExtra > 0)
        {
            partes.Add($"esquiva +{EsquivaExtra:P0}");
        }
        if (MultiplicadorCuracion > 1.0)
        {
            partes.Add($"curación +{MultiplicadorCuracion - 1:P0}");
        }
        if (MultiplicadorXp > 1.0)
        {
            partes.Add($"XP +{MultiplicadorXp - 1:P0}");
        }
        return partes.Count == 0 ? "sin bonificaciones" : string.Join(" · ", partes);
    }
}
