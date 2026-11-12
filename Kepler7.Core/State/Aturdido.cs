using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.State;

public sealed class Aturdido : IEstadoPersonaje
{
    public Aturdido(int turnos)
    {
        TurnosRestantes = turnos;
    }

    public TipoEstado Tipo => TipoEstado.Aturdido;
    public string Nombre => "Aturdido";
    public string Descripcion => "Pierde el turno. Recibe +25% de daño.";
    public int TurnosRestantes { get; private set; }

    public bool Permite(TipoAccion accion) => accion == TipoAccion.Esperar;

    public int Atacar(Personaje personaje) => 0;

    public bool Mover(Personaje personaje) => false;

    public void RecibirDanio(Personaje personaje, Danio danio)
    {
        danio.AplicarMultiplicador("Aturdido", 1.25);
        personaje.Defensa.Procesar(danio);
    }

    public void AlIniciarTurno(Personaje personaje, Contexto contexto)
    {
        contexto.Registrar(CategoriaRegistro.Estado, $"✦ {personaje.Nombre} está aturdido y no puede actuar.");
    }

    public void AlFinalizarTurno(Personaje personaje, Contexto contexto)
    {
        TurnosRestantes--;
        if (TurnosRestantes <= 0)
        {
            personaje.CambiarEstado(new Normal());
            contexto.Registrar(CategoriaRegistro.Estado, $"{personaje.Nombre} se recupera del aturdimiento → Normal.");
        }
    }

    public IEstadoPersonaje Copiar() => new Aturdido(TurnosRestantes);
}
