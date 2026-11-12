using Kepler7.Core.Comun;

namespace Kepler7.Core.Bridge;

public sealed class CentroComunicaciones
{
    private readonly IAleatorio _aleatorio;

    public CentroComunicaciones(IAleatorio aleatorio)
    {
        _aleatorio = aleatorio;
    }

    public static IReadOnlyList<string> TiposMensaje { get; } = new[] { "Texto", "Alerta", "Emergencia" };

    public static IReadOnlyList<string> TiposCifrado { get; } = new[] { "Sin cifrado", "AES-256", "Cuántico" };

    public static Mensaje Crear(string tipoMensaje, string tipoCifrado, string contenido, bool clavesRotadas)
    {
        ICifrado cifrado = tipoCifrado switch
        {
            "AES-256" => new CifradoAES(clavesRotadas),
            "Cuántico" => new CifradoCuantico(),
            _ => new SinCifrado()
        };
        Mensaje mensaje = tipoMensaje switch
        {
            "Alerta" => new MensajeAlerta(cifrado, contenido),
            "Emergencia" => new MensajeEmergencia(cifrado, contenido),
            _ => new MensajeTexto(cifrado, contenido)
        };
        return mensaje;
    }

    public Transmision Enviar(Mensaje mensaje)
    {
        var paquete = mensaje.Empaquetar();
        bool interceptada = _aleatorio.Probabilidad(mensaje.Cifrado.ProbabilidadIntercepcion);
        var efecto = new EfectoTransmision();
        mensaje.AplicarEfecto(efecto, mensaje.Cifrado.Integridad);
        return new Transmision(paquete, mensaje.Efecto, mensaje.Cifrado.Integridad, interceptada, efecto);
    }
}
