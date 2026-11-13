namespace Kepler7.Core.Bridge;

public sealed class MensajeEmergencia : Mensaje
{
    public const double Bombardeo = 0.20;

    public MensajeEmergencia(ICifrado cifrado, string contenido) : base(cifrado, contenido) { }

    public override string Tipo => "Emergencia";
    public override string Efecto => $"Bombardeo orbital: el alien empieza con -{Bombardeo:P0} de vida";

    protected override string Formatear() =>
        $"MAYDAY MAYDAY MAYDAY | KEPLER-7 | ÓRBITA 7-B | {Contenido} | SOLICITAMOS FUEGO ORBITAL";

    internal override void AplicarEfecto(EfectoTransmision efecto, double integridad)
    {
        efecto.DanioInicialAlien += Bombardeo * integridad;
    }
}
