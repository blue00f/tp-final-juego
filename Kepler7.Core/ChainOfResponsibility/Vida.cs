using Kepler7.Core.Modelo;

namespace Kepler7.Core.ChainOfResponsibility;

public sealed class Vida : ManejadorDanioBase
{
    private readonly Personaje _personaje;

    public Vida(Personaje personaje)
    {
        _personaje = personaje;
    }

    public override string Nombre => "Vida";
    public override string Descripcion => $"Vida {_personaje.Vida}/{_personaje.VidaMaxima}";

    public override void Manejar(Danio danio)
    {
        int vidaAntes = _personaje.Vida;
        _personaje.PerderVida(danio.Restante);
        danio.RegistrarVidaPerdida(Nombre, vidaAntes - _personaje.Vida);
    }

    public override int Estimar(int danioEntrante) => danioEntrante;

    public override IManejadorDanio Clonar() =>
        throw new NotSupportedException("El eslabón Vida pertenece a un personaje y no se clona.");
}
