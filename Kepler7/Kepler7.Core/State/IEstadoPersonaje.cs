using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.State;

public interface IEstadoPersonaje
{
    TipoEstado Tipo { get; }
    string Nombre { get; }
    string Descripcion { get; }

    int TurnosRestantes { get; }

    bool Permite(TipoAccion accion);

    int Atacar(Personaje personaje);

    bool Mover(Personaje personaje);

    void RecibirDanio(Personaje personaje, Danio danio);

    void AlIniciarTurno(Personaje personaje, Contexto contexto);

    void AlFinalizarTurno(Personaje personaje, Contexto contexto);

    IEstadoPersonaje Copiar();
}
