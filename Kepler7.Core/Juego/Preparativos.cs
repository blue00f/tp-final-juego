using Kepler7.Core.Bridge;
using Kepler7.Core.Builder;
using Kepler7.Core.Flyweight;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Juego;

public sealed class Preparativos
{
    public Nave? Nave { get; internal set; }

    public IEspecie? Destacamento { get; internal set; }

    public Tripulante? OficialSeguridad { get; internal set; }

    public Transmision? Transmision { get; internal set; }

    public MiembroEscuadron? ClonReserva { get; internal set; }

    public bool InteligenciaJefe { get; internal set; }

    public bool ClavesRotadas { get; internal set; }

    internal void ConsumirDeUnSoloUso()
    {
        Transmision = null;
        ClonReserva = null;
    }
}
