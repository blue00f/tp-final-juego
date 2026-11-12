namespace Kepler7.Core.Bridge;

public sealed class MensajeAlerta : Mensaje
{
    public const int Escudo = 20;

    public MensajeAlerta(ICifrado cifrado, string contenido) : base(cifrado, contenido) { }

    public override string Tipo => "Alerta";
    public override string Efecto => $"Pedido de suministros: escudo inicial +{Escudo} a todo el escuadrón";

    protected override string Formatear() => $"!! ALERTA NIVEL 2 !! KEPLER-7 SOLICITA SUMINISTROS | {Contenido} | FIN ALERTA";

    internal override void AplicarEfecto(EfectoTransmision efecto, double integridad)
    {
        efecto.EscudoInicial += (int)Math.Round(Escudo * integridad);
    }
}
