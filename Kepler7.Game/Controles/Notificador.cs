namespace Kepler7.Game.Controles;

internal sealed class Notificador : Form
{
    private static readonly List<Notificador> Abiertos = new();

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 4000 };

    private Notificador(string titulo, string texto)
    {
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        Text = titulo;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        ClientSize = new Size(320, 40);
        BackColor = Tema.Panel;
        ForeColor = Tema.Texto;
        var etiqueta = new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Padding = new Padding(10, 0, 10, 0),
            TextAlign = ContentAlignment.MiddleLeft,
            Font = Tema.Crear(9.5f)
        };
        Controls.Add(etiqueta);
        _timer.Tick += (_, _) => Close();
    }

    protected override bool ShowWithoutActivation => true;

    public static void Mostrar(Form? duenio, string titulo, string texto)
    {
        var aviso = new Notificador(titulo, texto);
        var referencia = duenio != null && !duenio.IsDisposed ? duenio.Bounds : Screen.PrimaryScreen!.WorkingArea;
        aviso.Location = new Point(referencia.Right - aviso.Width - 24, referencia.Top + 40 + Abiertos.Count * (aviso.Height + 8));
        Abiertos.Add(aviso);
        aviso.FormClosed += (_, _) => Abiertos.Remove(aviso);
        aviso.Show();
        aviso._timer.Start();
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
