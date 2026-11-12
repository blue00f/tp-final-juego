using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.State;

namespace Kepler7.Core.Modelo;

internal sealed class FotoPersonaje
{
    public FotoPersonaje(Personaje personaje)
    {
        Personaje = personaje;
    }

    public Personaje Personaje { get; }
    public int Vida { get; init; }
    public int AtaqueBase { get; init; }
    public Posicion Posicion { get; init; }
    public required IEstadoPersonaje Estado { get; init; }
    public required FotoCadena Cadena { get; init; }

    public bool YaActuo { get; set; }
    public Dictionary<TipoAccion, int>? Enfriamientos { get; set; }
    public Personaje? UltimoAgresor { get; set; }
}
