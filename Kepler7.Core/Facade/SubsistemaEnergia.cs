namespace Kepler7.Core.Facade;


internal sealed class SubsistemaEnergia
{
    public int Nivel { get; private set; }

    public string Encender()
    {
        Nivel = 100;
        return "[ENERGÍA] reactor de fusión en línea (100%).";
    }

    public string RedirigirAEscudos()
    {
        Nivel = Math.Max(0, Nivel - 40);
        return $"[ENERGÍA] 40% redirigido a escudos (reactor al {Nivel}%).";
    }

    public string Apagar()
    {
        Nivel = 0;
        return "[ENERGÍA] reactor en parada segura.";
    }
}
