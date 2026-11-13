namespace Kepler7.Core.ChainOfResponsibility;

public sealed class Armadura : ManejadorDanioBase
{
    public Armadura(double reduccion)
    {
        Reduccion = reduccion;
    }

    public double Reduccion { get; }

    public override string Nombre => "Armadura";
    public override string Descripcion => $"Armadura {Reduccion:P0}";

    public override void Manejar(Danio danio)
    {
        int reducido = (int)Math.Round(danio.Restante * Reduccion);
        danio.Reducir(Nombre, reducido, $"amortigua {Reduccion:P0}");
        TransferirAlSiguiente(danio);
    }

    public override int Estimar(int danioEntrante) => danioEntrante - (int)Math.Round(danioEntrante * Reduccion);

    public override IManejadorDanio Clonar() => new Armadura(Reduccion);
}
