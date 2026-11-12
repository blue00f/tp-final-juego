namespace Kepler7.Core.Visitor;

public interface IVisitor
{
    string Nombre { get; }
    string Descripcion { get; }

    void Visitar(Arma arma);
    void Visitar(Pocion pocion);
    void Visitar(Armadura armadura);

    string Resultado { get; }

    void Reiniciar();
}
