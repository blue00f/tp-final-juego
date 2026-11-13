namespace Kepler7.Core.Prototype;

public sealed record DiferenciaClon(string Atributo, string Original, string Clon)
{
    public bool Distinto => Original != Clon;
}
