namespace Kepler7.Core.Command;

public interface IComando
{
    string Descripcion { get; }

    void Ejecutar();

    void Deshacer();
}
