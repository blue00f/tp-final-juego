using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Prototype;

public sealed class LaboratorioBiotecnologia
{
    public static IReadOnlyList<MejoraGenetica> Mejoras { get; } = new[]
    {
        new MejoraGenetica("Injerto de quitina", "+30 de vida máxima"),
        new MejoraGenetica("Neuroimplante táctico", "+4 de ataque"),
        new MejoraGenetica("Reflejos sintéticos", "agrega un eslabón Esquiva 20% a su cadena"),
        new MejoraGenetica("Placas subdérmicas", "agrega un eslabón Bloqueo 6 a su cadena")
    };

    private int _numeroClon;

    public MiembroEscuadron Clonar(MiembroEscuadron original)
    {
        if (original.EsCapitan)
        {
            throw new InvalidOperationException("El protocolo de la estación prohíbe clonar al Capitán.");
        }
        _numeroClon++;
        var clon = original.Clonar();
        clon.Nombre = $"{original.Nombre} [C-{_numeroClon}]";
        return clon;
    }

    public void Mejorar(MiembroEscuadron clon, MejoraGenetica mejora, IAleatorio aleatorio)
    {
        if (!clon.EsClon)
        {
            throw new InvalidOperationException("Las mejoras genéticas solo se aplican a clones.");
        }

        switch (mejora.Nombre)
        {
            case "Injerto de quitina":
                clon.VidaMaxima += 30;
                clon.Vida = clon.VidaMaxima;
                break;
            case "Neuroimplante táctico":
                clon.AtaqueNatural += 4;
                clon.AtaqueBase += 4;
                break;
            case "Reflejos sintéticos":
                clon.Defensa.Agregar(new Esquiva(0.20, aleatorio));
                break;
            case "Placas subdérmicas":
                clon.Defensa.Agregar(new Bloqueo(6));
                break;
        }
        clon.Habilidades.Add(mejora.Nombre);
    }

    public static IReadOnlyList<DiferenciaClon> Comparar(MiembroEscuadron original, MiembroEscuadron clon)
    {
        return new[]
        {
            new DiferenciaClon("Nombre", original.Nombre, clon.Nombre),
            new DiferenciaClon("Vida máxima", original.VidaMaxima.ToString(), clon.VidaMaxima.ToString()),
            new DiferenciaClon("Ataque", original.AtaqueNatural.ToString(), clon.AtaqueNatural.ToString()),
            new DiferenciaClon("Habilidades", string.Join(", ", original.Habilidades), string.Join(", ", clon.Habilidades)),
            new DiferenciaClon("Cadena de defensas", original.Defensa.Describir(), clon.Defensa.Describir()),
            new DiferenciaClon("¿Misma lista en memoria?", "—",
                ReferenceEquals(original.Habilidades, clon.Habilidades) ? "SÍ (copia superficial)" : "NO (copia profunda)"),
            new DiferenciaClon("¿Misma cadena en memoria?", "—",
                ReferenceEquals(original.Defensa, clon.Defensa) ? "SÍ (copia superficial)" : "NO (copia profunda)")
        };
    }
}
