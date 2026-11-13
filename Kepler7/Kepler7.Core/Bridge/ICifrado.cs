namespace Kepler7.Core.Bridge;

public interface ICifrado
{
    string Nombre { get; }
    string Descripcion { get; }

    double ProbabilidadIntercepcion { get; }

    double Integridad { get; }

    string Cifrar(string textoPlano);

    string Descifrar(string textoCifrado);
}
