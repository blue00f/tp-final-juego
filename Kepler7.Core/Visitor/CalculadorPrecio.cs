namespace Kepler7.Core.Visitor;

public sealed class CalculadorPrecio : IVisitor
{
    private int _armas;
    private int _pociones;
    private int _armaduras;

    public string Nombre => "Calculador de precio";
    public string Descripcion => "Tasa el valor total del arsenal en créditos.";

    public void Visitar(Arma arma) => _armas += arma.Precio;
    public void Visitar(Pocion pocion) => _pociones += pocion.Precio;
    public void Visitar(Armadura armadura) => _armaduras += armadura.Precio;

    public string Resultado =>
        $"Valor total: {_armas + _pociones + _armaduras} ₵  (armas {_armas} ₵ · pociones {_pociones} ₵ · armaduras {_armaduras} ₵)";

    public void Reiniciar()
    {
        _armas = 0;
        _pociones = 0;
        _armaduras = 0;
    }
}
