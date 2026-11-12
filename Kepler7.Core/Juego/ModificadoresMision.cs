using Kepler7.Core.Modelo;

namespace Kepler7.Core.Juego;

public sealed class ModificadoresMision
{
    public double MultiplicadorAtaque { get; internal set; } = 1.0;
    public int EscudoInicial { get; internal set; }
    public double EsquivaExtra { get; internal set; }
    public double MultiplicadorCuracion { get; internal set; } = 1.0;
    public double MultiplicadorXp { get; internal set; } = 1.0;

    public double DanioInicialAlien { get; internal set; }

    public bool AlienAlertado { get; internal set; }

    public bool DebilidadJefe { get; internal set; }

    public MiembroEscuadron? ClonReserva { get; internal set; }

    public List<string> PatronesAplicados { get; } = new();

    internal void Sumar(double ataque = 0, int escudo = 0, double esquiva = 0, double curacion = 0, double xp = 0)
    {
        MultiplicadorAtaque += ataque;
        EscudoInicial += escudo;
        EsquivaExtra += esquiva;
        MultiplicadorCuracion += curacion;
        MultiplicadorXp += xp;
    }
}
