using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Command;

public sealed class MoverComando : ComandoBase
{
    public MoverComando(Combate combate, MiembroEscuadron actor) : base(combate, actor) { }

    public override string Descripcion
    {
        get
        {
            string destino = Actor.Posicion == Posicion.Vanguardia ? "retaguardia" : "vanguardia";
            return $"{Actor.Nombre} se mueve a {destino}";
        }
    }

    protected override AccionTurno CrearAccion() => new(TipoAccion.Mover);
}
