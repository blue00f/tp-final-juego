using System.Drawing.Drawing2D;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Game.Controles;

internal abstract class TarjetaBase : Control
{
    private const int DuracionDestello = 10;
    private const int DuracionTexto = 30;

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 50 };
    private Color _colorDestello;
    private int _destello;
    private string _texto = "";
    private Color _colorTexto;
    private int _vidaTexto;

    protected TarjetaBase(Personaje unidad)
    {
        Unidad = unidad;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Tema.Panel;
        Cursor = Cursors.Hand;
        _timer.Tick += (_, _) => Animar();
    }

    public Personaje Unidad { get; }

    private bool _seleccionada;
    public bool Seleccionada
    {
        get => _seleccionada;
        set { _seleccionada = value; Invalidate(); }
    }

    private bool _objetivable;
    public bool Objetivable
    {
        get => _objetivable;
        set { _objetivable = value; Invalidate(); }
    }

    public void Destellar(Color color)
    {
        _colorDestello = color;
        _destello = DuracionDestello;
        _timer.Start();
    }

    public void MostrarTexto(string texto, Color color)
    {
        _texto = texto;
        _colorTexto = color;
        _vidaTexto = DuracionTexto;
        _timer.Start();
    }

    private void Animar()
    {
        if (_destello > 0)
        {
            _destello--;
        }
        if (_vidaTexto > 0)
        {
            _vidaTexto--;
        }
        if (_destello == 0 && _vidaTexto == 0)
        {
            _timer.Stop();
        }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        Color fondo = Tema.Panel;
        if (_destello > 0)
        {
            fondo = Tema.Mezclar(Tema.Panel, _colorDestello, _destello / (DuracionDestello * 2.5));
        }
        using (var pincel = new SolidBrush(fondo))
        {
            g.FillRectangle(pincel, rect);
        }

        Color borde = Tema.ColorEstado(Unidad.Estado.Tipo);
        int grosor = Unidad.Estado.Tipo == TipoEstado.Normal ? 1 : 2;
        if (Seleccionada)
        {
            borde = Tema.Acento;
            grosor = 3;
        }
        else if (Objetivable)
        {
            borde = Tema.Exito;
            grosor = 3;
        }
        using (var lapiz = new Pen(borde, grosor) { Alignment = PenAlignment.Inset })
        {
            g.DrawRectangle(lapiz, 0, 0, Width - 1, Height - 1);
        }

        PintarContenido(g, Rectangle.Inflate(rect, -8, -6));

        if (!Unidad.EstaVivo)
        {
            using var velo = new SolidBrush(Color.FromArgb(190, Tema.Fondo));
            g.FillRectangle(velo, rect);
            using var fuente = Tema.Crear(12f, FontStyle.Bold);
            TextRenderer.DrawText(g, "Caído", fuente, rect, Tema.Peligro,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        if (_vidaTexto > 0)
        {
            using var fuente = Tema.Crear(10f, FontStyle.Bold);
            var tamanio = TextRenderer.MeasureText(_texto, fuente);
            TextRenderer.DrawText(g, _texto, fuente, new Point(Width - tamanio.Width - 8, 6), _colorTexto);
        }
    }

    protected abstract void PintarContenido(Graphics g, Rectangle area);

    protected void PintarEstado(Graphics g, Point posicion, Font fuente)
    {
        var estado = Unidad.Estado;
        string texto = estado.Tipo == TipoEstado.Normal
            ? "Estado: normal"
            : $"Estado: {estado.Nombre.ToLowerInvariant()} ({estado.TurnosRestantes} t)";
        Color color = estado.Tipo == TipoEstado.Normal ? Tema.TextoTenue : Tema.ColorEstado(estado.Tipo);
        TextRenderer.DrawText(g, texto, fuente, posicion, color);
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
