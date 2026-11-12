using Kepler7.Core.Bridge;
using Kepler7.Core.Juego;
using Kepler7.Core.Progreso;

namespace Kepler7.Core.Facade;

internal sealed class SubsistemaComunicaciones
{
    public List<string> AbrirCanal(Transmision? transmision, ModificadoresMision mods)
    {
        var lineas = new List<string>();
        if (transmision == null)
        {
            lineas.Add("[COMUNICACIONES] enlace con la Tierra abierto (sin transmisiones pendientes).");
        }
        else
        {
            var b = transmision.Bonificacion;
            mods.Sumar(escudo: b.EscudoInicial, xp: b.MultiplicadorXp - 1);
            mods.DanioInicialAlien += b.DanioInicialAlien;
            mods.PatronesAplicados.Add(Patrones.Bridge);
            lineas.Add($"[COMUNICACIONES] la Tierra respondió al mensaje {transmision.Paquete.Tipo} ({transmision.Paquete.Cifrado}, " +
                       $"integridad {transmision.Integridad:P0}): {transmision.Efecto}.");
            if (transmision.Interceptada)
            {
                mods.AlienAlertado = true;
                lineas.Add("[COMUNICACIONES] ¡ALERTA! Los aliens interceptaron la transmisión: el alien sabe que vamos.");
            }
        }
        return lineas;
    }

    public string EnviarSOS() => "[COMUNICACIONES] señal de socorro enviada a la Tierra.";
    public string CerrarCanal() => "[COMUNICACIONES] canal cerrado.";
}
