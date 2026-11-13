using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Core.TemplateMethod;

public sealed class TurnoAlien : TurnoUnidad
{
    public const int EnfriamientoVeneno = 3;
    public const int EnfriamientoAturdir = 4;
    public const int EnfriamientoDefensa = 3;
    public const int EscudoDefensivo = 40;

    public override string NombreTurno => "Turno de Alien";

    protected override AccionTurno Decidir(Personaje unidad, Contexto contexto)
    {
        var enemigo = (Enemigo)unidad;
        var decision = enemigo.Estrategia.DecidirAccion(enemigo, contexto);
        enemigo.UltimaDecision = decision;
        contexto.Registrar(CategoriaRegistro.IA, $"[{enemigo.Estrategia.Nombre}] {decision.Razon}");
        return new AccionTurno(decision.Tipo, decision.Objetivo);
    }

    protected override void Actuar(Personaje unidad, AccionTurno accion, Contexto contexto)
    {
        var enemigo = (Enemigo)unidad;
        var objetivo = accion.Objetivo ?? contexto.Capitan;

        switch (accion.Tipo)
        {
            case TipoAccion.Atacar:
                contexto.Golpear(enemigo, objetivo, enemigo.Atacar(), "Garra");
                break;

            case TipoAccion.Envenenar:
                var mordida = contexto.Golpear(enemigo, objetivo, (int)Math.Round(enemigo.Atacar() * 0.6), "Escupitajo ácido");
                if (!mordida.Esquivado && objetivo.EstaVivo)
                {
                    contexto.AplicarEstado(objetivo, new Envenenado(3, 8), "ácido del alien");
                }
                enemigo.IniciarEnfriamiento(TipoAccion.Envenenar, EnfriamientoVeneno);
                break;

            case TipoAccion.Aturdir:
                var rugido = contexto.Golpear(enemigo, objetivo, (int)Math.Round(enemigo.Atacar() * 0.5), "Rugido sónico");
                if (!rugido.Esquivado && objetivo.EstaVivo)
                {
                    contexto.AplicarEstado(objetivo, new Aturdido(1), "rugido sónico");
                }
                enemigo.IniciarEnfriamiento(TipoAccion.Aturdir, EnfriamientoAturdir);
                break;

            case TipoAccion.Defender:
                var escudo = enemigo.Defensa.Buscar<Escudo>();
                if (escudo == null)
                {
                    enemigo.Defensa.Agregar(new Escudo(EscudoDefensivo));
                }
                else
                {
                    escudo.Recargar(EscudoDefensivo);
                }
                contexto.Registrar(CategoriaRegistro.Danio, $"◈ {enemigo.Nombre} endurece su caparazón: {enemigo.Defensa.Describir()}");
                contexto.AgregarEvento(TipoEvento.Escudo, enemigo, EscudoDefensivo, $"+{EscudoDefensivo} ◈");
                enemigo.IniciarEnfriamiento(TipoAccion.Defender, EnfriamientoDefensa);
                break;

            default:
                contexto.Registrar(CategoriaRegistro.Sistema, $"{enemigo.Nombre} no hace nada.");
                break;
        }
    }

    protected override void Finalizar(Personaje unidad, Contexto contexto)
    {
        base.Finalizar(unidad, contexto);
        ((Enemigo)unidad).AvanzarEnfriamientos();
    }
}
