namespace Kepler7.Core.ChainOfResponsibility;

public sealed class Escudo : ManejadorDanioBase
{
    public Escudo(int puntos)
    {
        Puntos = puntos;
    }

    public int Puntos { get; internal set; }

    public bool Agotado => Puntos <= 0;

    public override string Nombre => "Escudo";
    public override string Descripcion => $"Escudo {Puntos} pts";

    internal void Recargar(int puntos)
    {
        Puntos += puntos;
    }

    public override void Manejar(Danio danio)
    {
        int absorbido = Math.Min(Puntos, danio.Restante);
        Puntos -= absorbido;
        string detalle = Agotado ? $"absorbe {absorbido} y se agota" : $"absorbe {absorbido} (quedan {Puntos} pts)";
        danio.Reducir(Nombre, absorbido, detalle);
        TransferirAlSiguiente(danio);
    }

    public override int Estimar(int danioEntrante) => Math.Max(0, danioEntrante - Puntos);

    public override IManejadorDanio Clonar() => new Escudo(Puntos);
}
