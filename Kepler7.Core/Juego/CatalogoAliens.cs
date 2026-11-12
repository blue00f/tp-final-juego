using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Modelo;
using Kepler7.Core.Strategy;

namespace Kepler7.Core.Juego;

public static class CatalogoAliens
{
    public const int Cantidad = 6;

    public static IReadOnlyList<InfoSector> Sectores { get; } = new[]
    {
        new InfoSector(1, "Acechador Ácido", "Depredador de los conductos de ventilación. Escupe ácido y paraliza con su rugido.",
            "IA Agresiva", "Restaura COMUNICACIONES (Bridge)"),
        new InfoSector(2, "Centinela de Cristal", "Coloso cristalino. Bloquea y amortigua casi todo; se repliega cuando está herido.",
            "IA Defensiva", "Restaura el LABORATORIO (Prototype)"),
        new InfoSector(3, "Espectro Sónico", "Se desvanece entre los ecos. Muy difícil de golpear y completamente impredecible.",
            "IA Aleatoria", "Restaura el REGISTRO DE TRIPULACIÓN (Flyweight)"),
        new InfoSector(4, "Devorador de Núcleos", "Se alimenta del reactor. Calcula cada golpe y protege su núcleo con un escudo.",
            "IA Calculadora", "Restaura el HANGAR (Builder)"),
        new InfoSector(5, "Matriarca Parasitaria", "Madre de la plaga. Enorme, venenosa y obsesionada con el Capitán.",
            "IA Agresiva", "Restaura la BÓVEDA (Proxy)"),
        new InfoSector(6, "Leviatán del Vacío", "El origen de la crisis. Blindado, con escudo y un frenesí letal al verse herido.",
            "IA Calculadora", "Salva Kepler-7: fin de la campaña")
    };

    public static InfoSector Info(int sector) => Sectores[Math.Clamp(sector, 1, Cantidad) - 1];

    public static SubsistemaEstacion? Restaura(int sector) =>
        sector >= 1 && sector <= 5 ? (SubsistemaEstacion)sector : null;

    public static Enemigo Crear(int numero, IAleatorio aleatorio)
    {
        var info = Info(numero);
        Enemigo enemigo;
        switch (numero)
        {
            case 2:
                enemigo = new Enemigo(info.Alien, info.Descripcion, 480, 66, new IADefensiva())
                {
                    Apariencia = new AparienciaAlien("#5AD2FF", 1, 4, 1.05)
                };
                enemigo.Defensa.Agregar(new Bloqueo(5));
                enemigo.Defensa.Agregar(new Armadura(0.20));
                break;
            case 3:
                enemigo = new Enemigo(info.Alien, info.Descripcion, 520, 74, new IAAleatoria())
                {
                    Apariencia = new AparienciaAlien("#E6E6FF", 5, 3, 0.95)
                };
                enemigo.Defensa.Agregar(new Esquiva(0.25, aleatorio));
                break;
            case 4:
                enemigo = new Enemigo(info.Alien, info.Descripcion, 680, 70, new IACalculadora())
                {
                    Apariencia = new AparienciaAlien("#FF8A3D", 2, 8, 1.1)
                };
                enemigo.Defensa.Agregar(new Armadura(0.15));
                enemigo.Defensa.Agregar(new Escudo(50));
                break;
            case 5:
                enemigo = new Enemigo(info.Alien, info.Descripcion, 820, 58, new IAAgresiva())
                {
                    Apariencia = new AparienciaAlien("#7CFF4F", 4, 10, 1.2)
                };
                enemigo.Defensa.Agregar(new Armadura(0.10));
                break;
            case 6:
                enemigo = new Enemigo(info.Alien, info.Descripcion, 1050, 58, new IACalculadora())
                {
                    Apariencia = new AparienciaAlien("#FF3D6E", 6, 12, 1.3),
                    EsJefe = true
                };
                enemigo.Defensa.Agregar(new Bloqueo(6));
                enemigo.Defensa.Agregar(new Armadura(0.20));
                enemigo.Defensa.Agregar(new Escudo(60));
                break;
            default:
                enemigo = new Enemigo(info.Alien, info.Descripcion, 600, 60, new IAAgresiva())
                {
                    Apariencia = new AparienciaAlien("#B25CFF", 3, 6, 1.0)
                };
                enemigo.Defensa.Agregar(new Armadura(0.10));
                break;
        }
        return enemigo;
    }
}
