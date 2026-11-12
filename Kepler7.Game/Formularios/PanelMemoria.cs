using System.Drawing.Drawing2D;
using Kepler7.Core.Flyweight;

namespace Kepler7.Game.Formularios;

internal sealed class PanelMemoria : Control
{
    private readonly Font _fuente = Tema.Crear(8.5f);
    private ComparacionMemoria? _comparacion;

    public PanelMemoria()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
    }

    public void Mostrar(ComparacionMemoria comparacion)
    {
        _comparacion = comparacion;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Tema.Panel);
        if (_comparacion == null)
        {
            TextRenderer.DrawText(g, "Presioná Comparar: se crean 10.000 copias reales de la especie y se mide la memoria.",
                _fuente, new Rectangle(8, 8, Width - 16, Height - 16), Tema.TextoTenue, TextFormatFlags.WordBreak);
        }
        else
        {
            var c = _comparacion;
            int anchoBarra = Width - 16;
            int anchoCon = Math.Max(3, (int)(anchoBarra * (double)c.BytesConFlyweight / c.BytesSinFlyweight));
            Tema.DibujarBarra(g, new Rectangle(8, 8, anchoCon, 18), 1, Tema.Exito, "", _fuente);
            TextRenderer.DrawText(g, $"Con Flyweight: {c.ObjetosEspecieConFlyweight} objetos especie · {Formatear(c.BytesConFlyweight)}",
                _fuente, new Point(anchoCon + 14, 9), Tema.Texto);
            Tema.DibujarBarra(g, new Rectangle(8, 34, anchoBarra, 18), 1, Tema.Peligro,
                $"Sin Flyweight: {c.ObjetosEspecieSinFlyweight:N0} objetos especie · {Formatear(c.BytesSinFlyweight)}", _fuente);
            TextRenderer.DrawText(g, $"Ahorro de memoria: {c.Ahorro:P2}", _fuente, new Point(8, 62), Tema.Texto);
        }
    }

    private static string Formatear(long bytes) =>
        bytes >= 1024 * 1024 ? $"{bytes / 1024.0 / 1024.0:F1} MB" : $"{bytes / 1024.0:F1} KB";

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fuente.Dispose();
        }
        base.Dispose(disposing);
    }
}
