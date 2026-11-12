using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Juego;
using Kepler7.Core.Progreso;

namespace Kepler7.Core.Facade;

internal sealed class SubsistemaSeguridad
{
    public string SellarBoveda(bool inteligenciaJefe, ModificadoresMision mods)
    {
        string linea = "[SEGURIDAD] bóveda sellada, protocolos de acceso activos.";
        if (inteligenciaJefe)
        {
            mods.DebilidadJefe = true;
            mods.PatronesAplicados.Add(Patrones.Proxy);
            linea = "[SEGURIDAD] bóveda sellada. Informe INTEL-LEVIATAN cargado en los visores del escuadrón.";
        }
        return linea;
    }

    public string CierreHermetico() => "[SEGURIDAD] cierre hermético de compartimentos.";
    public string Bloqueo() => "[SEGURIDAD] bloqueo total de accesos.";
}
