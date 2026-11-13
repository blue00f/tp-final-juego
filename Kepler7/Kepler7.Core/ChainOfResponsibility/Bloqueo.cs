namespace Kepler7.Core.ChainOfResponsibility;

public sealed class Bloqueo : ManejadorDanioBase
{
    public Bloqueo(int cantidad)
    {
        Cantidad = cantidad;
    }

    public int Cantidad { get; }

    public override string Nombre => "Bloqueo";
    public override string Descripcion => $"Bloqueo {Cantidad}";

    public override void Manejar(Danio danio)
    {
        bool bloqueaTodo = danio.Restante <= Cantidad;
        string detalle = bloqueaTodo ? "bloquea el golpe completo" : $"bloquea {Cantidad}, transfiere el resto";
        danio.Reducir(Nombre, Cantidad, detalle);
        TransferirAlSiguiente(danio);
    }

    public override int Estimar(int danioEntrante) => Math.Max(0, danioEntrante - Cantidad);

    public override IManejadorDanio Clonar() => new Bloqueo(Cantidad);
}
