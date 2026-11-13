using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.State;

public sealed class Berserk : IEstadoPersonaje
{
    public Berserk(int turnos)
    {
        TurnosRestantes = turnos;
    }

    public TipoEstado Tipo => TipoEstado.Berserk;
    public string Nombre => "Berserk";
    public string Descripcion => "Ataque x1.5, recibe +25% de daño. Solo puede atacar.";
    public int TurnosRestantes { get; private set; }

    public bool Permite(TipoAccion accion) =>
        accion is TipoAccion.Atacar or TipoAccion.Esperar or TipoAccion.Envenenar or TipoAccion.Aturdir;

    public int Atacar(Personaje personaje) => (int)Math.Round(personaje.AtaqueBase * 1.5);

    public bool Mover(Personaje personaje) => false;

    public void RecibirDanio(Personaje personaje, Danio danio)
    {
        danio.AplicarMultiplicador("Berserk", 1.25);
        personaje.Defensa.Procesar(danio);
    }

    public void AlIniciarTurno(Personaje personaje, Contexto contexto) { }

    public void AlFinalizarTurno(Personaje personaje, Contexto contexto)
    {
        TurnosRestantes--;
        if (TurnosRestantes <= 0)
        {
            personaje.CambiarEstado(new Normal());
            contexto.Registrar(CategoriaRegistro.Estado, $"{personaje.Nombre} se calma → Normal.");
        }
    }

    public IEstadoPersonaje Copiar() => new Berserk(TurnosRestantes);
}
