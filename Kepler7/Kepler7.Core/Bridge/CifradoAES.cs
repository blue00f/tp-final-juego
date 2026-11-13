using System.Security.Cryptography;
using System.Text;

namespace Kepler7.Core.Bridge;

public sealed class CifradoAES : ICifrado
{
    private readonly byte[] _clave;
    private readonly bool _clavesRotadas;

    public CifradoAES(bool clavesRotadas = false)
    {
        _clavesRotadas = clavesRotadas;
        string frase = clavesRotadas ? "KEPLER-7/CLAVES-ROTADAS" : "KEPLER-7/CLAVE-DE-FABRICA";
        using var sha = SHA256.Create();
        _clave = sha.ComputeHash(Encoding.UTF8.GetBytes(frase));
    }

    public string Nombre => "AES-256";
    public string Descripcion => _clavesRotadas
        ? "Claves rotadas desde la bóveda: los aliens ya no pueden descifrarlo."
        : "Cifrado fuerte, pero los aliens robaron claves viejas: 30% de intercepción.";
    public double ProbabilidadIntercepcion => _clavesRotadas ? 0.0 : 0.30;
    public double Integridad => 1.0;

    public string Cifrar(string textoPlano)
    {
        using var aes = Aes.Create();
        aes.Key = _clave;
        aes.GenerateIV();
        byte[] plano = Encoding.UTF8.GetBytes(textoPlano);
        using var cifrador = aes.CreateEncryptor();
        byte[] cifrado = cifrador.TransformFinalBlock(plano, 0, plano.Length);
        return Convert.ToBase64String(aes.IV.Concat(cifrado).ToArray());
    }

    public string Descifrar(string textoCifrado)
    {
        byte[] datos = Convert.FromBase64String(textoCifrado);
        using var aes = Aes.Create();
        aes.Key = _clave;
        aes.IV = datos.Take(16).ToArray();
        using var descifrador = aes.CreateDecryptor();
        byte[] plano = descifrador.TransformFinalBlock(datos, 16, datos.Length - 16);
        return Encoding.UTF8.GetString(plano);
    }
}
