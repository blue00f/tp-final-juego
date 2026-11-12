using Kepler7.Core.Progreso;

namespace Kepler7.Game.Controles;

internal sealed class PanelLogros : Control
{
    private readonly Font _familia = Tema.Crear(9f, FontStyle.Bold);
    private readonly Font _nombre = Tema.Crear(9.5f, FontStyle.Bold);
    private readonly Font _texto = Tema.Crear(7.5f);
    private IReadOnlyCollection<string> _dominados = Array.Empty<string>();
    private IReadOnlyCollection<string> _nuevos = Array.Empty<string>();

    public PanelLogros()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Tema.Fondo;
    }

    public bool Detallado { get; set; } = true;

    public void Mostrar(IReadOnlyCollection<string> dominados, IReadOnlyCollection<string>? nuevos = null)
    {
        _dominados = dominados;
        _nuevos = nuevos ?? Array.Empty<string>();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var familias = new[]
        {
            (FamiliaPatron.Creacional, "Creacionales"),
            (FamiliaPatron.Estructural, "Estructurales"),
            (FamiliaPatron.Comportamiento, "De comportamiento")
        };

        int columnas = 3;
        int separacion = 8;
        int anchoInsignia = (Width - separacion * (columnas - 1)) / columnas;
        int altoInsignia = Detallado ? 62 : 30;
        int y = 0;

        foreach (var (familia, titulo) in familias)
        {
            var patrones = Patrones.Todos.Where(p => p.Familia == familia).ToList();
            int dominadosFamilia = patrones.Count(p => _dominados.Contains(p.Nombre));
            TextRenderer.DrawText(g, $"{titulo} ({dominadosFamilia}/{patrones.Count})", _familia, new Point(0, y), Tema.Texto);
            y += 20;

            for (int i = 0; i < patrones.Count; i++)
            {
                int fila = i / columnas;
                int columna = i % columnas;
                var rect = new Rectangle(columna * (anchoInsignia + separacion), y + fila * (altoInsignia + separacion),
                    anchoInsignia - 1, altoInsignia);
                PintarInsignia(g, rect, patrones[i]);
            }
            int filas = (patrones.Count + columnas - 1) / columnas;
            y += filas * (altoInsignia + separacion) + 6;
        }
    }

    private void PintarInsignia(Graphics g, Rectangle rect, Patrones.InfoPatron patron)
    {
        bool dominado = _dominados.Contains(patron.Nombre);
        bool nuevo = _nuevos.Contains(patron.Nombre);
        Color color = nuevo ? Tema.Dorado : dominado ? Tema.Exito : Tema.Borde;

        using (var relleno = new SolidBrush(dominado ? Tema.Mezclar(Tema.Panel, color, 0.18) : Tema.Panel))
        using (var borde = new Pen(color, nuevo ? 2f : 1f))
        {
            g.FillRectangle(relleno, rect);
            g.DrawRectangle(borde, rect);
        }

        string marca = dominado ? "✓ " : "";
        TextRenderer.DrawText(g, marca + patron.Nombre, _nombre, new Point(rect.X + 8, rect.Y + 6),
            dominado ? Tema.Texto : Tema.TextoTenue);
        if (Detallado)
        {
            string texto = dominado ? patron.Mecanica : patron.ComoSeDomina;
            TextRenderer.DrawText(g, texto, _texto, new Rectangle(rect.X + 8, rect.Y + 26, rect.Width - 14, rect.Height - 28),
                Tema.TextoTenue, TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _familia.Dispose();
            _nombre.Dispose();
            _texto.Dispose();
        }
        base.Dispose(disposing);
    }
}
