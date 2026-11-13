using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.State;

namespace Kepler7.Core.Modelo;

public abstract class Personaje
{
    protected Personaje(string nombre, int vidaMaxima, int ataqueBase, Posicion posicion)
    {
        Nombre = nombre;
        VidaMaxima = vidaMaxima;
        Vida = vidaMaxima;
        AtaqueBase = ataqueBase;
        Posicion = posicion;
        Estado = new Normal();
        Defensa = new CadenaDefensa(this);
    }

    public string Nombre { get; internal set; }
    public int VidaMaxima { get; internal set; }
    public int Vida { get; internal set; }
    public int AtaqueBase { get; internal set; }
    public Posicion Posicion { get; internal set; }

    public IEstadoPersonaje Estado { get; private set; }

    public CadenaDefensa Defensa { get; }

    public bool EstaVivo => Vida > 0;

    public double PorcentajeVida => (double)Vida / VidaMaxima;

    public int PuntosEscudo => Defensa.Eslabones.OfType<Escudo>().Sum(e => e.Puntos);

    public void CambiarEstado(IEstadoPersonaje nuevoEstado)
    {
        Estado = nuevoEstado;
    }

    public int Atacar() => Estado.Atacar(this);

    public bool Mover() => Estado.Mover(this);

    public Danio RecibirDanio(int cantidad, string origen)
    {
        var danio = new Danio(cantidad, origen, Nombre);
        Estado.RecibirDanio(this, danio);
        return danio;
    }

    internal void PerderVida(int cantidad)
    {
        Vida = Math.Max(0, Vida - cantidad);
    }

    internal int Curar(int cantidad)
    {
        int vidaAnterior = Vida;
        Vida = Math.Min(VidaMaxima, Vida + cantidad);
        return Vida - vidaAnterior;
    }

    internal void CambiarPosicion()
    {
        Posicion = Posicion == Posicion.Vanguardia ? Posicion.Retaguardia : Posicion.Vanguardia;
    }

    // ---- Instantáneas para que los comandos puedan deshacer con exactitud ----

    internal virtual FotoPersonaje Fotografiar()
    {
        return new FotoPersonaje(this)
        {
            Vida = Vida,
            AtaqueBase = AtaqueBase,
            Posicion = Posicion,
            Estado = Estado.Copiar(),
            Cadena = Defensa.Capturar()
        };
    }

    internal virtual void Restaurar(FotoPersonaje foto)
    {
        Vida = foto.Vida;
        AtaqueBase = foto.AtaqueBase;
        Posicion = foto.Posicion;
        Estado = foto.Estado.Copiar();
        Defensa.Restaurar(foto.Cadena);
    }
}
