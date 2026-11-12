using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;

namespace Kepler7.Core.Command;

public sealed class AtacarComando : ComandoBase
{
    private readonly Enemigo _objetivo;

    public AtacarComando(Combate combate, MiembroEscuadron actor, Enemigo objetivo) : base(combate, actor)
    {
        _objetivo = objetivo;
    }

    public override string Descripcion => $"{Actor.Nombre} ataca a {_objetivo.Nombre}";

    public override string? Validar() => _objetivo.EstaVivo ? null : "El objetivo ya fue derrotado.";

    protected override AccionTurno CrearAccion() => new(TipoAccion.Atacar, _objetivo);
}
