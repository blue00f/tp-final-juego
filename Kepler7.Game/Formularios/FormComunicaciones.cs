using Kepler7.Core.Bridge;
using Kepler7.Core.Juego;

namespace Kepler7.Game.Formularios;

internal sealed class FormComunicaciones : FormBase
{
    private readonly Partida _partida;
    private readonly List<Opcion> _opcionesMensaje = new();
    private readonly List<Opcion> _opcionesCifrado = new();
    private readonly Label _lblEfectoMensaje;
    private readonly Label _lblDescripcionCifrado;
    private readonly TextBox _txtContenido = new CajaTexto(monoespaciada: false, soloLectura: false) { Multiline = false };
    private readonly CajaTexto _txtPlano = new();
    private readonly CajaTexto _txtCifrado = new();
    private readonly CajaTexto _txtRecibido = new();
    private readonly Label _lblTituloCifrado;
    private readonly Label _lblRiesgo;
    private readonly Label _lblResultado;
    private readonly Boton _btnTransmitir = new("Transmitir a la Tierra", 200, 32);
    private readonly ProgressBar _progreso = new() { Style = ProgressBarStyle.Continuous, Maximum = 30, Visible = false };
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 40 };

    public FormComunicaciones(Partida partida)
    {
        _partida = partida;
        Text = "Kepler-7 · Comunicaciones";
        ClientSize = new Size(1040, 660);
        AgregarEncabezado("Comunicaciones con la Tierra", "Bridge",
            "Mensaje (abstracción) mantiene una referencia a ICifrado (implementación). Tipo y cifrado se eligen por separado " +
            "y se combinan en ejecución: 3 × 3 = 9 combinaciones con 6 clases.");

        Controls.Add(Titulo("1. Tipo de mensaje (qué pide)", 112));
        // Cada grupo de RadioButton va en su propio panel: si no, Windows los trataría como un solo grupo.
        var grupoMensaje = new Panel { Bounds = new Rectangle(24, 136, 340, 36) };
        int x = 0;
        foreach (var tipo in CentroComunicaciones.TiposMensaje)
        {
            var opcion = new Opcion(tipo, 104, 34) { Location = new Point(x, 0), Tag = tipo };
            opcion.CheckedChanged += (_, _) => ActualizarVistaPrevia();
            _opcionesMensaje.Add(opcion);
            grupoMensaje.Controls.Add(opcion);
            x += 110;
        }
        Controls.Add(grupoMensaje);
        _lblEfectoMensaje = CrearTexto("", 9f, Tema.Texto, new Rectangle(24, 176, 330, 40));
        Controls.Add(_lblEfectoMensaje);

        Controls.Add(Titulo("2. Cifrado (cómo viaja)", 224));
        var grupoCifrado = new Panel { Bounds = new Rectangle(24, 248, 340, 36) };
        x = 0;
        foreach (var cifrado in CentroComunicaciones.TiposCifrado)
        {
            var opcion = new Opcion(cifrado, 104, 34) { Location = new Point(x, 0), Tag = cifrado };
            opcion.CheckedChanged += (_, _) => ActualizarVistaPrevia();
            _opcionesCifrado.Add(opcion);
            grupoCifrado.Controls.Add(opcion);
            x += 110;
        }
        Controls.Add(grupoCifrado);
        _lblDescripcionCifrado = CrearTexto("", 9f, Tema.Texto, new Rectangle(24, 288, 330, 54));
        Controls.Add(_lblDescripcionCifrado);

        Controls.Add(Titulo("3. Contenido", 350));
        _txtContenido.SetBounds(24, 374, 326, 28);
        _txtContenido.Text = "Plaga alienígena en los conductos. Solicitamos apoyo.";
        _txtContenido.MaxLength = 120;
        _txtContenido.TextChanged += (_, _) => ActualizarVistaPrevia();
        Controls.Add(_txtContenido);

        _lblRiesgo = CrearTexto("", 9f, Tema.Texto, new Rectangle(24, 418, 330, 60));
        Controls.Add(_lblRiesgo);

        _btnTransmitir.Location = new Point(24, 492);
        _btnTransmitir.Click += (_, _) => IniciarTransmision();
        _progreso.SetBounds(24, 536, 326, 14);
        Controls.Add(_btnTransmitir);
        Controls.Add(_progreso);

        _lblResultado = CrearTexto("", 10f, Tema.Texto, new Rectangle(24, 566, 330, 80), FontStyle.Bold);
        Controls.Add(_lblResultado);

        Controls.Add(Titulo("Formato del mensaje (lo define el tipo)", 112, 384));
        _txtPlano.SetBounds(384, 136, 632, 70);
        _lblTituloCifrado = Titulo("Salida del cifrado", 220, 384);
        Controls.Add(_lblTituloCifrado);
        _txtCifrado.SetBounds(384, 244, 632, 220);
        Controls.Add(Titulo("Recibido en la Tierra (descifrado)", 478, 384));
        _txtRecibido.SetBounds(384, 502, 632, 70);
        Controls.AddRange(new Control[] { _txtPlano, _txtCifrado, _txtRecibido });

        var cerrar = new Boton("Volver a la estación", 180, 32) { Location = new Point(836, 610) };
        cerrar.Click += (_, _) => Close();
        Controls.Add(cerrar);

        _timer.Tick += (_, _) => AvanzarTransmision();

        _opcionesMensaje[1].Checked = true;
        _opcionesCifrado[1].Checked = true;
        MostrarEstadoCanal();
    }

    private static Label Titulo(string texto, int y, int x = 24)
    {
        var etiqueta = CrearEtiqueta(texto, 9f, Tema.Texto, FontStyle.Bold);
        etiqueta.Location = new Point(x, y);
        return etiqueta;
    }

    private string TipoMensaje => (string)(_opcionesMensaje.FirstOrDefault(o => o.Checked)?.Tag ?? "Texto");
    private string TipoCifrado => (string)(_opcionesCifrado.FirstOrDefault(o => o.Checked)?.Tag ?? "Sin cifrado");

    private void ActualizarVistaPrevia()
    {
        if (_opcionesMensaje.Count == 3 && _opcionesCifrado.Count == 3)
        {
            var mensaje = CentroComunicaciones.Crear(TipoMensaje, TipoCifrado, _txtContenido.Text, _partida.Preparativos.ClavesRotadas);
            var paquete = mensaje.Empaquetar();
            _lblEfectoMensaje.Text = $"{mensaje.GetType().Name}: {mensaje.Efecto}";
            _lblDescripcionCifrado.Text = $"{mensaje.Cifrado.GetType().Name}: {mensaje.Cifrado.Descripcion}";
            _txtPlano.Text = paquete.TextoPlano;
            _lblTituloCifrado.Text = $"Salida del cifrado: {paquete.Cifrado} ({paquete.TextoCifrado.Length} caracteres)";
            _txtCifrado.Text = paquete.TextoCifrado;
            _txtRecibido.Text = paquete.RecibidoEnTierra;
            _lblRiesgo.Text = $"Intercepción: {mensaje.Cifrado.ProbabilidadIntercepcion:P0} · Integridad: {mensaje.Cifrado.Integridad:P0}\n" +
                              (mensaje.Cifrado.ProbabilidadIntercepcion > 0
                                  ? "Si lo interceptan, el alien empieza el combate en Berserk."
                                  : "Los aliens no pueden leerlo.");
        }
    }

    private void IniciarTransmision()
    {
        if (_partida.PuedeTransmitir && _txtContenido.Text.Trim().Length > 0)
        {
            _btnTransmitir.Enabled = false;
            _progreso.Value = 0;
            _progreso.Visible = true;
            _lblResultado.ForeColor = Tema.Acento;
            _lblResultado.Text = "Transmitiendo…";
            _timer.Start();
        }
    }

    private void AvanzarTransmision()
    {
        _progreso.Value = Math.Min(_progreso.Maximum, _progreso.Value + 1);
        if (_progreso.Value >= _progreso.Maximum)
        {
            _timer.Stop();
            _progreso.Visible = false;
            _partida.EnviarMensaje(TipoMensaje, TipoCifrado, _txtContenido.Text.Trim());
            MostrarEstadoCanal();
        }
    }

    private void MostrarEstadoCanal()
    {
        var transmision = _partida.Preparativos.Transmision;
        if (transmision != null)
        {
            foreach (var opcion in _opcionesMensaje)
            {
                opcion.Checked = (string)opcion.Tag! == transmision.Paquete.Tipo;
            }
            foreach (var opcion in _opcionesCifrado)
            {
                opcion.Checked = (string)opcion.Tag! == transmision.Paquete.Cifrado;
            }
            _txtPlano.Text = transmision.Paquete.TextoPlano;
            _txtCifrado.Text = transmision.Paquete.TextoCifrado;
            _txtRecibido.Text = transmision.Paquete.RecibidoEnTierra;

            bool interceptada = transmision.Interceptada;
            _lblResultado.ForeColor = interceptada ? Tema.Peligro : Tema.Exito;
            _lblResultado.Text = (interceptada
                ? "El mensaje fue interceptado: el alien empieza el combate en Berserk.\n"
                : "La Tierra recibió el mensaje sin intercepción.\n") +
                $"Efecto en la próxima misión (integridad {transmision.Integridad:P0}): {transmision.Efecto}.";
            _btnTransmitir.Enabled = false;
            _btnTransmitir.Text = "Canal ocupado hasta desplegar";
            foreach (var opcion in _opcionesMensaje.Concat(_opcionesCifrado))
            {
                opcion.Enabled = false;
            }
            _txtContenido.ReadOnly = true;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
        }
        base.Dispose(disposing);
    }
}
