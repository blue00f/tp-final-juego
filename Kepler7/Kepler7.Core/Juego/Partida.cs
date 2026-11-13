using Kepler7.Core.Bridge;
using Kepler7.Core.Builder;
using Kepler7.Core.Comun;
using Kepler7.Core.Facade;
using Kepler7.Core.Flyweight;
using Kepler7.Core.Modelo;
using Kepler7.Core.Progreso;
using Kepler7.Core.Prototype;
using Kepler7.Core.Proxy;
using Kepler7.Core.Visitor;

namespace Kepler7.Core.Juego;

public sealed class Partida
{
    public const int MaximoMejorasPorClon = 2;

    private RegistroTripulacion? _registro;
    private int _mejorasDelClon;

    private Partida(ProgresoJugador progreso, IAleatorio aleatorio, string? carpetaDatos, bool sandbox)
    {
        Progreso = progreso;
        Aleatorio = aleatorio;
        EsSandbox = sandbox;
        Inventario = Inventario.CrearInicial();
        Escuadron = FabricaEscuadron.Crear(Inventario);
        Preparativos = new Preparativos();
        Control = new ControlCentral(Escuadron, Inventario, Preparativos, aleatorio);

        Ingeniero = new IngenieroJefe();
        Laboratorio = new LaboratorioBiotecnologia();
        Comunicaciones = new CentroComunicaciones(aleatorio);
        Fabrica = new FabricaEspecies();
        Boveda = new ProxyBoveda(() => new BovedaDatos(carpetaDatos), new RegistroAccesos(carpetaDatos));

        RestaurarPreparativosGuardados();
    }

    public static Partida Nueva(ProgresoJugador progreso, IAleatorio? aleatorio = null, string? carpetaDatos = null)
    {
        return new Partida(progreso, aleatorio ?? new AleatorioSistema(), carpetaDatos, sandbox: false);
    }

    public static Partida NuevaSandbox(IAleatorio? aleatorio = null)
    {
        return new Partida(new ProgresoJugador(), aleatorio ?? new AleatorioSistema(), null, sandbox: true);
    }

    public event Action<string>? PatronDominado;

    public ProgresoJugador Progreso { get; }
    public Inventario Inventario { get; }
    public Escuadron Escuadron { get; }
    public ControlCentral Control { get; }
    public IAleatorio Aleatorio { get; }
    public Preparativos Preparativos { get; }
    public bool EsSandbox { get; }

    public IngenieroJefe Ingeniero { get; }
    public LaboratorioBiotecnologia Laboratorio { get; }
    public CentroComunicaciones Comunicaciones { get; }
    public FabricaEspecies Fabrica { get; }
    public ProxyBoveda Boveda { get; }

    public RegistroTripulacion Registro => _registro ??= new RegistroTripulacion(Fabrica);

    public int SectorSandbox { get; set; } = 1;

    public int SectorActual => EsSandbox
        ? Math.Clamp(SectorSandbox, 1, CatalogoAliens.Cantidad)
        : Math.Min(Progreso.AliensDerrotados + 1, CatalogoAliens.Cantidad);

    public bool Desbloqueado(SubsistemaEstacion subsistema) =>
        EsSandbox || Progreso.AliensDerrotados >= (int)subsistema;

    public Nave ConstruirNave(INaveBuilder receta, PlanEnsamblaje plan)
    {
        ExigirDesbloqueo(SubsistemaEstacion.Hangar);
        var nave = Ingeniero.Construir(receta, plan);
        Preparativos.Nave = nave;
        Progreso.NaveReceta = receta.NombreReceta;
        Progreso.NavePlan = plan.ToString();
        Dominar(Patrones.Builder);
        return nave;
    }

    public MiembroEscuadron ClonarTripulante(MiembroEscuadron original)
    {
        ExigirDesbloqueo(SubsistemaEstacion.Laboratorio);
        var clon = Laboratorio.Clonar(original);
        Preparativos.ClonReserva = clon;
        _mejorasDelClon = 0;
        return clon;
    }

    public int MejorasDisponibles => Preparativos.ClonReserva == null ? 0 : MaximoMejorasPorClon - _mejorasDelClon;

    public void MejorarClon(MejoraGenetica mejora)
    {
        var clon = Preparativos.ClonReserva ?? throw new InvalidOperationException("Primero hay que clonar a un tripulante.");
        if (MejorasDisponibles <= 0)
        {
            throw new InvalidOperationException($"Estabilidad genética al límite: máximo {MaximoMejorasPorClon} mejoras por clon.");
        }
        Laboratorio.Mejorar(clon, mejora, Aleatorio);
        _mejorasDelClon++;
        Dominar(Patrones.Prototype);
    }

    public bool PuedeTransmitir => Preparativos.Transmision == null;

    public Transmision EnviarMensaje(string tipoMensaje, string tipoCifrado, string contenido)
    {
        ExigirDesbloqueo(SubsistemaEstacion.Comunicaciones);
        if (!PuedeTransmitir)
        {
            throw new InvalidOperationException("Ya se envió un mensaje para esta misión. El canal se reabre al desplegar.");
        }
        var mensaje = CentroComunicaciones.Crear(tipoMensaje, tipoCifrado, contenido, Preparativos.ClavesRotadas);
        var transmision = Comunicaciones.Enviar(mensaje);
        Preparativos.Transmision = transmision;
        Dominar(Patrones.Bridge);
        return transmision;
    }

    public void AsignarDestacamento(IEspecie especie)
    {
        ExigirDesbloqueo(SubsistemaEstacion.Registro);
        Preparativos.Destacamento = especie;
        Progreso.Destacamento = especie.Nombre;
        Dominar(Patrones.Flyweight);
    }

    public void ReclutarOficial(Tripulante tripulante)
    {
        ExigirDesbloqueo(SubsistemaEstacion.Registro);
        if (tripulante.Rango != RegistroTripulacion.RangoSeguridad)
        {
            throw new InvalidOperationException($"{tripulante.Nombre} no es {RegistroTripulacion.RangoSeguridad}.");
        }
        Preparativos.OficialSeguridad = tripulante;
        Progreso.OficialId = tripulante.Identificador;
    }

    public IReadOnlyList<Usuario> OperadoresDisponibles
    {
        get
        {
            var operadores = Escuadron.Titulares.Select(m => new Usuario(m.Nombre, m.Rol switch
            {
                Rol.Comandante => RolAcceso.Capitan,
                Rol.Soldado => RolAcceso.Soldado,
                Rol.Medico => RolAcceso.Medico,
                _ => RolAcceso.Francotirador
            })).ToList();
            if (Preparativos.OficialSeguridad != null)
            {
                operadores.Add(new Usuario(Preparativos.OficialSeguridad.Nombre, RolAcceso.OficialSeguridad));
            }
            return operadores;
        }
    }

    public string LeerBoveda(Usuario operador, string archivo)
    {
        ExigirDesbloqueo(SubsistemaEstacion.Boveda);
        string contenido = OperarBoveda(operador, () => Boveda.LeerArchivo(archivo));
        if (archivo == BovedaDatos.ArchivoIntel)
        {
            Preparativos.InteligenciaJefe = true;
            Progreso.InteligenciaJefe = true;
        }
        return contenido;
    }

    public void EscribirBoveda(Usuario operador, string archivo, string contenido)
    {
        ExigirDesbloqueo(SubsistemaEstacion.Boveda);
        OperarBoveda(operador, () =>
        {
            Boveda.EscribirArchivo(archivo, contenido);
            return contenido;
        });
        if (archivo == BovedaDatos.ArchivoClaves)
        {
            Preparativos.ClavesRotadas = true;
            Progreso.ClavesRotadas = true;
        }
    }

    private string OperarBoveda(Usuario operador, Func<string> operacion)
    {
        Boveda.Operador = operador;
        try
        {
            return operacion();
        }
        finally
        {
            Dominar(Patrones.Proxy);
        }
    }

    public Combate IniciarCombate()
    {
        if (!Control.TurnoIniciado)
        {
            throw new InvalidOperationException("Primero hay que iniciar el turno desde el Control Central.");
        }
        Control.ConsumirTurno();
        var mods = Control.Modificadores;
        if (mods.PatronesAplicados.Count > 0)
        {
            Dominar(Patrones.Facade);
        }
        var enemigo = CatalogoAliens.Crear(SectorActual, Aleatorio);
        var combate = new Combate(this, enemigo, mods);
        Preparativos.ConsumirDeUnSoloUso();
        _mejorasDelClon = 0;
        return combate;
    }

    public List<string> RegistrarResultado(ResumenCombate resumen)
    {
        var nuevos = Progreso.RegistrarResultado(resumen);
        foreach (var patron in nuevos)
        {
            PatronDominado?.Invoke(patron);
        }
        return nuevos;
    }

    private void Dominar(string patron)
    {
        if (Progreso.Dominar(patron))
        {
            PatronDominado?.Invoke(patron);
        }
    }

    private void ExigirDesbloqueo(SubsistemaEstacion subsistema)
    {
        if (!Desbloqueado(subsistema))
        {
            throw new InvalidOperationException($"El subsistema {subsistema} todavía está fuera de servicio.");
        }
    }

    private void RestaurarPreparativosGuardados()
    {
        if (Progreso.NaveReceta != null && CatalogoRecetas.Buscar(Progreso.NaveReceta) is { } receta
            && Enum.TryParse<PlanEnsamblaje>(Progreso.NavePlan, out var plan))
        {
            Preparativos.Nave = Ingeniero.Construir(receta, plan);
        }
        if (Progreso.Destacamento != null && FabricaEspecies.Nombres.Contains(Progreso.Destacamento))
        {
            Preparativos.Destacamento = Fabrica.ObtenerEspecie(Progreso.Destacamento);
        }
        if (Progreso.OficialId is int id)
        {
            Preparativos.OficialSeguridad = Registro.Buscar(id);
        }
        Preparativos.InteligenciaJefe = Progreso.InteligenciaJefe;
        Preparativos.ClavesRotadas = Progreso.ClavesRotadas;
    }
}
