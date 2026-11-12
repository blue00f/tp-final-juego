using Kepler7.Core.Modelo;

namespace Kepler7.Core.Visitor;

public sealed class Armadura : IItem
{
    public Armadura(string nombre, int precio, double reduccion, Rol portador)
    {
        Nombre = nombre;
        Precio = precio;
        Reduccion = reduccion;
        Portador = portador;
    }

    public string Nombre { get; }
    public int Precio { get; }
    public double Reduccion { get; }
    public Rol Portador { get; }

    public void Aceptar(IVisitor visitante) => visitante.Visitar(this);
}
