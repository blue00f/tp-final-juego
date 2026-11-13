using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormSecuencia : FormBase
{
    private readonly IReadOnlyList<string> _lineas;
    private readonly ListaRegistro _lista = new();
    private readonly Boton _continuar;
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 260 };
    private int _indice;

    public FormSecuencia(string titulo, string subtitulo, IReadOnlyList<string> lineas, string textoBoton)
    {
        _lineas = lineas;
        Text = $"Kepler-7 · {titulo}";
        ClientSize = new Size(760, 470);

        var lblTitulo = CrearEtiqueta(titulo, 14f, Tema.Texto, FontStyle.Bold);
        lblTitulo.Location = new Point(24, 18);
        var lblSub = CrearEtiqueta(subtitulo, 9.5f, Tema.TextoTenue);
        lblSub.Location = new Point(26, 56);

        _lista.SetBounds(24, 86, 712, 310);
        _lista.BorderStyle = BorderStyle.FixedSingle;

        var saltar = new Boton("Saltar", 110, 32) { Location = new Point(506, 414) };
        saltar.Click += (_, _) => MostrarTodo();

        _continuar = new Boton(textoBoton, 110, 32) { Location = new Point(626, 414), Enabled = false };
        _continuar.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };

        Controls.AddRange(new Control[] { lblTitulo, lblSub, _lista, saltar, _continuar });

        _timer.Tick += (_, _) => MostrarSiguiente();
        Shown += (_, _) => _timer.Start();
    }

    private void MostrarSiguiente()
    {
        if (_indice < _lineas.Count)
        {
            _lista.AgregarLinea(_lineas[_indice]);
            _indice++;
        }
        if (_indice >= _lineas.Count)
        {
            _timer.Stop();
            _continuar.Enabled = true;
            _continuar.Focus();
        }
    }

    private void MostrarTodo()
    {
        while (_indice < _lineas.Count)
        {
            MostrarSiguiente();
        }
        MostrarSiguiente();
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
