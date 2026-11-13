namespace Kepler7.Core.Visitor;

public interface IItem
{
    string Nombre { get; }
    int Precio { get; }

    void Aceptar(IVisitor visitante);
}
