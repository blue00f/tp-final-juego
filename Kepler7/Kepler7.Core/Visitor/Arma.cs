using Kepler7.Core.Modelo;

namespace Kepler7.Core.Visitor;

public sealed class Arma : IItem
{
    public Arma(string nombre, int precio, int bonusDanio, Rol portador)
    {
        Nombre = nombre;
        Precio = precio;
        BonusDanio = bonusDanio;
        Portador = portador;
    }

    public string Nombre { get; }
    public int Precio { get; }
    public int BonusDanio { get; }
    public Rol Portador { get; }

    public bool Envenenada { get; internal set; }

    public void Aceptar(IVisitor visitante) => visitante.Visitar(this);
}
