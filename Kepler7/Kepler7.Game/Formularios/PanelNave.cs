using System.Drawing.Drawing2D;
using Kepler7.Core.Builder;

namespace Kepler7.Game.Formularios;

internal sealed class PanelNave : Control
{
    private static readonly string[] Orden = { "Casco", "Motor", "Escudo", "Armas", "Radar", "IA", "Tanques" };

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 400 };
    private readonly Font _fuente = Tema.Crear(8.5f);
    private readonly Font _fuenteTitulo = Tema.Crear(11f, FontStyle.Bold);
    private Nave? _nave;
    private int _visibles;

    public PanelNave()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        _timer.Tick += (_, _) =>
        {
            if (_nave != null && _visibles < _nave.Componentes.Count)
            {
                _visibles++;
            }
            else
            {
                _timer.Stop();
            }
            Invalidate();
        };
    }

    public void Mostrar(Nave nave, bool animar)
    {
        _nave = nave;
        _visibles = animar ? 0 : nave.Componentes.Count;
        if (animar)
        {
            _timer.Start();
        }
        Invalidate();
    }

    private bool Instalada(string tipo)
    {
        int indice = _nave?.Componentes.ToList().FindIndex(c => c.Tipo == tipo) ?? -1;
        return indice >= 0 && indice < _visibles;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Tema.Panel);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        string titulo = _nave == null ? "Plano vacío: elegí receta y plan" : $"Nave {_nave.Nombre}";
        TextRenderer.DrawText(g, titulo, _fuenteTitulo, new Point(14, 10), Tema.Texto);

        float cx = (Width - 240) / 2f + 10;
        float cy = Height / 2f + 10;
        var estadoGrafico = g.Save();
        g.TranslateTransform(cx, cy);
        g.ScaleTransform(0.88f, 0.88f);
        g.TranslateTransform(-cx, -cy);
        Color color = _nave?.Receta switch
        {
            "Combate" => Color.FromArgb(160, 95, 95),
            "Carga" => Color.FromArgb(95, 140, 100),
            _ => Color.FromArgb(90, 120, 165)
        };
        using var contorno = new Pen(Tema.Texto, 1.5f);
        using var relleno = new SolidBrush(color);

        Pieza(g, "Tanques", new RectangleF(cx - 70, cy + 38, 144, 22), r =>
        {
            for (int i = 0; i < 3; i++)
            {
                g.FillEllipse(relleno, cx - 70 + i * 50, cy + 38, 44, 22);
                g.DrawEllipse(contorno, cx - 70 + i * 50, cy + 38, 44, 22);
            }
        });
        Pieza(g, "Motor", new RectangleF(cx - 190, cy - 22, 40, 44), r =>
        {
            g.FillRectangle(relleno, r);
            g.DrawRectangle(contorno, r.X, r.Y, r.Width, r.Height);
        });
        Pieza(g, "Casco", new RectangleF(cx - 150, cy - 34, 320, 68), r =>
        {
            var casco = new[]
            {
                new PointF(cx - 150, cy - 34), new PointF(cx + 90, cy - 26), new PointF(cx + 170, cy),
                new PointF(cx + 90, cy + 26), new PointF(cx - 150, cy + 34)
            };
            g.FillPolygon(relleno, casco);
            g.DrawPolygon(contorno, casco);
        });
        Pieza(g, "IA", new RectangleF(cx + 40, cy - 12, 24, 24), r =>
        {
            g.FillEllipse(Brushes.LightGray, r);
            g.DrawEllipse(contorno, r);
        });
        Pieza(g, "Armas", new RectangleF(cx - 20, cy - 52, 110, 104), r =>
        {
            g.FillRectangle(Brushes.Gray, cx - 20, cy - 52, 110, 10);
            g.FillRectangle(Brushes.Gray, cx - 20, cy + 42, 110, 10);
        });
        Pieza(g, "Radar", new RectangleF(cx - 84, cy - 92, 48, 58), r =>
        {
            g.DrawLine(contorno, cx - 60, cy - 34, cx - 60, cy - 70);
            g.DrawArc(contorno, cx - 84, cy - 92, 48, 30, 200, 140);
        });
        Pieza(g, "Escudo", new RectangleF(cx - 220, cy - 100, 420, 200), r =>
        {
            using var lapiz = new Pen(Tema.Escudo, 2) { DashStyle = DashStyle.Dash };
            g.DrawEllipse(lapiz, r);
        });

        g.Restore(estadoGrafico);

        int ly = 50;
        foreach (var tipo in Orden)
        {
            var componente = _nave?.Componentes.FirstOrDefault(c => c.Tipo == tipo);
            bool visible = Instalada(tipo);
            Color c = componente == null ? Tema.Borde : visible ? Tema.Texto : Tema.TextoTenue;
            string texto = componente == null ? $"{tipo}: —" : visible ? $"{tipo}: {componente.Nombre}\n   {componente.Efecto}" : $"{tipo}: ensamblando…";
            TextRenderer.DrawText(g, texto, _fuente, new Rectangle(Width - 230, ly, 222, 44), c, TextFormatFlags.WordBreak);
            ly += 48;
        }

        using var borde = new Pen(Tema.Borde);
        g.DrawRectangle(borde, 0, 0, Width - 1, Height - 1);
    }

    private void Pieza(Graphics g, string tipo, RectangleF area, Action<RectangleF> dibujar)
    {
        if (Instalada(tipo))
        {
            dibujar(area);
        }
        else
        {
            using var hueco = new Pen(Tema.Borde) { DashStyle = DashStyle.Dot };
            g.DrawRectangle(hueco, area.X, area.Y, area.Width, area.Height);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
            _fuente.Dispose();
            _fuenteTitulo.Dispose();
        }
        base.Dispose(disposing);
    }
}
