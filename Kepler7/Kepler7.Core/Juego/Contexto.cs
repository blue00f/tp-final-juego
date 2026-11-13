using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Comun;
using Kepler7.Core.Modelo;
using Kepler7.Core.Progreso;
using Kepler7.Core.State;
using Kepler7.Core.Visitor;

namespace Kepler7.Core.Juego;

public sealed class Contexto
{
    private readonly Registro _registro;
    private readonly List<EventoCombate> _eventos = new();
    private readonly List<Danio> _danios = new();

    internal Contexto(Escuadron escuadron, IReadOnlyList<Enemigo> enemigos, Inventario inventario,
                      IAleatorio aleatorio, Registro registro)
    {
        Escuadron = escuadron;
        Enemigos = enemigos;
        Inventario = inventario;
        Aleatorio = aleatorio;
        _registro = registro;
    }

    public Escuadron Escuadron { get; }
    public IReadOnlyList<Enemigo> Enemigos { get; }
    public IAleatorio Aleatorio { get; }
    public int Ronda { get; internal set; } = 1;

    public MiembroEscuadron Capitan => Escuadron.Capitan;
    public IEnumerable<MiembroEscuadron> AliadosVivos => Escuadron.Miembros.Where(m => m.EstaVivo);
    public IEnumerable<Enemigo> EnemigosVivos => Enemigos.Where(e => e.EstaVivo);

    internal Inventario Inventario { get; }

    internal AccionTurno? AccionSolicitada { get; set; }

    internal EstadisticasCombate Estadisticas { get; set; } = new();

    public ModificadoresMision Modificadores { get; internal set; } = new();

    internal HashSet<string> PatronesObservados { get; } = new();

    internal IReadOnlyList<Danio> Danios => _danios;

    internal void Registrar(CategoriaRegistro categoria, string texto) => _registro.Agregar(Ronda, categoria, texto);

    internal void AgregarEvento(TipoEvento tipo, Personaje objetivo, int valor, string texto)
    {
        _eventos.Add(new EventoCombate(tipo, objetivo, valor, texto));
    }

    internal List<EventoCombate> ConsumirEventos()
    {
        var copia = new List<EventoCombate>(_eventos);
        _eventos.Clear();
        return copia;
    }

    internal void Observar(string patron) => PatronesObservados.Add(patron);

    internal void AplicarEstado(Personaje personaje, IEstadoPersonaje estado, string motivo)
    {
        personaje.CambiarEstado(estado);
        Registrar(CategoriaRegistro.Estado, $"{personaje.Nombre} → {estado.Nombre} ({motivo}).");
        AgregarEvento(TipoEvento.Estado, personaje, 0, estado.Nombre.ToUpperInvariant());
        Observar(Patrones.State);
    }

    internal Danio Golpear(Personaje atacante, Personaje objetivo, int cantidad, string nombreAtaque)
    {
        int cantidadFinal = objetivo.Posicion == Posicion.Retaguardia ? (int)Math.Round(cantidad * 0.6) : cantidad;
        var danio = objetivo.RecibirDanio(cantidadFinal, atacante.Nombre);
        _danios.Add(danio);

        string posicion = objetivo.Posicion == Posicion.Retaguardia ? " [retaguardia -40%]" : "";
        Registrar(CategoriaRegistro.Danio,
            $"{nombreAtaque}: {atacante.Nombre} → {objetivo.Nombre}: {danio.Original}{posicion} | {danio.ResumenRecorrido()}");

        if (danio.Esquivado)
        {
            AgregarEvento(TipoEvento.Esquiva, objetivo, 0, "¡ESQUIVA!");
        }
        else
        {
            AgregarEvento(TipoEvento.Danio, objetivo, danio.VidaPerdida, $"-{danio.VidaPerdida}");
        }

        if (danio.Recorrido.Any(p => p.Eslabon != "Vida" && p.Absorbido > 0))
        {
            Observar(Patrones.ChainOfResponsibility);
        }

        if (atacante is MiembroEscuadron)
        {
            Estadisticas.DanioInfligido += danio.VidaPerdida;
        }
        else
        {
            Estadisticas.DanioRecibido += danio.VidaPerdida;
        }

        if (objetivo is Enemigo enemigo)
        {
            enemigo.UltimoAgresor = atacante;
        }

        if (!objetivo.EstaVivo)
        {
            Registrar(CategoriaRegistro.Sistema, $"✖ {objetivo.Nombre} ha caído.");
            AgregarEvento(TipoEvento.Caida, objetivo, 0, "CAÍDO");
        }

        return danio;
    }

    internal int Sanar(Personaje sanador, Personaje objetivo, int cantidad)
    {
        int curado = objetivo.Curar(cantidad);
        Estadisticas.CuracionTotal += curado;
        AgregarEvento(TipoEvento.Curacion, objetivo, curado, $"+{curado}");
        return curado;
    }

    public int EstimarDanio(Personaje objetivo, int cantidad)
    {
        int ajustado = objetivo.Posicion == Posicion.Retaguardia ? (int)Math.Round(cantidad * 0.6) : cantidad;
        if (objetivo.Estado.Tipo is TipoEstado.Aturdido or TipoEstado.Berserk)
        {
            ajustado = (int)Math.Round(ajustado * 1.25);
        }
        return objetivo.Defensa.Estimar(ajustado);
    }
}
