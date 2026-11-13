namespace Kepler7.Core.Visitor;

public sealed class Pocion : IItem
{
    public Pocion(string nombre, int precio, int curacion)
    {
        Nombre = nombre;
        Precio = precio;
        Curacion = curacion;
    }

    public string Nombre { get; }
    public int Precio { get; }
    public int Curacion { get; }

    public void Aceptar(IVisitor visitante) => visitante.Visitar(this);
}
