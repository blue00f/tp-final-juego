using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Visitor;

namespace Kepler7.Core.Facade;

public sealed class ControlCentral
{
    public const int EscudoEmergencia = 35;
    public const int EstimulanteEmergencia = 20;

    private readonly Escuadron _escuadron;
    private readonly Inventario _inventario;
    private readonly Preparativos _preparativos;

    private readonly SubsistemaEnergia _energia = new();
    private readonly SubsistemaSoporteVital _soporteVital = new();
    private readonly SubsistemaNavegacion _navegacion = new();
    private readonly SubsistemaComunicaciones _comunicaciones = new();
    private readonly SubsistemaSeguridad _seguridad = new();
    private readonly SubsistemaDefensa _defensa;

    internal ControlCentral(Escuadron escuadron, Inventario inventario, Preparativos preparativos, IAleatorio aleatorio)
    {
        _escuadron = escuadron;
        _inventario = inventario;
        _preparativos = preparativos;
        _defensa = new SubsistemaDefensa(aleatorio);
        _defensa.CalibrarBase(escuadron, inventario);
    }

    public bool TurnoIniciado { get; private set; }

    public bool EnEmergencia { get; private set; }

    public ModificadoresMision Modificadores { get; private set; } = new();

    public IReadOnlyList<string> UltimaSecuencia { get; private set; } = Array.Empty<string>();

    public IReadOnlyList<string> IniciarTurno()
    {
        var mods = new ModificadoresMision();
        var pasos = new List<string> { "═══ CONTROL CENTRAL · INICIO DE TURNO ═══" };
        pasos.Add(_energia.Encender());
        pasos.Add(_soporteVital.RestaurarTripulacion(_escuadron));
        Agregar(pasos, _soporteVital.PrepararDestacamento(_preparativos.Destacamento, mods));
        Agregar(pasos, _soporteVital.PrepararClon(_preparativos.ClonReserva, mods));
        pasos.Add(_navegacion.FijarRumbo(_preparativos.Nave, mods));
        pasos.AddRange(_comunicaciones.AbrirCanal(_preparativos.Transmision, mods));
        pasos.Add(_seguridad.SellarBoveda(_preparativos.InteligenciaJefe, mods));
        // Defensa va al final: arma las cadenas con todas las bonificaciones ya sumadas.
        pasos.AddRange(_defensa.CalibrarDefensas(_escuadron, _inventario, mods));
        pasos.Add(mods.PatronesAplicados.Count > 0
            ? $"Turno iniciado. Preparativos aplicados: {string.Join(", ", mods.PatronesAplicados)}."
            : "Turno iniciado. Escuadrón listo para el despliegue.");

        Modificadores = mods;
        TurnoIniciado = true;
        EnEmergencia = false;
        UltimaSecuencia = pasos;
        return pasos;
    }

    public IReadOnlyList<string> EntrarEnEmergencia()
    {
        var capitan = _escuadron.Capitan;
        var pasos = new List<string> { "═══ ¡MODO EMERGENCIA! El Capitán está en peligro ═══" };
        pasos.Add(_energia.RedirigirAEscudos());
        pasos.Add(_defensa.EscudoDeEmergencia(capitan, EscudoEmergencia));
        pasos.Add(_soporteVital.InyectarEstimulante(capitan, EstimulanteEmergencia));
        pasos.Add(_navegacion.ManiobraEvasiva(capitan));
        pasos.Add(_comunicaciones.EnviarSOS());
        pasos.Add(_seguridad.CierreHermetico());

        EnEmergencia = true;
        UltimaSecuencia = pasos;
        return pasos;
    }

    public IReadOnlyList<string> ApagarEstacion()
    {
        var pasos = new List<string> { "═══ CONTROL CENTRAL · APAGADO DE LA ESTACIÓN ═══" };
        pasos.Add(_seguridad.Bloqueo());
        pasos.Add(_comunicaciones.CerrarCanal());
        pasos.Add(_navegacion.Anclar());
        pasos.Add(_defensa.Standby());
        pasos.Add(_soporteVital.ModoMinimo());
        pasos.Add(_energia.Apagar());
        pasos.Add("Kepler-7 en reposo. Progreso guardado.");

        TurnoIniciado = false;
        EnEmergencia = false;
        UltimaSecuencia = pasos;
        return pasos;
    }

    internal void ConsumirTurno() => TurnoIniciado = false;

    private static void Agregar(List<string> pasos, string? linea)
    {
        if (linea != null)
        {
            pasos.Add(linea);
        }
    }
}
