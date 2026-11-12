using Kepler7.Core.Modelo;
using Kepler7.Core.Prototype;

namespace Kepler7.Game.Formularios;

internal sealed class PanelComparacion : Control
{
    private readonly Font _fuenteCabecera = Tema.Crear(9f, FontStyle.Bold);
    private readonly Font _fuente = Tema.Crear(9f);
    private IReadOnlyList<DiferenciaClon> _filas = Array.Empty<DiferenciaClon>();

    public PanelComparacion()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        BackColor = Tema.Panel;
    }

    public void Mostrar(MiembroEscuadron original, MiembroEscuadron? clon)
    {
        _filas = clon == null
            ? LaboratorioBiotecnologia.Comparar(original, original)
                .Select(f => f with { Clon = "—" }).ToList()
            : LaboratorioBiotecnologia.Comparar(original, clon);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Tema.Panel);
        int colAtributo = 10;
        int colOriginal = 180;
        int colClon = 420;
        int ancho = (Width - colClon) - 10;

        TextRenderer.DrawText(g, "Atributo", _fuenteCabecera, new Point(colAtributo, 10), Tema.Texto);
        TextRenderer.DrawText(g, "Original", _fuenteCabecera, new Point(colOriginal, 10), Tema.Texto);
        TextRenderer.DrawText(g, "Clon", _fuenteCabecera, new Point(colClon, 10), Tema.Texto);

        int y = 36;
        int i = 0;
        foreach (var fila in _filas)
        {
            int alto = fila.Atributo is "Habilidades" or "Cadena de defensas" ? 64 : 34;
            if (i % 2 == 0)
            {
                using var fondo = new SolidBrush(Tema.Fila);
                g.FillRectangle(fondo, 0, y - 4, Width, alto);
            }
            bool distinto = fila.Distinto && fila.Clon != "—";
            if (distinto)
            {
                using var resalte = new SolidBrush(Tema.Mezclar(Tema.Panel, Tema.Exito, 0.2));
                g.FillRectangle(resalte, colClon - 6, y - 4, ancho + 6, alto);
            }
            var flags = TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis;
            TextRenderer.DrawText(g, fila.Atributo, _fuenteCabecera, new Rectangle(colAtributo, y, colOriginal - colAtributo - 8, alto - 4), Tema.Texto, flags);
            TextRenderer.DrawText(g, fila.Original, _fuente, new Rectangle(colOriginal, y, colClon - colOriginal - 12, alto - 4), Tema.Texto, flags);
            TextRenderer.DrawText(g, fila.Clon, _fuente, new Rectangle(colClon, y, ancho, alto - 4), distinto ? Tema.Exito : Tema.Texto, flags);
            y += alto;
            i++;
        }

        using var borde = new Pen(Tema.Borde);
        g.DrawRectangle(borde, 0, 0, Width - 1, Height - 1);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fuenteCabecera.Dispose();
            _fuente.Dispose();
        }
        base.Dispose(disposing);
    }
}
