using System.Drawing.Drawing2D;
using Kepler7.Core.ChainOfResponsibility;

namespace Kepler7.Game.Controles;

internal sealed class PanelCadena : Control
{
    private readonly Font _titulo = Tema.Crear(9f, FontStyle.Bold);
    private readonly Font _eslabon = Tema.Crear(9f, FontStyle.Bold);
    private readonly Font _valor = Tema.Crear(11f, FontStyle.Bold);
    private readonly Font _detalle = Tema.Crear(7.5f);
    private Danio? _danio;

    public PanelCadena()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Tema.Panel;
    }

    public void Mostrar(Danio? danio)
    {
        if (!ReferenceEquals(danio, _danio))
        {
            _danio = danio;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using (var lapiz = new Pen(Tema.Borde))
        {
            g.DrawRectangle(lapiz, 0, 0, Width - 1, Height - 1);
        }

        if (_danio == null)
        {
            TextRenderer.DrawText(g, "Cadena de defensas: todavía no hubo daño", _titulo, new Point(10, 8), Tema.TextoTenue);
        }
        else
        {
            PintarRecorrido(g, _danio);
        }
    }

    private void PintarRecorrido(Graphics g, Danio danio)
    {
        string titulo = $"Cadena de defensas: {danio.Origen} → {danio.Objetivo}: {danio.Original} de daño entrante";
        TextRenderer.DrawText(g, titulo, _titulo, new Point(10, 8), Tema.Texto);

        var pasos = danio.Recorrido;
        int n = pasos.Count;
        int separacion = 22;
        int ancho = Math.Min(140, (Width - 20 - separacion * (n - 1)) / Math.Max(1, n));
        int alto = Height - 44;
        int x = 10;
        int y = 32;

        for (int i = 0; i < n; i++)
        {
            var paso = pasos[i];
            var caja = new Rectangle(x, y, ancho, alto);
            Color color = ColorPaso(paso);

            using (var relleno = new SolidBrush(Tema.Mezclar(Tema.Panel, color, 0.15)))
            using (var borde = new Pen(color))
            {
                g.FillRectangle(relleno, caja);
                g.DrawRectangle(borde, caja);
            }

            TextRenderer.DrawText(g, paso.Eslabon, _eslabon, new Rectangle(caja.X, caja.Y + 4, caja.Width, 18),
                Tema.Texto, TextFormatFlags.HorizontalCenter);
            string valor = paso.Absorbido > 0 ? $"-{paso.Absorbido}" : paso.Absorbido < 0 ? $"+{-paso.Absorbido}" : "→";
            TextRenderer.DrawText(g, valor, _valor, new Rectangle(caja.X, caja.Y + 22, caja.Width, 22),
                color, TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, paso.Detalle, _detalle, new Rectangle(caja.X + 3, caja.Y + 44, caja.Width - 6, caja.Height - 46),
                Tema.TextoTenue, TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);

            if (i < n - 1)
            {
                int ax = caja.Right + 3;
                int ay = caja.Y + caja.Height / 2;
                using var flecha = new Pen(Tema.TextoTenue, 2) { CustomEndCap = new AdjustableArrowCap(4, 4) };
                g.DrawLine(flecha, ax, ay, ax + separacion - 7, ay);
            }
            x += ancho + separacion;
        }
    }

    private static Color ColorPaso(PasoDanio paso)
    {
        Color color;
        if (paso.Eslabon == "Vida")
        {
            color = paso.Absorbido > 0 ? Tema.Peligro : Tema.Exito;
        }
        else if (paso.Absorbido < 0)
        {
            color = Tema.Peligro;
        }
        else if (paso.Absorbido > 0)
        {
            color = paso.Eslabon == "Escudo" ? Tema.Escudo : Tema.Exito;
        }
        else
        {
            color = Tema.TextoTenue;
        }
        return color;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _titulo.Dispose();
            _eslabon.Dispose();
            _valor.Dispose();
            _detalle.Dispose();
        }
        base.Dispose(disposing);
    }
}
