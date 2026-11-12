using Kepler7.Core.ChainOfResponsibility;
using Kepler7.Core.Command;
using Kepler7.Core.Comun;
using Kepler7.Core.Facade;
using Kepler7.Core.Modelo;
using Kepler7.Core.Progreso;
using Kepler7.Core.State;
using Kepler7.Core.Strategy;
using Kepler7.Core.TemplateMethod;
using Kepler7.Core.Visitor;
using ArmaduraEslabon = Kepler7.Core.ChainOfResponsibility.Armadura;

namespace Kepler7.Core.Juego;

public sealed class Combate
{
    private readonly Partida _partida;
    private readonly Contexto _contexto;
    private readonly Dictionary<Rol, TurnoUnidad> _turnos;
    private readonly TurnoAlien _turnoAlien = new();
    private readonly List<Enemigo> _enemigos;
    private bool _emergenciaUsada;
    private bool _estrategiaCambiadaPorJugador;
    private bool _adaptacionRealizada;
    private readonly HashSet<Rol> _rolesQueCompletaronTurno = new();
    private readonly int _sector;
    private MiembroEscuadron? _reserva;
    private bool _frenesiJefe;

    internal Combate(Partida partida, Enemigo enemigo, ModificadoresMision modificadores)
    {
        _partida = partida;
        _sector = partida.SectorActual;
        _enemigos = new List<Enemigo> { enemigo };
        Registro = new Registro();
        _contexto = new Contexto(partida.Escuadron, _enemigos, partida.Inventario, partida.Aleatorio, Registro)
        {
            Modificadores = modificadores
        };
        _reserva = modificadores.ClonReserva;

        _turnos = new Dictionary<Rol, TurnoUnidad>
        {
            [Rol.Comandante] = new TurnoComandante(),
            [Rol.Soldado] = new TurnoSoldado(),
            [Rol.Medico] = new TurnoMedico(),
            [Rol.Francotirador] = new TurnoFrancotirador()
        };

        foreach (var linea in partida.Control.UltimaSecuencia)
        {
            _contexto.Registrar(CategoriaRegistro.Estacion, linea);
        }
        _contexto.Registrar(CategoriaRegistro.Sistema, $"⚠ Contacto: {enemigo.Nombre}. {enemigo.Descripcion}");
        _contexto.Registrar(CategoriaRegistro.IA, $"{enemigo.Nombre} usa {enemigo.Estrategia.Nombre}: {enemigo.Estrategia.Descripcion}");
        AplicarPreparativosAlEnemigo(enemigo, modificadores);
    }

    private void AplicarPreparativosAlEnemigo(Enemigo enemigo, ModificadoresMision mods)
    {
        if (mods.DanioInicialAlien > 0)
        {
            int perdida = (int)Math.Round(enemigo.VidaMaxima * mods.DanioInicialAlien);
            enemigo.PerderVida(perdida);
            _contexto.Registrar(CategoriaRegistro.Sistema, $"☄ Bombardeo orbital de la Tierra: {enemigo.Nombre} pierde {perdida} de vida.");
        }
        if (mods.DebilidadJefe && enemigo.EsJefe)
        {
            foreach (var eslabon in enemigo.Defensa.Eslabones.Where(e => e is ArmaduraEslabon or Escudo).ToList())
            {
                enemigo.Defensa.Quitar(eslabon);
            }
            _contexto.Registrar(CategoriaRegistro.Danio,
                $"◈ Inteligencia de la bóveda: grieta en el caparazón. Cadena del jefe: {enemigo.Defensa.Describir()}");
        }
        if (mods.AlienAlertado)
        {
            _contexto.AplicarEstado(enemigo, new Berserk(2), "interceptó la transmisión y se lanzó al ataque");
        }
        if (_reserva != null)
        {
            _contexto.Registrar(CategoriaRegistro.Sistema, $"❄ {_reserva.Nombre} espera en criogenia para relevar al primer caído.");
        }
    }

    public MiembroEscuadron? Reserva => _reserva;

    public Escuadron Escuadron => _partida.Escuadron;
    public IReadOnlyList<Enemigo> Enemigos => _enemigos;
    public Enemigo EnemigoPrincipal => _enemigos[0];
    public Inventario Inventario => _partida.Inventario;
    public ControlCentral Control => _partida.Control;

    public GestorComandos Comandos { get; } = new();
    public Registro Registro { get; }

    public int Ronda => _contexto.Ronda;
    public FaseCombate Fase { get; private set; } = FaseCombate.Jugador;
    public ResultadoCombate Resultado { get; private set; } = ResultadoCombate.EnCurso;

    public IReadOnlyList<Danio> HistorialDanios => _contexto.Danios;
    public Danio? UltimoDanio => _contexto.Danios.Count > 0 ? _contexto.Danios[^1] : null;

    public IReadOnlyCollection<string> PatronesObservados => _contexto.PatronesObservados;

    public bool PuedeActuar(MiembroEscuadron miembro) =>
        Resultado == ResultadoCombate.EnCurso && Fase == FaseCombate.Jugador && miembro.EstaVivo && !miembro.YaActuo;

    public IReadOnlyList<TipoAccion> AccionesDisponibles(MiembroEscuadron miembro)
    {
        IReadOnlyList<TipoAccion> acciones = PuedeActuar(miembro)
            ? miembro.AccionesDelRol.Where(a => miembro.Estado.Permite(a)).ToList()
            : Array.Empty<TipoAccion>();
        return acciones;
    }

    public string? Ordenar(MiembroEscuadron actor, TipoAccion tipo, Personaje? objetivo = null)
    {
        string? error = null;
        ComandoBase? comando = null;

        if (!PuedeActuar(actor))
        {
            error = $"{actor.Nombre} no puede actuar ahora.";
        }
        else if (!AccionesDisponibles(actor).Contains(tipo))
        {
            error = $"{actor.Nombre} no puede {tipo} (rol {actor.Rol}, estado {actor.Estado.Nombre}).";
        }
        else
        {
            comando = tipo switch
            {
                TipoAccion.Atacar => new AtacarComando(this, actor, objetivo as Enemigo ?? EnemigoPrincipal),
                TipoAccion.Mover => new MoverComando(this, actor),
                TipoAccion.Curar when objetivo is MiembroEscuadron paciente => new CurarComando(this, actor, paciente),
                TipoAccion.Construir when objetivo is MiembroEscuadron aliado => new ConstruirComando(this, actor, aliado),
                _ => null
            };
            error = comando == null ? "Esa acción necesita un aliado como objetivo." : comando.Validar();
        }

        if (error == null && comando != null)
        {
            Comandos.Ejecutar(comando);
            _contexto.Registrar(CategoriaRegistro.Comando, $"▶ {comando.Descripcion}");
        }
        return error;
    }

    public bool Deshacer()
    {
        bool hecho = false;
        if (Resultado == ResultadoCombate.EnCurso && Fase == FaseCombate.Jugador)
        {
            var comando = Comandos.Deshacer();
            if (comando != null)
            {
                hecho = true;
                _contexto.Observar(Patrones.Command);
                _contexto.Registrar(CategoriaRegistro.Comando, $"↩ Deshecho: {comando.Descripcion}");
            }
        }
        return hecho;
    }

    public bool Rehacer()
    {
        bool hecho = false;
        if (Resultado == ResultadoCombate.EnCurso && Fase == FaseCombate.Jugador)
        {
            var comando = Comandos.Rehacer();
            if (comando != null)
            {
                hecho = true;
                _contexto.Registrar(CategoriaRegistro.Comando, $"↪ Rehecho: {comando.Descripcion}");
            }
        }
        return hecho;
    }

    public void TerminarTurnoJugador()
    {
        if (Resultado == ResultadoCombate.EnCurso && Fase == FaseCombate.Jugador)
        {
            foreach (var miembro in Escuadron.Miembros.Where(m => m.EstaVivo && !m.YaActuo).ToList())
            {
                EjecutarTurnoMiembro(miembro, new AccionTurno(TipoAccion.Esperar));
            }
            Comandos.ConfirmarTurno(Ronda);
            _contexto.Registrar(CategoriaRegistro.Comando, $"✔ Turno {Ronda} confirmado: ya no se puede deshacer.");

            if (Resultado == ResultadoCombate.EnCurso)
            {
                Fase = FaseCombate.Enemigo;
            }
        }
    }

    public void EjecutarTurnoEnemigo()
    {
        if (Resultado == ResultadoCombate.EnCurso && Fase == FaseCombate.Enemigo)
        {
            foreach (var enemigo in _enemigos.Where(e => e.EstaVivo).ToList())
            {
                _turnoAlien.EjecutarTurno(enemigo, _contexto);
                Verificar();
                if (Resultado != ResultadoCombate.EnCurso)
                {
                    break;
                }
            }

            if (Resultado == ResultadoCombate.EnCurso)
            {
                NuevaRonda();
            }
        }
    }

    public void CambiarEstrategia(Enemigo enemigo, IEstrategiaIA estrategia)
    {
        if (enemigo.Estrategia.GetType() != estrategia.GetType())
        {
            enemigo.CambiarEstrategia(estrategia);
            _estrategiaCambiadaPorJugador = true;
            _contexto.Observar(Patrones.Strategy);
            _contexto.Registrar(CategoriaRegistro.IA, $"⇄ Interferencia neural: {enemigo.Nombre} pasa a {estrategia.Nombre}. {estrategia.Descripcion}");
        }
    }

    public string AplicarVisitante(IVisitor visitante)
    {
        string resultado = Inventario.Aplicar(visitante);
        _contexto.Observar(Patrones.Visitor);
        _contexto.Registrar(CategoriaRegistro.Sistema, $"» {visitante.Nombre}: {resultado}");
        return resultado;
    }

    public List<EventoCombate> ConsumirEventos() => _contexto.ConsumirEventos();

    public ResumenCombate GenerarResumen()
    {
        int xpBase = Resultado == ResultadoCombate.Victoria
            ? 100 + 20 * _sector + Math.Max(0, 10 * (12 - Ronda)) + (EnemigoPrincipal.EsJefe ? 150 : 0)
            : 30 + _contexto.Estadisticas.DanioInfligido / 10;
        int xp = (int)Math.Round(xpBase * _contexto.Modificadores.MultiplicadorXp);
        int bajas = Escuadron.Titulares.Count(m => !Escuadron.Miembros.Contains(m) || !m.EstaVivo);
        return new ResumenCombate(Resultado, _sector, EnemigoPrincipal.EsJefe, EnemigoPrincipal.Nombre, Ronda,
            _contexto.Estadisticas.DanioInfligido, _contexto.Estadisticas.DanioRecibido, _contexto.Estadisticas.CuracionTotal,
            Comandos.Ejecutados, Comandos.Deshechos, bajas, xp, _contexto.PatronesObservados.ToList());
    }

    internal void EjecutarTurnoMiembro(MiembroEscuadron miembro, AccionTurno accion)
    {
        _contexto.AccionSolicitada = accion;
        _turnos[miembro.Rol].EjecutarTurno(miembro, _contexto);
        _contexto.AccionSolicitada = null;

        _rolesQueCompletaronTurno.Add(miembro.Rol);
        if (_rolesQueCompletaronTurno.Count == _turnos.Count)
        {
            _contexto.Observar(Patrones.TemplateMethod);
        }
        Verificar();
    }

    private void Verificar()
    {
        if (!_enemigos.Any(e => e.EstaVivo))
        {
            Resultado = ResultadoCombate.Victoria;
            Fase = FaseCombate.Terminado;
            _contexto.Registrar(CategoriaRegistro.Sistema, "★ ¡VICTORIA! El alien fue neutralizado.");
        }
        else if (!Escuadron.Capitan.EstaVivo)
        {
            Resultado = ResultadoCombate.Derrota;
            Fase = FaseCombate.Terminado;
            _contexto.Registrar(CategoriaRegistro.Sistema, "✖ DERROTA. El Capitán ha caído.");
        }
        else
        {
            VerificarEmergencia();
            VerificarAdaptacionEnemiga();
        }
    }

    private void VerificarEmergencia()
    {
        var capitan = Escuadron.Capitan;
        if (!_emergenciaUsada && capitan.PorcentajeVida < 0.30)
        {
            _emergenciaUsada = true;
            foreach (var linea in Control.EntrarEnEmergencia())
            {
                _contexto.Registrar(CategoriaRegistro.Estacion, linea);
            }
            _contexto.AgregarEvento(TipoEvento.Emergencia, capitan, 0, "EMERGENCIA");
            _contexto.Observar(Patrones.Facade);
        }
    }

    private void VerificarAdaptacionEnemiga()
    {
        var enemigo = EnemigoPrincipal;
        bool herido = enemigo.PorcentajeVida < 0.5;
        if (!_adaptacionRealizada && !_estrategiaCambiadaPorJugador && herido)
        {
            _adaptacionRealizada = true;
            if (enemigo.Estrategia is not IACalculadora)
            {
                enemigo.CambiarEstrategia(new IACalculadora());
                _contexto.Registrar(CategoriaRegistro.IA, $"⇄ {enemigo.Nombre} está herido y se adapta: pasa a IA Calculadora.");
            }
        }
        if (enemigo.EsJefe && herido && !_frenesiJefe && enemigo.EstaVivo)
        {
            _frenesiJefe = true;
            _contexto.AplicarEstado(enemigo, new Berserk(3), "¡el Leviatán entra en frenesí!");
        }
    }

    private void ActivarReserva()
    {
        var caido = Escuadron.Miembros.FirstOrDefault(m => !m.EstaVivo && !m.EsCapitan);
        if (_reserva != null && caido != null)
        {
            var clon = _reserva;
            _reserva = null;
            var mods = _contexto.Modificadores;
            clon.AtaqueBase = (int)Math.Round(clon.AtaqueNatural * mods.MultiplicadorAtaque);
            if (mods.EscudoInicial > 0)
            {
                clon.Defensa.Agregar(new Escudo(mods.EscudoInicial));
            }
            clon.Posicion = caido.Posicion;
            Escuadron.Reemplazar(caido, clon);
            _contexto.Registrar(CategoriaRegistro.Sistema,
                $"❄ {clon.Nombre} sale de la criogenia y ocupa el lugar de {caido.Nombre}. Cadena: {clon.Defensa.Describir()}");
            _contexto.AgregarEvento(TipoEvento.Refuerzo, clon, 0, "¡REFUERZO!");
            _contexto.Observar(Patrones.Prototype);
        }
    }

    private void NuevaRonda()
    {
        ActivarReserva();
        _contexto.Ronda++;
        foreach (var miembro in Escuadron.Miembros)
        {
            miembro.YaActuo = false;
        }
        Fase = FaseCombate.Jugador;
        _contexto.Registrar(CategoriaRegistro.Sistema, $"── Ronda {Ronda} ──");
    }

    internal FotoCombate Fotografiar()
    {
        var personajes = Escuadron.Miembros.Cast<Personaje>().Concat(_enemigos).Select(p => p.Fotografiar()).ToList();
        return new FotoCombate(personajes, Escuadron.Miembros.ToList(), _contexto.Estadisticas.Copiar(), Resultado, Fase,
            _emergenciaUsada, _adaptacionRealizada, _frenesiJefe, _reserva, EnemigoPrincipal.Estrategia);
    }

    internal void Restaurar(FotoCombate foto)
    {
        Escuadron.RestaurarFormacion(foto.Formacion);
        _reserva = foto.Reserva;
        _frenesiJefe = foto.FrenesiJefe;
        foreach (var fotoPersonaje in foto.Personajes)
        {
            fotoPersonaje.Personaje.Restaurar(fotoPersonaje);
        }
        _contexto.Estadisticas = foto.Estadisticas.Copiar();
        Resultado = foto.Resultado;
        Fase = foto.Fase;
        _emergenciaUsada = foto.EmergenciaUsada;
        _adaptacionRealizada = foto.AdaptacionRealizada;
        EnemigoPrincipal.CambiarEstrategia(foto.Estrategia);
    }
}
