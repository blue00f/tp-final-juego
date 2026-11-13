using Kepler7.Core.Comun;

namespace Kepler7.Core.ChainOfResponsibility;

public sealed class Esquiva : ManejadorDanioBase
{
    private readonly IAleatorio _aleatorio;

    public Esquiva(double probabilidad, IAleatorio aleatorio)
    {
        Probabilidad = probabilidad;
        _aleatorio = aleatorio;
    }

    public double Probabilidad { get; }

    public override string Nombre => "Esquiva";
    public override string Descripcion => $"Esquiva {Probabilidad:P0}";

    public override void Manejar(Danio danio)
    {
        if (_aleatorio.Probabilidad(Probabilidad))
        {
            danio.Esquivar(Nombre);
        }
        else
        {
            danio.Transferir(Nombre, $"no logra esquivar ({Probabilidad:P0}), transfiere");
            TransferirAlSiguiente(danio);
        }
    }

    // Valor esperado: la IA no conoce el resultado del azar.
    public override int Estimar(int danioEntrante) => (int)Math.Round(danioEntrante * (1 - Probabilidad));

    public override IManejadorDanio Clonar() => new Esquiva(Probabilidad, _aleatorio);
}
