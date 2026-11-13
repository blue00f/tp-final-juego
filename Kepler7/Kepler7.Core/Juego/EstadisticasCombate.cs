namespace Kepler7.Core.Juego;

public sealed class EstadisticasCombate
{
    public int DanioInfligido { get; set; }
    public int DanioRecibido { get; set; }
    public int CuracionTotal { get; set; }

    public EstadisticasCombate Copiar() => new()
    {
        DanioInfligido = DanioInfligido,
        DanioRecibido = DanioRecibido,
        CuracionTotal = CuracionTotal
    };
}
