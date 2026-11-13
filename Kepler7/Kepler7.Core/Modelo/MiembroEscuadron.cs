using Kepler7.Core.Prototype;

namespace Kepler7.Core.Modelo;

public sealed class MiembroEscuadron : Personaje, IPrototipo<MiembroEscuadron>
{
    public MiembroEscuadron(string nombre, Rol rol, int vidaMaxima, int ataqueBase, Posicion posicion)
        : base(nombre, vidaMaxima, ataqueBase, posicion)
    {
        Rol = rol;
        AtaqueNatural = ataqueBase;
        Habilidades = HabilidadesIniciales(rol);
    }

    public Rol Rol { get; }

    public bool EsCapitan => Rol == Rol.Comandante;

    public int AtaqueNatural { get; internal set; }

    public List<string> Habilidades { get; private set; }

    public bool EsClon { get; private set; }

    public bool YaActuo { get; internal set; }

    public IReadOnlyList<TipoAccion> AccionesDelRol => Rol switch
    {
        Rol.Comandante => new[] { TipoAccion.Atacar, TipoAccion.Construir, TipoAccion.Mover },
        Rol.Medico => new[] { TipoAccion.Atacar, TipoAccion.Curar, TipoAccion.Mover },
        _ => new[] { TipoAccion.Atacar, TipoAccion.Mover }
    };

    public MiembroEscuadron Clonar()
    {
        var clon = new MiembroEscuadron(Nombre, Rol, VidaMaxima, AtaqueNatural, Posicion)
        {
            Vida = Vida,
            AtaqueBase = AtaqueBase,
            EsClon = true,
            Habilidades = new List<string>(Habilidades)
        };
        clon.CambiarEstado(Estado.Copiar());
        clon.Defensa.CopiarDe(Defensa);
        return clon;
    }

    private static List<string> HabilidadesIniciales(Rol rol) => rol switch
    {
        Rol.Comandante => new List<string> { "Orden de fuego", "Generador de escudo" },
        Rol.Soldado => new List<string> { "Ráfaga", "Culatazo aturdidor", "Furia de combate" },
        Rol.Medico => new List<string> { "Curación de campo", "Antídoto", "Dardo sedante" },
        _ => new List<string> { "Disparo de precisión", "Tiro crítico" }
    };

    internal override FotoPersonaje Fotografiar()
    {
        var foto = base.Fotografiar();
        foto.YaActuo = YaActuo;
        return foto;
    }

    internal override void Restaurar(FotoPersonaje foto)
    {
        base.Restaurar(foto);
        YaActuo = foto.YaActuo;
    }
}
