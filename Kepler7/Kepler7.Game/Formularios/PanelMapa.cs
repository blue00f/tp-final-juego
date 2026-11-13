using System.Drawing.Drawing2D;
using Kepler7.Core.Flyweight;

namespace Kepler7.Game.Formularios;

internal sealed class PanelMapa : Control
{
    private readonly RegistroTripulacion _registro;
    private readonly Font _fuente = Tema.Crear(8f, FontStyle.Bold);
    private Bitmap? _imagen;
    private IEspecie? _resaltada;

    public PanelMapa(RegistroTripulacion registro)
    {
        _registro = registro;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
    }

    public void Resaltar(IEspecie especie)
    {
        _resaltada = especie;
        _imagen?.Dispose();
        _imagen = null;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        _imagen ??= Dibujar();
        e.Graphics.DrawImageUnscaled(_imagen, 0, 0);
    }

    private Bitmap Dibujar()
    {
        var bmp = new Bitmap(Math.Max(1, Width), Math.Max(1, Height));
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Tema.Panel);

        float cx = Width / 2f;
        float cy = Height / 2f + 6;
        float radioExterno = Math.Min(Width, Height) / 2f - 20;
        float radioInterno = radioExterno * 0.42f;

        using (var anillo = new Pen(Tema.Borde, 1))
        {
            g.DrawEllipse(anillo, cx - radioExterno, cy - radioExterno, radioExterno * 2, radioExterno * 2);
            g.DrawEllipse(anillo, cx - radioInterno, cy - radioInterno, radioInterno * 2, radioInterno * 2);
            for (int s = 0; s < 12; s++)
            {
                double a = s * Math.PI / 6;
                g.DrawLine(anillo, cx + (float)Math.Cos(a) * radioInterno, cy + (float)Math.Sin(a) * radioInterno,
                    cx + (float)Math.Cos(a) * radioExterno, cy + (float)Math.Sin(a) * radioExterno);
            }
        }

        var pinceles = _registro.Fabrica.Especies.ToDictionary(e => e, e =>
        {
            var color = ColorTranslator.FromHtml(e.Color);
            int alfa = _resaltada == null || _resaltada == e ? 230 : 35;
            return new SolidBrush(Color.FromArgb(alfa, color));
        });

        foreach (var t in _registro.Tripulantes)
        {
            // Posición determinística: sector (estado extrínseco) + identificador.
            double angulo = (t.Sector + ((t.Identificador * 7919) % 1000) / 1000.0) * Math.PI / 6;
            double distancia = radioInterno + 6 + ((t.Identificador * 104729) % 1000) / 1000.0 * (radioExterno - radioInterno - 12);
            float x = cx + (float)(Math.Cos(angulo) * distancia);
            float y = cy + (float)(Math.Sin(angulo) * distancia);
            g.FillRectangle(pinceles[t.Especie], x, y, 2, 2);
        }
        foreach (var pincel in pinceles.Values)
        {
            pincel.Dispose();
        }

        TextRenderer.DrawText(g, "Núcleo", _fuente, new Rectangle((int)(cx - 40), (int)(cy - 10), 80, 20), Tema.TextoTenue,
            TextFormatFlags.HorizontalCenter);
        TextRenderer.DrawText(g, "10.000 tripulantes · 5 objetos especie", _fuente, new Point(10, 8), Tema.TextoTenue);
        using (var borde = new Pen(Tema.Borde))
        {
            g.DrawRectangle(borde, 0, 0, bmp.Width - 1, bmp.Height - 1);
        }
        return bmp;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _imagen?.Dispose();
            _fuente.Dispose();
        }
        base.Dispose(disposing);
    }
}
