using Kepler7.Core.Juego;
using Kepler7.Core.Progreso;
using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormEstacion : FormBase
{
    private readonly Partida _partida;
    private readonly RepositorioProgreso? _repositorio;

    private readonly Label _lblTitulo = new();
    private readonly Label _lblEstado = new();
    private readonly TarjetaModulo _modControl = new("Control Central", Patrones.Facade);
    private readonly TarjetaModulo _modComunicaciones = new("Comunicaciones", Patrones.Bridge);
    private readonly TarjetaModulo _modLaboratorio = new("Laboratorio", Patrones.Prototype);
    private readonly TarjetaModulo _modRegistro = new("Registro", Patrones.Flyweight);
    private readonly TarjetaModulo _modHangar = new("Hangar", Patrones.Builder);
    private readonly TarjetaModulo _modBoveda = new("Bóveda", Patrones.Proxy);
    private readonly ListaRegistro _bitacora = new();

    private readonly Panel _panelMision = new();
    private readonly Label _lblMision = new();
    private readonly Label _lblAlien = new();
    private readonly Label _lblPreparativos = new();
    private readonly ComboBox _cmbSector = new();
    private readonly Boton _btnDesplegar = new("Iniciar turno y desplegar", 404, 40);
    private TarjetaAlien? _vistaAlien;

    public FormEstacion(Partida partida, RepositorioProgreso? repositorio)
    {
        _partida = partida;
        _repositorio = repositorio;
        Text = partida.EsSandbox ? "Kepler-7 · Estación (sandbox)" : "Kepler-7 · Estación";
        ClientSize = new Size(1280, 780);

        ConstruirEncabezado();
        ConstruirModulos();
        ConstruirPanelMision();

        _partida.PatronDominado += AlDominarPatron;
        _bitacora.AgregarLinea(partida.EsSandbox
            ? "Modo sandbox: todos los subsistemas en línea, sector a elección. No se guarda el progreso."
            : "Bienvenido a la estación, Capitán. Prepará la misión y desplegá.");
        Refrescar();
    }

    private void ConstruirEncabezado()
    {
        var barra = new Panel { Location = new Point(0, 0), Size = new Size(1280, 56) };
        Configurar(_lblTitulo, "Estación orbital Kepler-7", 14f, Tema.Texto, FontStyle.Bold);
        _lblTitulo.Location = new Point(16, 14);
        Configurar(_lblEstado, "", 9.5f, Tema.Texto);
        _lblEstado.AutoSize = false;
        _lblEstado.TextAlign = ContentAlignment.MiddleRight;
        _lblEstado.SetBounds(380, 14, 780, 28);
        var btnLogros = new Boton("Logros", 90, 30) { Location = new Point(1174, 13) };
        btnLogros.Click += (_, _) => new FormLogros(_partida.Progreso).ShowDialog(this);
        barra.Controls.AddRange(new Control[] { _lblTitulo, _lblEstado, btnLogros });
        Controls.Add(barra);
    }

    private void ConstruirModulos()
    {
        var modulos = new[] { _modControl, _modComunicaciones, _modLaboratorio, _modRegistro, _modHangar, _modBoveda };
        for (int i = 0; i < modulos.Length; i++)
        {
            modulos[i].Location = new Point(16 + (i % 3) * 268, 70 + (i / 3) * 222);
            Controls.Add(modulos[i]);
        }

        _modControl.Click += (_, _) => Desplegar();
        _modComunicaciones.Click += (_, _) => Abrir(SubsistemaEstacion.Comunicaciones, () => new FormComunicaciones(_partida));
        _modLaboratorio.Click += (_, _) => Abrir(SubsistemaEstacion.Laboratorio, () => new FormLaboratorio(_partida));
        _modRegistro.Click += (_, _) => Abrir(SubsistemaEstacion.Registro, () => new FormRegistro(_partida));
        _modHangar.Click += (_, _) => Abrir(SubsistemaEstacion.Hangar, () => new FormHangar(_partida));
        _modBoveda.Click += (_, _) => Abrir(SubsistemaEstacion.Boveda, () => new FormBoveda(_partida));

        var lblBitacora = new Label();
        Configurar(lblBitacora, "Bitácora de la estación", 9f, Tema.Texto, FontStyle.Bold);
        lblBitacora.Location = new Point(16, 518);
        _bitacora.SetBounds(16, 540, 792, 224);
        Controls.Add(lblBitacora);
        Controls.Add(_bitacora);
    }

    private void ConstruirPanelMision()
    {
        _panelMision.SetBounds(828, 70, 436, 694);
        _panelMision.BorderStyle = BorderStyle.FixedSingle;
        Controls.Add(_panelMision);

        Configurar(_lblMision, "", 11f, Tema.Texto, FontStyle.Bold);
        _lblMision.Location = new Point(16, 12);
        _panelMision.Controls.Add(_lblMision);

        if (_partida.EsSandbox)
        {
            _cmbSector.SetBounds(236, 10, 184, 26);
            _cmbSector.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var sector in CatalogoAliens.Sectores)
            {
                _cmbSector.Items.Add($"Sector {sector.Numero}: {sector.Alien}");
            }
            _cmbSector.SelectedIndex = _partida.SectorActual - 1;
            _cmbSector.SelectedIndexChanged += (_, _) =>
            {
                _partida.SectorSandbox = _cmbSector.SelectedIndex + 1;
                Refrescar();
            };
            _panelMision.Controls.Add(_cmbSector);
        }

        Configurar(_lblAlien, "", 9f, Tema.Texto);
        _lblAlien.AutoSize = false;
        _lblAlien.SetBounds(16, 350, 404, 76);
        _panelMision.Controls.Add(_lblAlien);

        var lblPrep = new Label();
        Configurar(lblPrep, "Preparativos (los aplica el Control Central)", 9f, Tema.Texto, FontStyle.Bold);
        lblPrep.Location = new Point(16, 432);
        _panelMision.Controls.Add(lblPrep);

        Configurar(_lblPreparativos, "", 9f, Tema.Texto);
        _lblPreparativos.AutoSize = false;
        _lblPreparativos.SetBounds(16, 454, 404, 160);
        _panelMision.Controls.Add(_lblPreparativos);

        _btnDesplegar.Location = new Point(16, 624);
        _btnDesplegar.Font = Tema.Crear(10f, FontStyle.Bold);
        _btnDesplegar.Click += (_, _) => Desplegar();
        _panelMision.Controls.Add(_btnDesplegar);
    }

    private static void Configurar(Label etiqueta, string texto, float tamanio, Color color, FontStyle estilo = FontStyle.Regular)
    {
        etiqueta.Text = texto;
        etiqueta.AutoSize = true;
        etiqueta.ForeColor = color;
        etiqueta.BackColor = Color.Transparent;
        etiqueta.Font = Tema.Crear(tamanio, estilo);
    }

    private void Abrir(SubsistemaEstacion subsistema, Func<Form> crear)
    {
        if (!_partida.Desbloqueado(subsistema))
        {
            var info = CatalogoAliens.Info((int)subsistema);
            MessageBox.Show(this, $"Este subsistema está fuera de servicio. Se restaura al vencer al alien {(int)subsistema}: {info.Alien}.",
                "Kepler-7", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            using (var form = crear())
            {
                form.ShowDialog(this);
            }
            Guardar();
            Refrescar();
        }
    }

    private void Desplegar()
    {
        bool campaniaTerminada = _partida.Progreso.CampaniaCompletada && !_partida.EsSandbox;
        bool desplegar = false;
        if (campaniaTerminada)
        {
            MessageBox.Show(this, "Kepler-7 está a salvo. Probá el modo sandbox para repetir combates.", "Campaña completa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            // La interfaz habla con la FACHADA: un solo llamado prepara toda la estación.
            var secuencia = _partida.Control.IniciarTurno();
            using var form = new FormSecuencia("Control Central",
                "IniciarTurno() coordina los seis subsistemas y aplica todos los preparativos.", secuencia, "Desplegar");
            desplegar = form.ShowDialog(this) == DialogResult.OK;
        }

        if (desplegar)
        {
            int derrotadosAntes = _partida.Progreso.AliensDerrotados;
            using (var combate = new FormCombate(_partida, _repositorio))
            {
                combate.ShowDialog(this);
            }
            Guardar();
            RegistrarRegreso(derrotadosAntes);
            Refrescar();

            if (!_partida.EsSandbox && derrotadosAntes < CatalogoAliens.Cantidad && _partida.Progreso.CampaniaCompletada)
            {
                using var final = new FormCampania(_partida.Progreso);
                final.ShowDialog(this);
            }
        }
    }

    private void RegistrarRegreso(int derrotadosAntes)
    {
        int derrotados = _partida.Progreso.AliensDerrotados;
        if (derrotados > derrotadosAntes && CatalogoAliens.Restaura(derrotados) is { } subsistema)
        {
            _bitacora.AgregarLinea($"{subsistema} vuelve a estar en línea. {CatalogoAliens.Info(derrotados).Recompensa}.");
            ModuloDe(subsistema).Destellar();
        }
        else
        {
            _bitacora.AgregarLinea("El escuadrón regresó a la estación. Soporte vital lo dejará listo para la próxima misión.");
        }
    }

    private void AlDominarPatron(string patron)
    {
        _bitacora.AgregarLinea($"Patrón dominado: {patron}.");
        Notificador.Mostrar(ActiveForm ?? this, "Logro desbloqueado", $"Patrón dominado: {patron}");
        Guardar();
    }

    private void Guardar()
    {
        if (_repositorio != null && !_partida.EsSandbox)
        {
            _repositorio.Guardar(_partida.Progreso);
        }
    }

    private TarjetaModulo ModuloDe(SubsistemaEstacion subsistema) => subsistema switch
    {
        SubsistemaEstacion.Comunicaciones => _modComunicaciones,
        SubsistemaEstacion.Laboratorio => _modLaboratorio,
        SubsistemaEstacion.Registro => _modRegistro,
        SubsistemaEstacion.Hangar => _modHangar,
        _ => _modBoveda
    };

    private void Refrescar()
    {
        var progreso = _partida.Progreso;
        var prep = _partida.Preparativos;

        _lblEstado.Text = _partida.EsSandbox
            ? "Modo sandbox (no se guarda el progreso)"
            : $"{progreso.Rango} · Nivel {progreso.Nivel} · XP {progreso.XpEnNivelActual}/{ProgresoJugador.XpPorNivel} · " +
              $"Sectores {progreso.AliensDerrotados}/{CatalogoAliens.Cantidad} · Patrones {progreso.PatronesDominados.Count}/12";

        _modControl.Detalle = new[]
        {
            "Clic: IniciarTurno() y despliegue.",
            "Emergencia automática si el Capitán baja del 30%.",
            $"Preparativos listos: {ContarPreparativos(prep)}"
        };
        Configurar(_modComunicaciones, SubsistemaEstacion.Comunicaciones, prep.Transmision == null
            ? new[] { "Canal libre: enviá un mensaje a la Tierra.", "Tipo × cifrado se combinan en vivo." }
            : new[]
            {
                $"Enviado: {prep.Transmision.Paquete.Tipo} · {prep.Transmision.Paquete.Cifrado}",
                prep.Transmision.Efecto,
                prep.Transmision.Interceptada ? "Fue interceptado por los aliens." : "No fue interceptado."
            });
        Configurar(_modLaboratorio, SubsistemaEstacion.Laboratorio, prep.ClonReserva == null
            ? new[] { "Sin clon de reserva.", "Cloná un tripulante y mejoralo." }
            : new[] { $"Criogenia: {prep.ClonReserva.Nombre}", string.Join(", ", prep.ClonReserva.Habilidades), "Releva al primer caído." });
        Configurar(_modRegistro, SubsistemaEstacion.Registro, new[]
        {
            "10.000 tripulantes · 5 especies.",
            prep.Destacamento == null ? "Sin destacamento de apoyo." : $"Destacamento: {prep.Destacamento.Nombre} ({prep.Destacamento.Bonus.Descripcion})",
            prep.OficialSeguridad == null ? "Sin oficial de seguridad." : $"Oficial: {prep.OficialSeguridad.Nombre}"
        });
        Configurar(_modHangar, SubsistemaEstacion.Hangar, prep.Nave == null
            ? new[] { "Sin nave: despliegue a pie.", "Elegí receta y plan de ensamblaje." }
            : new[] { $"Nave: {prep.Nave.Nombre}", prep.Nave.ResumenEfectos() });
        Configurar(_modBoveda, SubsistemaEstacion.Boveda, new[]
        {
            prep.InteligenciaJefe ? "Debilidad del jefe conocida." : "Informe del jefe sin leer.",
            prep.ClavesRotadas ? "Claves AES rotadas." : "Claves AES de fábrica (comprometidas).",
            $"Rechazos registrados: {_partida.Boveda.Registro.Intentos.Count}"
        });

        var info = CatalogoAliens.Info(_partida.SectorActual);
        _lblMision.Text = _partida.EsSandbox ? "Misión de prueba" : $"Próxima misión: sector {info.Numero}";
        _lblAlien.Text = $"{info.Descripcion}\nIA inicial: {info.EstrategiaInicial}\nRecompensa: {info.Recompensa}";
        ActualizarVistaAlien();
        _lblPreparativos.Text = DescribirPreparativos(prep);
        _btnDesplegar.Enabled = _partida.EsSandbox || !progreso.CampaniaCompletada;
        _btnDesplegar.Text = _btnDesplegar.Enabled ? "Iniciar turno y desplegar" : "Kepler-7 está a salvo";
    }

    private void Configurar(TarjetaModulo modulo, SubsistemaEstacion subsistema, IReadOnlyList<string> detalle)
    {
        modulo.EnLinea = _partida.Desbloqueado(subsistema);
        modulo.Detalle = detalle;
        modulo.Requisito = $"Se restaura al vencer al alien {(int)subsistema}: {CatalogoAliens.Info((int)subsistema).Alien}.";
        modulo.Invalidate();
    }

    private void ActualizarVistaAlien()
    {
        if (_vistaAlien == null || _vistaAlien.Enemigo.Nombre != CatalogoAliens.Info(_partida.SectorActual).Alien)
        {
            if (_vistaAlien != null)
            {
                _panelMision.Controls.Remove(_vistaAlien);
                _vistaAlien.Dispose();
            }
            _vistaAlien = new TarjetaAlien(CatalogoAliens.Crear(_partida.SectorActual, _partida.Aleatorio))
            {
                Location = new Point(78, 44),
                Cursor = Cursors.Default
            };
            _panelMision.Controls.Add(_vistaAlien);
        }
    }

    private static int ContarPreparativos(Preparativos p) =>
        new object?[] { p.Nave, p.Destacamento, p.Transmision, p.ClonReserva }.Count(x => x != null)
        + (p.InteligenciaJefe ? 1 : 0);

    private static string DescribirPreparativos(Preparativos p)
    {
        var lineas = new List<string>
        {
            p.Nave == null ? "Nave: ninguna" : $"Nave {p.Nave.Nombre}: {p.Nave.ResumenEfectos()}",
            p.Destacamento == null ? "Destacamento: ninguno" : $"Destacamento {p.Destacamento.Nombre}: {p.Destacamento.Bonus.Descripcion}",
            p.Transmision == null ? "Transmisión: ninguna" : $"Transmisión: {p.Transmision.Paquete.Tipo} ({p.Transmision.Paquete.Cifrado})" +
                (p.Transmision.Interceptada ? " (interceptada: el alien empieza en Berserk)" : ""),
            p.ClonReserva == null ? "Clon de reserva: ninguno" : $"Clon de reserva: {p.ClonReserva.Nombre}",
            p.InteligenciaJefe ? "Inteligencia: el jefe pierde Armadura y Escudo" : "Inteligencia del jefe: no"
        };
        return string.Join("\n", lineas);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _partida.PatronDominado -= AlDominarPatron;
        }
        base.Dispose(disposing);
    }
}
