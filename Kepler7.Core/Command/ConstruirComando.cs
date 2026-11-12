using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Command;

public sealed class ConstruirComando : ComandoBase
{
    private readonly MiembroEscuadron _objetivo;

    public ConstruirComando(Combate combate, MiembroEscuadron actor, MiembroEscuadron objetivo) : base(combate, actor)
    {
        _objetivo = objetivo;
    }

    public override string Descripcion => $"{Actor.Nombre} despliega un generador de escudo sobre {_objetivo.Nombre}";

    public override string? Validar() => _objetivo.EstaVivo ? null : "No se puede proteger a un tripulante caído.";

    protected override AccionTurno CrearAccion() => new(TipoAccion.Construir, _objetivo);
}
