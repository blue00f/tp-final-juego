using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Core.TemplateMethod;

public abstract class TurnoUnidad
{
    public abstract string NombreTurno { get; }

    public void EjecutarTurno(Personaje unidad, Contexto contexto)
    {
        RegistrarEtapa(contexto, unidad, "1·Inicio", $"estado {unidad.Estado.Nombre}, vida {unidad.Vida}/{unidad.VidaMaxima}");
        Iniciar(unidad, contexto);

        if (unidad.EstaVivo)
        {
            var accion = Decidir(unidad, contexto);
            if (!unidad.Estado.Permite(accion.Tipo))
            {
                RegistrarEtapa(contexto, unidad, "2·Decisión", $"quería {accion.Tipo}, pero el estado {unidad.Estado.Nombre} lo impide → espera");
                accion = new AccionTurno(TipoAccion.Esperar);
            }
            else
            {
                RegistrarEtapa(contexto, unidad, "2·Decisión", DescribirAccion(accion));
            }

            Actuar(unidad, accion, contexto);
            RegistrarEtapa(contexto, unidad, "3·Acción", "resuelta");

            Finalizar(unidad, contexto);
            RegistrarEtapa(contexto, unidad, "4·Final", $"estado {unidad.Estado.Nombre}, vida {unidad.Vida}/{unidad.VidaMaxima}");
        }
        else
        {
            RegistrarEtapa(contexto, unidad, "—", "cayó al inicio del turno; no actúa");
        }
    }

    protected virtual void Iniciar(Personaje unidad, Contexto contexto)
    {
        unidad.Estado.AlIniciarTurno(unidad, contexto);
    }

    protected virtual AccionTurno Decidir(Personaje unidad, Contexto contexto)
    {
        return contexto.AccionSolicitada ?? new AccionTurno(TipoAccion.Esperar);
    }

    protected abstract void Actuar(Personaje unidad, AccionTurno accion, Contexto contexto);

    protected virtual void Finalizar(Personaje unidad, Contexto contexto)
    {
        unidad.Estado.AlFinalizarTurno(unidad, contexto);
        if (unidad is MiembroEscuadron miembro)
        {
            miembro.YaActuo = true;
        }
    }

    protected static void RegistrarEtapa(Contexto contexto, Personaje unidad, string etapa, string detalle)
    {
        contexto.Registrar(CategoriaRegistro.Turno, $"[{unidad.Nombre}] {etapa}: {detalle}");
    }

    protected static string DescribirAccion(AccionTurno accion)
    {
        string objetivo = accion.Objetivo != null ? $" → {accion.Objetivo.Nombre}" : "";
        return $"{accion.Tipo}{objetivo}";
    }

    protected static void AtaqueDeMiembro(MiembroEscuadron miembro, AccionTurno accion, Contexto contexto,
                                          double factor, string nombreAtaque)
    {
        var objetivo = accion.Objetivo as Enemigo ?? contexto.EnemigosVivos.FirstOrDefault();
        if (objetivo != null)
        {
            int cantidad = miembro.Atacar();
            if (miembro.Posicion == Posicion.Retaguardia && miembro.Rol != Rol.Francotirador)
            {
                cantidad = (int)Math.Round(cantidad * 0.7);
            }
            cantidad = (int)Math.Round(cantidad * factor);

            var danio = contexto.Golpear(miembro, objetivo, cantidad, nombreAtaque);

            var arma = contexto.Inventario.ArmaDe(miembro.Rol);
            bool envenena = arma is { Envenenada: true } && danio.VidaPerdida > 0 && objetivo.EstaVivo
                            && objetivo.Estado.Tipo == TipoEstado.Normal;
            if (envenena)
            {
                contexto.AplicarEstado(objetivo, new Envenenado(3, 10), $"arma envenenada: {arma!.Nombre}");
            }
        }
    }

    protected static void MoverUnidad(Personaje unidad, Contexto contexto)
    {
        bool movido = unidad.Mover();
        string texto = movido ? $"{unidad.Nombre} pasa a {unidad.Posicion}." : $"{unidad.Nombre} no puede moverse en estado {unidad.Estado.Nombre}.";
        contexto.Registrar(CategoriaRegistro.Sistema, texto);
    }
}
