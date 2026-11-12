using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.State;

public sealed class Envenenado : IEstadoPersonaje
{
    public Envenenado(int turnos, int danioPorTurno)
    {
        TurnosRestantes = turnos;
        DanioPorTurno = danioPorTurno;
    }

    public int DanioPorTurno { get; }

    public TipoEstado Tipo => TipoEstado.Envenenado;
    public string Nombre => "Envenenado";
    public string Descripcion => $"Pierde {DanioPorTurno} de vida por turno. Ataque al 80%.";
    public int TurnosRestantes { get; private set; }

    public bool Permite(TipoAccion accion) => true;

    public int Atacar(Personaje personaje) => (int)Math.Round(personaje.AtaqueBase * 0.8);

    public bool Mover(Personaje personaje)
    {
        personaje.CambiarPosicion();
        return true;
    }

    public void RecibirDanio(Personaje personaje, Danio danio) => personaje.Defensa.Procesar(danio);

    public void AlIniciarTurno(Personaje personaje, Contexto contexto)
    {
        // El veneno ignora la cadena de defensas: va directo a la vida.
        int vidaAntes = personaje.Vida;
        personaje.PerderVida(DanioPorTurno);
        int perdida = vidaAntes - personaje.Vida;
        TurnosRestantes--;

        contexto.Registrar(CategoriaRegistro.Estado, $"☠ {personaje.Nombre} sufre {perdida} de daño por veneno ({TurnosRestantes} turnos restantes).");
        contexto.AgregarEvento(TipoEvento.Veneno, personaje, perdida, $"-{perdida} ☠");

        if (TurnosRestantes <= 0 && personaje.EstaVivo)
        {
            personaje.CambiarEstado(new Normal());
            contexto.Registrar(CategoriaRegistro.Estado, $"{personaje.Nombre}: el veneno se disipa → Normal.");
        }
    }

    public void AlFinalizarTurno(Personaje personaje, Contexto contexto) { }

    public IEstadoPersonaje Copiar() => new Envenenado(TurnosRestantes, DanioPorTurno);
}
