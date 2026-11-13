namespace Kepler7.Core.Bridge;

public sealed class MensajeTexto : Mensaje
{
    public const double BonusXp = 0.30;

    public MensajeTexto(ICifrado cifrado, string contenido) : base(cifrado, contenido) { }

    public override string Tipo => "Texto";
    public override string Efecto => $"Informe a la Tierra: XP de la misión +{BonusXp:P0}";

    protected override string Formatear() => $"KEPLER-7 > TIERRA | INFORME | {Contenido}";

    internal override void AplicarEfecto(EfectoTransmision efecto, double integridad)
    {
        efecto.MultiplicadorXp += BonusXp * integridad;
    }
}
