using Kepler7.Core.Builder;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.Progreso;

namespace Kepler7.Core.Facade;

internal sealed class SubsistemaNavegacion
{
    public string FijarRumbo(Nave? nave, ModificadoresMision mods)
    {
        string linea = "[NAVEGACIÓN] rumbo fijado al punto de contacto (sin nave asignada: despliegue a pie).";
        if (nave != null)
        {
            mods.Sumar(nave.MultiplicadorAtaque - 1, nave.EscudoInicial, nave.EsquivaExtra,
                nave.MultiplicadorCuracion - 1, nave.MultiplicadorXp - 1);
            mods.PatronesAplicados.Add(Patrones.Builder);
            linea = $"[NAVEGACIÓN] despliegue en la nave {nave.Nombre}: {nave.ResumenEfectos()}.";
        }
        return linea;
    }

    public string ManiobraEvasiva(MiembroEscuadron capitan)
    {
        string texto;
        if (capitan.Posicion == Posicion.Vanguardia)
        {
            capitan.CambiarPosicion();
            texto = "[NAVEGACIÓN] maniobra evasiva, el Capitán se repliega a retaguardia.";
        }
        else
        {
            texto = "[NAVEGACIÓN] maniobra evasiva (el Capitán ya está en retaguardia).";
        }
        return texto;
    }

    public string Anclar() => "[NAVEGACIÓN] estación anclada en órbita estable.";
}
