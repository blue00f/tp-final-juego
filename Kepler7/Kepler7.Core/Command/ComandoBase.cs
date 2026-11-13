using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Command;

public abstract class ComandoBase : IComando
{
    private FotoCombate? _antes;
    private FotoCombate? _despues;

    protected ComandoBase(Combate combate, MiembroEscuadron actor)
    {
        Combate = combate;
        Actor = actor;
    }

    protected Combate Combate { get; }
    public MiembroEscuadron Actor { get; }

    public abstract string Descripcion { get; }

    protected abstract AccionTurno CrearAccion();

    public virtual string? Validar() => null;

    // Deshacer/rehacer restauran instantáneas del combate (antes y después de ejecutar):
    // así rehacer NO vuelve a tirar los dados (esquivas, críticos).
    public void Ejecutar()
    {
        if (_despues == null)
        {
            _antes = Combate.Fotografiar();
            Combate.EjecutarTurnoMiembro(Actor, CrearAccion());
            _despues = Combate.Fotografiar();
        }
        else
        {
            Combate.Restaurar(_despues);
        }
    }

    public void Deshacer()
    {
        if (_antes != null)
        {
            Combate.Restaurar(_antes);
        }
    }
}
