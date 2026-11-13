using Kepler7.Core.Juego;
using Kepler7.Core.Progreso;

namespace Kepler7.Game.Formularios;

internal sealed class FormInicio : FormBase
{
    private static readonly string[] Integrantes = { "Iñaki Farias", "Agustin Quiroz", "Bruno Roca" };

    private readonly RepositorioProgreso _repositorio;
    private readonly Boton _btnCampania = new("", 260, 40);
    private readonly Label _lblProgreso;
    private readonly ProgressBar _barraXp = new();
    private ProgresoJugador _progreso;
    private Partida _partida;

    public FormInicio(RepositorioProgreso repositorio)
    {
        _repositorio = repositorio;
        _progreso = repositorio.Cargar();
        _partida = NuevaPartida();

        Text = "Kepler-7";
        ClientSize = new Size(640, 520);

        var lblTitulo = CrearTexto("Kepler-7", 26f, Tema.Acento, new Rectangle(0, 30, 640, 50), FontStyle.Bold);
        lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
        var lblSub = CrearTexto("Protocolo de Contención", 11f, Tema.TextoTenue, new Rectangle(0, 80, 640, 24));
        lblSub.TextAlign = ContentAlignment.MiddleCenter;

        _lblProgreso = CrearTexto("", 9f, Tema.Texto, new Rectangle(0, 122, 640, 20));
        _lblProgreso.TextAlign = ContentAlignment.MiddleCenter;
        _barraXp.SetBounds(190, 146, 260, 14);
        _barraXp.Maximum = ProgresoJugador.XpPorNivel;

        Controls.AddRange(new Control[] { lblTitulo, lblSub, _lblProgreso, _barraXp });

        _btnCampania.Click += (_, _) => AbrirEstacion(_partida, _repositorio);
        var botones = new (string Texto, EventHandler Accion)[]
        {
            ("Modo sandbox (todo desbloqueado)", (_, _) => AbrirEstacion(Partida.NuevaSandbox(), null)),
            ("Tutorial", (_, _) => new FormTutorial().ShowDialog(this)),
            ("Logros", (_, _) => new FormLogros(_progreso).ShowDialog(this)),
            ("Reiniciar progreso", (_, _) => ReiniciarProgreso()),
            ("Salir", (_, _) => Salir())
        };

        int y = 184;
        _btnCampania.Location = new Point(190, y);
        Controls.Add(_btnCampania);
        y += _btnCampania.Height + 10;
        foreach (var (texto, accion) in botones)
        {
            var boton = new Boton(texto, 260, 32) { Location = new Point(190, y) };
            boton.Click += accion;
            Controls.Add(boton);
            y += boton.Height + 6;
        }

        var lblEquipo = CrearTexto($"Integrantes: {string.Join(", ", Integrantes)}\nPatrones de Diseño · UAI · 2026",
            9f, Tema.TextoTenue, new Rectangle(16, 474, 600, 36));
        Controls.Add(lblEquipo);

        ActualizarProgreso();
    }

    private Partida NuevaPartida() => Partida.Nueva(_progreso, carpetaDatos: _repositorio.Carpeta);

    private void ActualizarProgreso()
    {
        _btnCampania.Text = _progreso.CampaniaCompletada ? "Estación (campaña completa)"
            : _progreso.AliensDerrotados > 0 ? $"Continuar campaña (sector {_partida.SectorActual})"
            : "Iniciar campaña";
        _lblProgreso.Text = $"{_progreso.Rango} · Nivel {_progreso.Nivel} · XP {_progreso.XpEnNivelActual}/{ProgresoJugador.XpPorNivel}" +
                            $" · Aliens {_progreso.AliensDerrotados}/{CatalogoAliens.Cantidad}" +
                            $" · Patrones {_progreso.PatronesDominados.Count}/12";
        _barraXp.Value = Math.Clamp(_progreso.XpEnNivelActual, 0, _barraXp.Maximum);
    }

    private void AbrirEstacion(Partida partida, RepositorioProgreso? repositorio)
    {
        using (var estacion = new FormEstacion(partida, repositorio))
        {
            estacion.ShowDialog(this);
        }
        ActualizarProgreso();
    }

    private void ReiniciarProgreso()
    {
        var respuesta = MessageBox.Show(this, "¿Borrar todo el progreso guardado (XP, sectores, logros y bóveda)?", "Kepler-7",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (respuesta == DialogResult.Yes)
        {
            _repositorio.Reiniciar();
            _progreso = _repositorio.Cargar();
            _partida = NuevaPartida();
            ActualizarProgreso();
        }
    }

    private void Salir()
    {
        _repositorio.Guardar(_progreso);
        var secuencia = _partida.Control.ApagarEstacion();
        using (var form = new FormSecuencia("Apagado de la estación", "Apagar estación cierra los subsistemas en orden inverso.",
                   secuencia, "Cerrar"))
        {
            form.ShowDialog(this);
        }
        Close();
    }
}
