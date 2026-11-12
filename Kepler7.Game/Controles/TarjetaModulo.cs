namespace Kepler7.Game.Controles;

internal sealed class TarjetaModulo : Control
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 50 };
    private readonly Font _fuenteTitulo = Tema.Crear(10.5f, FontStyle.Bold);
    private readonly Font _fuenteTexto = Tema.Crear(8.5f);
    private bool _encima;
    private int _destello;

    public TarjetaModulo(string titulo, string patron)
    {
        Titulo = titulo;
        Patron = patron;
        Size = new Size(256, 212);
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        _timer.Tick += (_, _) =>
        {
            _destello--;
            if (_destello <= 0)
            {
                _timer.Stop();
            }
            Invalidate();
        };
    }

    public string Titulo { get; }
    public string Patron { get; }

    public bool EnLinea { get; set; } = true;

    public string Requisito { get; set; } = "";

    public IReadOnlyList<string> Detalle { get; set; } = Array.Empty<string>();

    public void Destellar()
    {
        _destello = 30;
        _timer.Start();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _encima = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _encima = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        Color fondo = !EnLinea ? Tema.Fondo : _encima ? Tema.PanelClaro : Tema.Panel;
        if (_destello > 0)
        {
            fondo = Tema.Mezclar(fondo, Tema.Exito, _destello / 90.0);
        }
        g.Clear(fondo);
        using (var borde = new Pen(_encima && EnLinea ? Tema.Acento : Tema.Borde))
        {
            g.DrawRectangle(borde, 0, 0, Width - 1, Height - 1);
        }

        TextRenderer.DrawText(g, $"{Titulo} ({Patron})", _fuenteTitulo, new Point(12, 10), EnLinea ? Tema.Texto : Tema.TextoTenue);
        TextRenderer.DrawText(g, EnLinea ? "En línea" : "Fuera de servicio", _fuenteTexto, new Point(12, 32),
            EnLinea ? Tema.Exito : Tema.Peligro);

        var area = new Rectangle(12, 56, Width - 24, Height - 64);
        string texto = EnLinea ? string.Join("\n", Detalle) : Requisito;
        TextRenderer.DrawText(g, texto, _fuenteTexto, area, EnLinea ? Tema.Texto : Tema.TextoTenue,
            TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
            _fuenteTitulo.Dispose();
            _fuenteTexto.Dispose();
        }
        base.Dispose(disposing);
    }
}
