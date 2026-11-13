namespace Kepler7.Game.Formularios;

internal sealed class PanelCampo : Panel
{
    private readonly Font _fuente = Tema.Crear(8.5f, FontStyle.Bold);
    private bool _alerta;

    public PanelCampo()
    {
        DoubleBuffered = true;
        BackColor = Tema.PanelClaro;
    }

    public bool Alerta
    {
        get => _alerta;
        set { _alerta = value; Invalidate(); }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        TextRenderer.DrawText(g, "Retaguardia", _fuente, new Point(20, 12), Tema.TextoTenue);
        TextRenderer.DrawText(g, "Vanguardia", _fuente, new Point(236, 12), Tema.TextoTenue);

        using (var separador = new Pen(Tema.Borde) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
        {
            g.DrawLine(separador, 466, 20, 466, Height - 20);
        }

        using (var lapiz = new Pen(_alerta ? Tema.Peligro : Tema.Borde, _alerta ? 3 : 1))
        {
            g.DrawRectangle(lapiz, 1, 1, Width - 3, Height - 3);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fuente.Dispose();
        }
        base.Dispose(disposing);
    }
}
