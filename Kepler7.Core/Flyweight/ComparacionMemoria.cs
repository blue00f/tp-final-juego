namespace Kepler7.Core.Flyweight;

public sealed record ComparacionMemoria(
    int Tripulantes,
    int ObjetosEspecieConFlyweight,
    long BytesConFlyweight,
    int ObjetosEspecieSinFlyweight,
    long BytesSinFlyweight)
{
    public double Ahorro => BytesSinFlyweight == 0 ? 0 : 1 - (double)BytesConFlyweight / BytesSinFlyweight;
}
