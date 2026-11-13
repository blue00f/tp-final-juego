namespace Kepler7.Core.Bridge;

public sealed class SinCifrado : ICifrado
{
    public string Nombre => "Sin cifrado";
    public string Descripcion => "Llega completo, pero los aliens interceptan el 100% de las transmisiones.";
    public double ProbabilidadIntercepcion => 1.0;
    public double Integridad => 1.0;

    public string Cifrar(string textoPlano) => textoPlano;

    public string Descifrar(string textoCifrado) => textoCifrado;
}
