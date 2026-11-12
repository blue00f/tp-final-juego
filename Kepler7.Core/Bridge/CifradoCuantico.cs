using System.Text;

namespace Kepler7.Core.Bridge;

public sealed class CifradoCuantico : ICifrado
{
    // Cuatro bases de polarización = 2 bits por fotón.
    private static readonly char[] Fotones = { '↑', '→', '↗', '↖' };
    private readonly byte[] _libreta;

    public CifradoCuantico(int semilla = 7)
    {
        _libreta = new byte[4096];
        new Random(semilla).NextBytes(_libreta);
    }

    public string Nombre => "Cuántico";
    public string Descripcion => "Indescifrable para los aliens, pero el canal pierde el 30% de la potencia.";
    public double ProbabilidadIntercepcion => 0.0;
    public double Integridad => 0.7;

    public string Cifrar(string textoPlano)
    {
        byte[] datos = Encoding.UTF8.GetBytes(textoPlano);
        var salida = new StringBuilder(datos.Length * 4);
        for (int i = 0; i < datos.Length; i++)
        {
            byte b = (byte)(datos[i] ^ _libreta[i % _libreta.Length]);
            for (int desplazamiento = 6; desplazamiento >= 0; desplazamiento -= 2)
            {
                salida.Append(Fotones[(b >> desplazamiento) & 0b11]);
            }
        }
        return salida.ToString();
    }

    public string Descifrar(string textoCifrado)
    {
        var bytes = new byte[textoCifrado.Length / 4];
        for (int i = 0; i < bytes.Length; i++)
        {
            int valor = 0;
            for (int j = 0; j < 4; j++)
            {
                valor = (valor << 2) | Array.IndexOf(Fotones, textoCifrado[i * 4 + j]);
            }
            bytes[i] = (byte)(valor ^ _libreta[i % _libreta.Length]);
        }
        return Encoding.UTF8.GetString(bytes);
    }
}
