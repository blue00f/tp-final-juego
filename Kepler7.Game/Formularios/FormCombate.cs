using Kepler7.Core.Comun;
using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.Progreso;
using Kepler7.Core.Strategy;
using Kepler7.Core.Visitor;
using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormCombate : FormBase
{
    private readonly Partida _partida;
    private readonly RepositorioProgreso? _repositorio;
    private readonly Combate _combate;

    private readonly Dictionary<Personaje, TarjetaBase> _tarjetas = new();
    private readonly TarjetaAlien _tarjetaAlien;
    private readonly PanelCampo _campo = new();
    private readonly PanelCadena _panelCadena = new();

    private readonly Label _lblSector = new();
    private readonly Label _lblRonda = new();
    private readonly Label _lblEmergencia = new();
    private readonly Label _lblReserva = new();

    private readonly Label _lblSeleccion = new();
    private readonly Label _lblAyuda = new();
    private readonly Boton _btnAtacar = new("Atacar", 100);
    private readonly Boton _btnCurar = new("Curar", 100);
    private readonly Boton _btnMover = new("Mover", 100);
    private readonly Boton _btnConstruir = new("Construir", 100);
    private readonly Boton _btnDeshacer = new("Deshacer", 100);
    private readonly Boton _btnRehacer = new("Rehacer", 100);
    private readonly Boton _btnFinTurno = new("Fin de turno", 116, 32);

    private readonly ComboBox _cmbEstrategia = new();
    private readonly Label _lblDecision = new();
    private readonly List<Func<IEstrategiaIA>> _fabricasEstrategia = CatalogoEstrategias.Todas.ToList();

    private readonly TabControl _pestanias = new();
    private readonly ListaRegistro _lstBitacora = new();
    private readonly ListaRegistro _lstTurnos = new();
    private readonly ListaRegistro _lstDanio = new();
    private readonly ListaRegistro _lstComandos = new();
    private readonly ListaRegistro _lstControl = new();
    private readonly ListaRegistro _lstIA = new();
    private TabPage _tabControl = null!;

    private readonly ListaRegistro _lstArsenal = new();
    private readonly ComboBox _cmbVisitante = new();
    private readonly Label _lblResultadoVisitante = new();
    private readonly List<Func<IVisitor>> _fabricasVisitante = CatalogoVisitantes.Todos.ToList();

    private readonly Label _lblDefensaDe = new();
    private readonly ListBox _lstEslabones = new();
    private readonly Boton _btnSubir = new("Subir", 100, 30);
    private readonly Boton _btnBajar = new("Bajar", 100, 30);

    private readonly System.Windows.Forms.Timer _timerEmergencia = new() { Interval = 250 };

    private MiembroEscuadron? _seleccionado;
    private TipoAccion? _accionPendiente;
    private Personaje _enfocado;
    private bool _ocupado;
    private bool _terminado;
    private bool _actualizandoCombo;
    private int _indiceRegistro;
    private int _parpadeos;

    public FormCombate(Partida partida, RepositorioProgreso? repositorio)
    {
        _partida = partida;
        _repositorio = repositorio;
        _combate = partida.IniciarCombate();
        _enfocado = _combate.Escuadron.Capitan;
        _tarjetaAlien = new TarjetaAlien(_combate.EnemigoPrincipal);

        Text = "Kepler-7 · Combate";
        ClientSize = new Size(1360, 830);
        KeyPreview = true;

        ConstruirBarraSuperior();
        ConstruirCampo();
        ConstruirAcciones();
        ConstruirPestanias();

        _panelCadena.SetBounds(12, 666, 820, 152);
        Controls.Add(_panelCadena);

        _timerEmergencia.Tick += (_, _) => Parpadear();

        RefrescarTodo();
        _lblAyuda.Text = "Elegí un tripulante haciendo clic en su tarjeta. Atajos: Ctrl+Z deshacer · Ctrl+Y rehacer.";
    }

    private void ConstruirBarraSuperior()
    {
        var barra = new Panel { Location = new Point(0, 0), Size = new Size(1360, 56) };

        Configurar(_lblSector, $"Sector {_partida.SectorActual}: {_combate.EnemigoPrincipal.Nombre}", 13f, Tema.Texto, FontStyle.Bold);
        _lblSector.Location = new Point(16, 9);

        Configurar(_lblRonda, "", 11f, Tema.Texto, FontStyle.Bold);
        _lblRonda.AutoSize = false;
        _lblRonda.TextAlign = ContentAlignment.MiddleCenter;
        _lblRonda.SetBounds(430, 12, 500, 30);

        Configurar(_lblReserva, "", 8.5f, Tema.TextoTenue);
        _lblReserva.Location = new Point(18, 38);

        Configurar(_lblEmergencia, "Modo emergencia", 11f, Tema.Peligro, FontStyle.Bold);
        _lblEmergencia.Location = new Point(1010, 16);
        _lblEmergencia.Visible = false;

        var btnAyuda = new Boton("Ayuda", 80, 30) { Location = new Point(1268, 13) };
        btnAyuda.Click += (_, _) => new FormTutorial().ShowDialog(this);

        barra.Controls.AddRange(new Control[] { _lblSector, _lblReserva, _lblRonda, _lblEmergencia, btnAyuda });
        Controls.Add(barra);
    }

    private void ConstruirCampo()
    {
        _campo.SetBounds(12, 66, 820, 470);
        Controls.Add(_campo);

        foreach (var miembro in _combate.Escuadron.Miembros)
        {
            CrearTarjeta(miembro);
        }

        _tarjetaAlien.Location = new Point(510, 30);
        _tarjetaAlien.Click += (_, _) => Enfocar(_combate.EnemigoPrincipal);
        _tarjetas[_combate.EnemigoPrincipal] = _tarjetaAlien;
        _campo.Controls.Add(_tarjetaAlien);

        var lblInterferencia = new Label();
        Configurar(lblInterferencia, "Estrategia del alien (interferencia neural)", 9f, Tema.Texto, FontStyle.Bold);
        lblInterferencia.Location = new Point(510, 340);

        _cmbEstrategia.SetBounds(510, 362, 280, 26);
        _cmbEstrategia.DropDownStyle = ComboBoxStyle.DropDownList;
        foreach (var fabrica in _fabricasEstrategia)
        {
            _cmbEstrategia.Items.Add(fabrica().Nombre);
        }
        _cmbEstrategia.SelectedIndexChanged += (_, _) => AlCambiarEstrategia();

        Configurar(_lblDecision, "", 8.5f, Tema.Texto);
        _lblDecision.AutoSize = false;
        _lblDecision.SetBounds(510, 394, 290, 70);

        _campo.Controls.AddRange(new Control[] { lblInterferencia, _cmbEstrategia, _lblDecision });
    }

    private void ConstruirAcciones()
    {
        var panel = new Panel { Location = new Point(12, 546), Size = new Size(820, 112), BorderStyle = BorderStyle.FixedSingle };

        Configurar(_lblSeleccion, "", 9.5f, Tema.Texto, FontStyle.Bold);
        _lblSeleccion.Location = new Point(12, 8);

        _btnAtacar.Location = new Point(12, 34);
        _btnCurar.Location = new Point(124, 34);
        _btnMover.Location = new Point(236, 34);
        _btnConstruir.Location = new Point(348, 34);
        _btnDeshacer.Location = new Point(476, 34);
        _btnRehacer.Location = new Point(582, 34);
        _btnFinTurno.Location = new Point(692, 34);
        _btnFinTurno.Font = Tema.Crear(9f, FontStyle.Bold);

        _btnAtacar.Click += (_, _) => Ordenar(TipoAccion.Atacar, _combate.EnemigoPrincipal);
        _btnMover.Click += (_, _) => Ordenar(TipoAccion.Mover, null);
        _btnCurar.Click += (_, _) => PrepararAccionConObjetivo(TipoAccion.Curar);
        _btnConstruir.Click += (_, _) => PrepararAccionConObjetivo(TipoAccion.Construir);
        _btnDeshacer.Click += (_, _) => Deshacer();
        _btnRehacer.Click += (_, _) => Rehacer();
        _btnFinTurno.Click += async (_, _) => await TerminarTurnoAsync();

        Configurar(_lblAyuda, "", 9f, Tema.TextoTenue);
        _lblAyuda.AutoSize = false;
        _lblAyuda.SetBounds(12, 84, 796, 22);

        panel.Controls.AddRange(new Control[]
        {
            _lblSeleccion, _btnAtacar, _btnCurar, _btnMover, _btnConstruir, _btnDeshacer, _btnRehacer, _btnFinTurno, _lblAyuda
        });
        Controls.Add(panel);
    }

    private void ConstruirPestanias()
    {
        // El borde del TabControl es claro y no se puede cambiar: se lo recorta dentro de un panel oscuro.
        var marco = new Panel { Bounds = new Rectangle(844, 66, 504, 752) };
        _pestanias.SetBounds(-4, 0, 512, 756);
        _pestanias.SizeMode = TabSizeMode.Fixed;
        _pestanias.ItemSize = new Size(61, 24);
        _pestanias.DrawMode = TabDrawMode.OwnerDrawFixed;
        _pestanias.DrawItem += DibujarPestania;

        AgregarPestania("Bitácora", _lstBitacora, "Eventos generales del combate.");
        AgregarPestania("Turnos", _lstTurnos, "Template Method: cada turno sigue Inicio → Decisión → Acción → Final.");
        AgregarPestania("Daño", _lstDanio, "Chain of Responsibility: recorrido de cada golpe por las capas de defensa.");
        AgregarPestania("Órdenes", _lstComandos, "Command: historial de órdenes ejecutadas, deshechas y rehechas.");
        _tabControl = AgregarPestania("Control", _lstControl, "Facade: secuencias coordinadas por el Control Central.");
        AgregarPestania("IA", _lstIA, "Strategy: decisiones del alien y por qué las toma.");
        _pestanias.TabPages.Add(CrearPestaniaArsenal());
        _pestanias.TabPages.Add(CrearPestaniaDefensas());

        marco.Controls.Add(_pestanias);
        Controls.Add(marco);
    }

    private TabPage AgregarPestania(string titulo, ListaRegistro lista, string descripcion)
    {
        var pagina = new TabPage(titulo) { BackColor = Tema.Panel };
        var encabezado = new Label();
        Configurar(encabezado, descripcion, 9f, Tema.TextoTenue);
        encabezado.AutoSize = false;
        encabezado.SetBounds(6, 6, 484, 36);
        lista.SetBounds(4, 46, 488, 666);
        pagina.Controls.AddRange(new Control[] { encabezado, lista });
        _pestanias.TabPages.Add(pagina);
        return pagina;
    }

    private TabPage CrearPestaniaArsenal()
    {
        var pagina = new TabPage("Arsenal") { BackColor = Tema.Panel };

        var titulo = new Label();
        Configurar(titulo, "Visitor: el analizador recorre los ítems sin modificar sus clases.", 9f, Tema.TextoTenue);
        titulo.AutoSize = false;
        titulo.SetBounds(6, 6, 484, 36);

        _lstArsenal.SetBounds(4, 46, 488, 300);

        var lblAnalizador = new Label();
        Configurar(lblAnalizador, "Analizador", 9f, Tema.Texto, FontStyle.Bold);
        lblAnalizador.Location = new Point(8, 360);

        _cmbVisitante.SetBounds(8, 384, 330, 26);
        _cmbVisitante.DropDownStyle = ComboBoxStyle.DropDownList;
        foreach (var fabrica in _fabricasVisitante)
        {
            _cmbVisitante.Items.Add(fabrica().Nombre);
        }
        _cmbVisitante.SelectedIndexChanged += (_, _) =>
            _lblResultadoVisitante.Text = _fabricasVisitante[_cmbVisitante.SelectedIndex]().Descripcion;
        _cmbVisitante.SelectedIndex = 0;

        var btnAplicar = new Boton("Aplicar", 110, 28) { Location = new Point(350, 383) };
        btnAplicar.Click += (_, _) => AplicarVisitante();

        Configurar(_lblResultadoVisitante, "Elegí un analizador y aplicalo al arsenal.", 9.5f, Tema.Texto);
        _lblResultadoVisitante.AutoSize = false;
        _lblResultadoVisitante.SetBounds(8, 426, 480, 90);

        pagina.Controls.AddRange(new Control[] { titulo, _lstArsenal, lblAnalizador, _cmbVisitante, btnAplicar, _lblResultadoVisitante });
        return pagina;
    }

    private TabPage CrearPestaniaDefensas()
    {
        var pagina = new TabPage("Defensas") { BackColor = Tema.Panel };

        var titulo = new Label();
        Configurar(titulo, "Chain of Responsibility: el daño entra por arriba y baja eslabón por eslabón.", 9f, Tema.TextoTenue);
        titulo.AutoSize = false;
        titulo.SetBounds(6, 6, 484, 36);

        Configurar(_lblDefensaDe, "", 10f, Tema.Texto, FontStyle.Bold);
        _lblDefensaDe.Location = new Point(8, 50);

        _lstEslabones.SetBounds(8, 82, 330, 200);
        _lstEslabones.Font = Tema.Crear(10f);
        _lstEslabones.SelectedIndexChanged += (_, _) => ActualizarBotonesDefensa();

        _btnSubir.Location = new Point(350, 82);
        _btnBajar.Location = new Point(350, 118);
        _btnSubir.Click += (_, _) => MoverEslabon(-1);
        _btnBajar.Click += (_, _) => MoverEslabon(+1);

        var explicacion = new Label();
        Configurar(explicacion,
            "Hacé clic en cualquier tarjeta para ver su cadena. Podés reordenar los eslabones de tu escuadrón " +
            "en cualquier momento: el orden cambia el resultado (por ejemplo, la armadura reduce un porcentaje y el " +
            "bloqueo un valor fijo).\n\nLos eslabones también se agregan y se quitan en plena partida: el Capitán " +
            "agrega Escudos con Construir, el alien endurece su caparazón, y un escudo agotado se retira solo. " +
            "Vida es siempre el último eslabón.", 9f, Tema.TextoTenue);
        explicacion.AutoSize = false;
        explicacion.SetBounds(8, 296, 480, 200);

        pagina.Controls.AddRange(new Control[] { titulo, _lblDefensaDe, _lstEslabones, _btnSubir, _btnBajar, explicacion });
        return pagina;
    }

    private void DibujarPestania(object? sender, DrawItemEventArgs e)
    {
        bool activa = e.Index == _pestanias.SelectedIndex;
        using (var fondo = new SolidBrush(activa ? Tema.Panel : Tema.Fondo))
        {
            e.Graphics.FillRectangle(fondo, e.Bounds);
        }
        TextRenderer.DrawText(e.Graphics, _pestanias.TabPages[e.Index].Text, _pestanias.Font, e.Bounds,
            activa ? Tema.Texto : Tema.TextoTenue, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    private static void Configurar(Label etiqueta, string texto, float tamanio, Color color, FontStyle estilo = FontStyle.Regular)
    {
        etiqueta.Text = texto;
        etiqueta.AutoSize = true;
        etiqueta.ForeColor = color;
        etiqueta.BackColor = Color.Transparent;
        etiqueta.Font = Tema.Crear(tamanio, estilo);
    }

    private void AlHacerClicEnTripulante(MiembroEscuadron miembro)
    {
        Enfocar(miembro);
        if (_accionPendiente.HasValue && _seleccionado != null)
        {
            var accion = _accionPendiente.Value;
            _accionPendiente = null;
            Ordenar(accion, miembro);
        }
        else if (!_ocupado && _combate.PuedeActuar(miembro))
        {
            _seleccionado = miembro;
            _lblAyuda.Text = $"{miembro.Nombre}: elegí una acción.";
            ActualizarControles();
        }
        else
        {
            string motivo = !miembro.EstaVivo ? "está caído"
                : miembro.YaActuo ? "ya actuó en esta ronda"
                : "no puede actuar ahora";
            _lblAyuda.Text = $"{miembro.Nombre} {motivo}.";
        }
    }

    private void PrepararAccionConObjetivo(TipoAccion accion)
    {
        if (_accionPendiente == accion)
        {
            _accionPendiente = null;
            _lblAyuda.Text = "Acción cancelada.";
        }
        else
        {
            _accionPendiente = accion;
            _lblAyuda.Text = accion == TipoAccion.Curar
                ? "Elegí a qué tripulante curar (clic en su tarjeta)."
                : "Elegí sobre qué tripulante desplegar el generador de escudo.";
        }
        ActualizarControles();
    }

    private void Ordenar(TipoAccion accion, Personaje? objetivo)
    {
        if (_seleccionado != null && !_ocupado)
        {
            string? error = _combate.Ordenar(_seleccionado, accion, objetivo);
            if (error != null)
            {
                _lblAyuda.Text = error;
            }
            else
            {
                if (accion == TipoAccion.Atacar)
                {
                    _tarjetas[_seleccionado].Destellar(Tema.Acento);
                }
                _lblAyuda.Text = $"Orden ejecutada. Podés deshacerla hasta confirmar el turno.";
                _seleccionado = null;
            }
            _accionPendiente = null;
            RefrescarTodo();
            _ = TerminarSiCorrespondeAsync();
        }
    }

    private void Deshacer()
    {
        if (!_ocupado && _combate.Deshacer())
        {
            _seleccionado = null;
            _accionPendiente = null;
            _lblAyuda.Text = "Orden deshecha: el combate volvió exactamente al estado anterior.";
            RefrescarTodo();
        }
    }

    private void Rehacer()
    {
        if (!_ocupado && _combate.Rehacer())
        {
            _seleccionado = null;
            _accionPendiente = null;
            _lblAyuda.Text = "Orden rehecha con el mismo resultado (no se vuelven a tirar los dados).";
            RefrescarTodo();
        }
    }

    private async Task TerminarTurnoAsync()
    {
        if (!_ocupado && _combate.Fase == FaseCombate.Jugador)
        {
            _ocupado = true;
            _seleccionado = null;
            _accionPendiente = null;

            _combate.TerminarTurnoJugador();
            RefrescarTodo();
            bool termino = await TerminarSiCorrespondeAsync();

            if (!termino)
            {
                _lblRonda.Text = "Turno del alien...";
                _lblRonda.ForeColor = Tema.Texto;
                await Task.Delay(650);
                _tarjetaAlien.Embestir();
                await Task.Delay(250);

                _combate.EjecutarTurnoEnemigo();
                RefrescarTodo();
                termino = await TerminarSiCorrespondeAsync();
            }

            if (!termino)
            {
                _ocupado = false;
                _lblAyuda.Text = "Tu turno. Elegí un tripulante.";
                ActualizarControles();
            }
        }
    }

    private void AlCambiarEstrategia()
    {
        if (!_actualizandoCombo && _cmbEstrategia.SelectedIndex >= 0)
        {
            var estrategia = _fabricasEstrategia[_cmbEstrategia.SelectedIndex]();
            _combate.CambiarEstrategia(_combate.EnemigoPrincipal, estrategia);
            _lblAyuda.Text = $"Interferencia neural: el alien ahora usa {estrategia.Nombre}.";
            RefrescarTodo();
        }
    }

    private void AplicarVisitante()
    {
        var visitante = _fabricasVisitante[_cmbVisitante.SelectedIndex]();
        _lblResultadoVisitante.Text = _combate.AplicarVisitante(visitante);
        RefrescarTodo();
    }

    private void Enfocar(Personaje personaje)
    {
        _enfocado = personaje;
        ActualizarDefensas();
    }

    private void MoverEslabon(int desplazamiento)
    {
        int indice = _lstEslabones.SelectedIndex;
        bool esPropio = _enfocado is MiembroEscuadron;
        if (esPropio && !_ocupado && _enfocado.Defensa.Mover(indice, desplazamiento))
        {
            ActualizarDefensas();
            _lstEslabones.SelectedIndex = indice + desplazamiento;
            _lblAyuda.Text = $"Cadena de {_enfocado.Nombre}: {_enfocado.Defensa.Describir()}";
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        bool manejado = true;
        if (keyData == (Keys.Control | Keys.Z))
        {
            Deshacer();
        }
        else if (keyData == (Keys.Control | Keys.Y))
        {
            Rehacer();
        }
        else
        {
            manejado = base.ProcessCmdKey(ref msg, keyData);
        }
        return manejado;
    }

    private void RefrescarTodo()
    {
        UbicarTarjetas();
        foreach (var tarjeta in _tarjetas.Values)
        {
            tarjeta.Invalidate();
        }
        ActualizarRegistros();
        ProcesarEventos();
        _panelCadena.Mostrar(_combate.UltimoDanio);
        ActualizarDefensas();
        ActualizarArsenal();
        ActualizarIA();
        ActualizarControles();
    }

    private void CrearTarjeta(MiembroEscuadron miembro)
    {
        var tarjeta = new TarjetaTripulante(miembro);
        tarjeta.Click += (_, _) => AlHacerClicEnTripulante(miembro);
        _tarjetas[miembro] = tarjeta;
        _campo.Controls.Add(tarjeta);
    }

    private void SincronizarTarjetas()
    {
        var formacion = _combate.Escuadron.Miembros;
        foreach (var personaje in _tarjetas.Keys.OfType<MiembroEscuadron>().Where(m => !formacion.Contains(m)).ToList())
        {
            var tarjeta = _tarjetas[personaje];
            _tarjetas.Remove(personaje);
            _campo.Controls.Remove(tarjeta);
            tarjeta.Dispose();
        }
        foreach (var miembro in formacion.Where(m => !_tarjetas.ContainsKey(m)))
        {
            CrearTarjeta(miembro);
        }
        _lblReserva.Text = _combate.Reserva == null ? "" : $"En reserva (criogenia): {_combate.Reserva.Nombre} (releva al primer caído)";
    }

    private void UbicarTarjetas()
    {
        SincronizarTarjetas();
        var miembros = _combate.Escuadron.Miembros;
        for (int i = 0; i < miembros.Count; i++)
        {
            int x = miembros[i].Posicion == Posicion.Retaguardia ? 20 : 236;
            _tarjetas[miembros[i]].Location = new Point(x, 36 + i * 106);
        }
    }

    private void ActualizarControles()
    {
        bool turnoJugador = !_ocupado && _combate.Resultado == ResultadoCombate.EnCurso && _combate.Fase == FaseCombate.Jugador;
        var acciones = _seleccionado != null && turnoJugador
            ? _combate.AccionesDisponibles(_seleccionado)
            : Array.Empty<TipoAccion>();

        _btnAtacar.Enabled = acciones.Contains(TipoAccion.Atacar);
        _btnCurar.Enabled = acciones.Contains(TipoAccion.Curar);
        _btnMover.Enabled = acciones.Contains(TipoAccion.Mover);
        _btnConstruir.Enabled = acciones.Contains(TipoAccion.Construir);
        _btnCurar.Text = _accionPendiente == TipoAccion.Curar ? "Cancelar" : "Curar";
        _btnConstruir.Text = _accionPendiente == TipoAccion.Construir ? "Cancelar" : "Construir";
        _btnDeshacer.Enabled = turnoJugador && _combate.Comandos.PuedeDeshacer;
        _btnRehacer.Enabled = turnoJugador && _combate.Comandos.PuedeRehacer;
        _btnFinTurno.Enabled = turnoJugador;
        _cmbEstrategia.Enabled = turnoJugador;

        foreach (var (personaje, tarjeta) in _tarjetas)
        {
            tarjeta.Seleccionada = personaje == _seleccionado;
            tarjeta.Objetivable = _accionPendiente.HasValue && personaje is MiembroEscuadron && personaje.EstaVivo;
        }

        if (_combate.Fase == FaseCombate.Jugador && _combate.Resultado == ResultadoCombate.EnCurso)
        {
            int pendientes = _combate.Escuadron.Miembros.Count(_combate.PuedeActuar);
            _lblRonda.Text = $"Ronda {_combate.Ronda} · tu turno · {pendientes} por actuar";
            _lblRonda.ForeColor = Tema.Texto;
        }

        _lblSeleccion.Text = _seleccionado == null
            ? "Ningún tripulante seleccionado"
            : $"{_seleccionado.Nombre} · {Tema.NombreRol(_seleccionado.Rol)} · {_seleccionado.Estado.Nombre}";
    }

    private void ActualizarRegistros()
    {
        var entradas = _combate.Registro.Entradas;
        for (; _indiceRegistro < entradas.Count; _indiceRegistro++)
        {
            var entrada = entradas[_indiceRegistro];
            var lista = entrada.Categoria switch
            {
                CategoriaRegistro.Turno => _lstTurnos,
                CategoriaRegistro.Danio => _lstDanio,
                CategoriaRegistro.Comando => _lstComandos,
                CategoriaRegistro.Estacion => _lstControl,
                CategoriaRegistro.IA => _lstIA,
                _ => _lstBitacora
            };
            lista.AgregarLinea(entrada.Texto);

            if (entrada.Categoria is CategoriaRegistro.Danio or CategoriaRegistro.IA)
            {
                _lstBitacora.AgregarLinea(entrada.Texto);
            }
        }
    }

    private void ProcesarEventos()
    {
        foreach (var evento in _combate.ConsumirEventos())
        {
            if (_tarjetas.TryGetValue(evento.Objetivo, out var tarjeta))
            {
                Color color = evento.Tipo switch
                {
                    TipoEvento.Danio => Tema.Peligro,
                    TipoEvento.Esquiva => Tema.Acento,
                    TipoEvento.Curacion => Tema.Exito,
                    TipoEvento.Veneno => Tema.Veneno,
                    TipoEvento.Estado => Tema.ColorEstado(evento.Objetivo.Estado.Tipo),
                    TipoEvento.Escudo => Tema.Escudo,
                    TipoEvento.Refuerzo => Tema.Exito,
                    _ => Tema.Peligro
                };
                tarjeta.Destellar(color);
                tarjeta.MostrarTexto(evento.Texto, color);
            }

            if (evento.Tipo == TipoEvento.Emergencia)
            {
                IniciarAlertaEmergencia();
            }
        }
    }

    private void ActualizarDefensas()
    {
        var eslabones = _enfocado.Defensa.Eslabones;
        bool esPropio = _enfocado is MiembroEscuadron;
        _lblDefensaDe.Text = $"Cadena de {_enfocado.Nombre}" + (esPropio ? "" : " (solo lectura)");

        int seleccion = _lstEslabones.SelectedIndex;
        _lstEslabones.BeginUpdate();
        _lstEslabones.Items.Clear();
        for (int i = 0; i < eslabones.Count; i++)
        {
            _lstEslabones.Items.Add($"{i + 1}. {eslabones[i].Descripcion}");
        }
        _lstEslabones.EndUpdate();
        if (seleccion >= 0 && seleccion < _lstEslabones.Items.Count)
        {
            _lstEslabones.SelectedIndex = seleccion;
        }
        ActualizarBotonesDefensa();
    }

    private void ActualizarBotonesDefensa()
    {
        int indice = _lstEslabones.SelectedIndex;
        int ultimoMovible = _lstEslabones.Items.Count - 2;
        bool editable = _enfocado is MiembroEscuadron && !_ocupado && indice >= 0 && indice <= ultimoMovible;
        _btnSubir.Enabled = editable && indice > 0;
        _btnBajar.Enabled = editable && indice < ultimoMovible;
    }

    private void ActualizarArsenal()
    {
        var lineas = _combate.Inventario.Items.Select(item => item switch
        {
            Arma a => $"[ARMA] {a.Nombre} · +{a.BonusDanio} daño · {Tema.NombreRol(a.Portador)}{(a.Envenenada ? " · envenenada" : "")} · {a.Precio} ₵",
            Pocion p => $"[POCIÓN] {p.Nombre} · cura {p.Curacion} · {p.Precio} ₵",
            Armadura ar => $"[ARMADURA] {ar.Nombre} · -{ar.Reduccion:P0} daño · {Tema.NombreRol(ar.Portador)} · {ar.Precio} ₵",
            _ => item.Nombre
        });
        _lstArsenal.Reemplazar(lineas);
    }

    private void ActualizarIA()
    {
        var enemigo = _combate.EnemigoPrincipal;
        int indice = _fabricasEstrategia.FindIndex(f => f().GetType() == enemigo.Estrategia.GetType());
        if (indice != _cmbEstrategia.SelectedIndex)
        {
            _actualizandoCombo = true;
            _cmbEstrategia.SelectedIndex = indice;
            _actualizandoCombo = false;
        }

        string decision = enemigo.UltimaDecision?.Razon ?? "Todavía no actuó.";
        _lblDecision.Text = $"{enemigo.Estrategia.Descripcion}\nÚltima decisión: {decision}";
    }

    private void IniciarAlertaEmergencia()
    {
        _parpadeos = 16;
        _lblEmergencia.Visible = true;
        _timerEmergencia.Start();
        _pestanias.SelectedTab = _tabControl;
        _lblAyuda.Text = "El Capitán está en peligro: el Control Central activó el modo emergencia.";
    }

    private void Parpadear()
    {
        _parpadeos--;
        _lblEmergencia.ForeColor = _parpadeos % 2 == 0 ? Tema.Peligro : Tema.Texto;
        _campo.Alerta = _parpadeos % 2 == 0;
        if (_parpadeos <= 0)
        {
            _timerEmergencia.Stop();
            _campo.Alerta = false;
        }
    }

    private async Task<bool> TerminarSiCorrespondeAsync()
    {
        bool termino = _combate.Resultado != ResultadoCombate.EnCurso;
        if (termino && !_terminado)
        {
            _terminado = true;
            _ocupado = true;
            ActualizarControles();
            _lblRonda.Text = _combate.Resultado == ResultadoCombate.Victoria ? "Victoria" : "Derrota";
            _lblRonda.ForeColor = _combate.Resultado == ResultadoCombate.Victoria ? Tema.Exito : Tema.Peligro;
            await Task.Delay(1500);

            var resumen = _combate.GenerarResumen();
            var nuevos = _partida.RegistrarResultado(resumen);
            if (_repositorio != null && !_partida.EsSandbox)
            {
                _repositorio.Guardar(_partida.Progreso);
            }

            using (var final = new FormFinal(resumen, _partida.Progreso, nuevos))
            {
                final.ShowDialog(this);
            }
            Close();
        }
        return termino;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_terminado && e.CloseReason == CloseReason.UserClosing)
        {
            var respuesta = MessageBox.Show(this, "¿Abandonar la misión? No se guarda XP de este combate.", "Kepler-7",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            e.Cancel = respuesta != DialogResult.Yes;
        }
        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timerEmergencia.Dispose();
        }
        base.Dispose(disposing);
    }
}
