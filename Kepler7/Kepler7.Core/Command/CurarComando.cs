using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Core.Command;

public sealed class CurarComando : ComandoBase
{
    private readonly MiembroEscuadron _objetivo;

    public CurarComando(Combate combate, MiembroEscuadron actor, MiembroEscuadron objetivo) : base(combate, actor)
    {
        _objetivo = objetivo;
    }

    public override string Descripcion => $"{Actor.Nombre} cura a {_objetivo.Nombre}";

    public override string? Validar()
    {
        string? error = null;
        if (!_objetivo.EstaVivo)
        {
            error = "No se puede curar a un tripulante caído.";
        }
        else if (_objetivo.Vida == _objetivo.VidaMaxima && _objetivo.Estado.Tipo != TipoEstado.Envenenado)
        {
            error = $"{_objetivo.Nombre} ya tiene la vida completa.";
        }
        return error;
    }

    protected override AccionTurno CrearAccion() => new(TipoAccion.Curar, _objetivo);
}
