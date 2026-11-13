namespace Kepler7.Game;

internal class FormBase : Form
{
    public FormBase()
    {
        BackColor = Tema.Fondo;
        ForeColor = Tema.Texto;
        Font = Tema.Crear(9f);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        DoubleBuffered = true;
        AutoScaleMode = AutoScaleMode.Dpi;
    }

    protected override void OnLoad(EventArgs e)
    {
        AplicarTema(this);
        base.OnLoad(e);
    }

    private static void AplicarTema(Control padre)
    {
        foreach (Control control in padre.Controls)
        {
            switch (control)
            {
                case TextBox:
                    control.BackColor = Tema.Panel;
                    control.ForeColor = Tema.Texto;
                    break;
                case ListBox lista:
                    lista.BackColor = Tema.Panel;
                    lista.ForeColor = Tema.Texto;
                    lista.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ComboBox combo:
                    combo.FlatStyle = FlatStyle.Flat;
                    combo.BackColor = Tema.PanelClaro;
                    combo.ForeColor = Tema.Texto;
                    break;
                case GroupBox:
                    control.ForeColor = Tema.Texto;
                    break;
            }
            AplicarTema(control);
        }
    }

    protected static Label CrearEtiqueta(string texto, float tamanio, Color color, FontStyle estilo = FontStyle.Regular)
    {
        return new Label
        {
            Text = texto,
            AutoSize = true,
            ForeColor = color,
            BackColor = Color.Transparent,
            Font = Tema.Crear(tamanio, estilo)
        };
    }

    protected static Label CrearTexto(string texto, float tamanio, Color color, Rectangle limites, FontStyle estilo = FontStyle.Regular)
    {
        var etiqueta = CrearEtiqueta(texto, tamanio, color, estilo);
        etiqueta.AutoSize = false;
        etiqueta.Bounds = limites;
        return etiqueta;
    }

    protected void AgregarEncabezado(string titulo, string patron, string explicacion)
    {
        var lblTitulo = CrearEtiqueta($"{titulo} ({patron})", 14f, Tema.Texto, FontStyle.Bold);
        lblTitulo.Location = new Point(24, 14);
        var lblExplicacion = CrearTexto(explicacion, 9f, Tema.TextoTenue, new Rectangle(26, 50, ClientSize.Width - 52, 50));
        Controls.AddRange(new Control[] { lblTitulo, lblExplicacion });
    }
}
