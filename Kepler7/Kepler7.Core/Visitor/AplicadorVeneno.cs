namespace Kepler7.Core.Visitor;

public sealed class AplicadorVeneno : IVisitor
{
    private int _aplicadas;
    private int _yaEnvenenadas;

    public string Nombre => "Aplicador de veneno";
    public string Descripcion => "Envenena las armas: los golpes que hieren envenenan al alien.";

    public void Visitar(Arma arma)
    {
        if (arma.Envenenada)
        {
            _yaEnvenenadas++;
        }
        else
        {
            arma.Envenenada = true;
            _aplicadas++;
        }
    }

    public void Visitar(Pocion pocion) { }
    public void Visitar(Armadura armadura) { }

    public string Resultado => _aplicadas > 0
        ? $"☠ {_aplicadas} armas envenenadas. Los golpes que hieran envenenarán al alien."
        : $"Las {_yaEnvenenadas} armas ya estaban envenenadas.";

    public void Reiniciar()
    {
        _aplicadas = 0;
        _yaEnvenenadas = 0;
    }
}
