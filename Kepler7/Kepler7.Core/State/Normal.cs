using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.State;

public sealed class Normal : IEstadoPersonaje
{
    public TipoEstado Tipo => TipoEstado.Normal;
    public string Nombre => "Normal";
    public string Descripcion => "Sin efectos.";
    public int TurnosRestantes => 0;

    public bool Permite(TipoAccion accion) => true;

    public int Atacar(Personaje personaje) => personaje.AtaqueBase;

    public bool Mover(Personaje personaje)
    {
        personaje.CambiarPosicion();
        return true;
    }

    public void RecibirDanio(Personaje personaje, Danio danio) => personaje.Defensa.Procesar(danio);

    public void AlIniciarTurno(Personaje personaje, Contexto contexto) { }

    public void AlFinalizarTurno(Personaje personaje, Contexto contexto) { }

    public IEstadoPersonaje Copiar() => new Normal();
}
