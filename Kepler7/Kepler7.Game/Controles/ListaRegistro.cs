namespace Kepler7.Game.Controles;

internal sealed class ListaRegistro : ListBox
{
    private const int Margen = 6;
    private const int AltoMinimo = 18;

    private readonly List<string> _lineas = new();

    public ListaRegistro()
    {
        DrawMode = DrawMode.OwnerDrawVariable;
        BackColor = Tema.Panel;
        ForeColor = Tema.Texto;
        Font = new Font(Tema.FuenteMono, 9f);
        IntegralHeight = false;
        SelectionMode = SelectionMode.None;
    }

    public void AgregarLinea(string texto)
    {
        // Primero en la lista propia: cuando Windows pida medir el ítem, el texto ya está.
        _lineas.Add(texto);
        Items.Add(texto);
        TopIndex = Math.Max(0, Items.Count - 1);
    }

    public void Reemplazar(IEnumerable<string> lineas)
    {
        BeginUpdate();
        Items.Clear();
        _lineas.Clear();
        foreach (var linea in lineas)
        {
            _lineas.Add(linea);
            Items.Add(linea);
        }
        EndUpdate();
    }

    private string LineaEn(int indice)
    {
        string texto = "";
        if (indice >= 0 && indice < _lineas.Count)
        {
            texto = _lineas[indice];
        }
        return texto;
    }

    protected override void OnMeasureItem(MeasureItemEventArgs e)
    {
        int alto = AltoMinimo;
        string texto = LineaEn(e.Index);
        if (texto.Length > 0)
        {
            int ancho = Math.Max(50, Width - Margen * 2 - 18);
            var tamanio = TextRenderer.MeasureText(e.Graphics, texto, Font, new Size(ancho, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
            alto = Math.Min(255, tamanio.Height + 6);
        }
        e.ItemHeight = alto;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index >= 0)
        {
            string texto = LineaEn(e.Index);
            using (var fondo = new SolidBrush(e.Index % 2 == 0 ? Tema.Panel : Tema.Fila))
            {
                e.Graphics.FillRectangle(fondo, e.Bounds);
            }
            var area = new Rectangle(e.Bounds.X + Margen, e.Bounds.Y + 3, e.Bounds.Width - Margen * 2, e.Bounds.Height - 3);
            TextRenderer.DrawText(e.Graphics, texto, Font, area, ColorPara(texto),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
        }
    }

    private static Color ColorPara(string texto)
    {
        Color color = Tema.Texto;
        if (texto.StartsWith("═") || texto.StartsWith("──"))
        {
            color = Tema.Acento;
        }
        else if (texto.Contains("EMERGENCIA") || texto.StartsWith("✖") || texto.Contains("DERROTA"))
        {
            color = Tema.Peligro;
        }
        else if (texto.StartsWith("✔") || texto.StartsWith("★") || texto.StartsWith("✚"))
        {
            color = Tema.Exito;
        }
        else if (texto.StartsWith("↩") || texto.StartsWith("↪") || texto.StartsWith("⇄") || texto.StartsWith("⚠"))
        {
            color = Tema.Dorado;
        }
        else if (texto.StartsWith("☠"))
        {
            color = Tema.Veneno;
        }
        else if (texto.StartsWith("◈") || texto.StartsWith("["))
        {
            color = Tema.Escudo;
        }
        return color;
    }
}
