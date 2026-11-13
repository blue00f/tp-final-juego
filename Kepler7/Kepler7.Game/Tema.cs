using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Game;

internal static class Tema
{
    public static readonly Color Fondo = Color.FromArgb(32, 33, 36);
    public static readonly Color Panel = Color.FromArgb(43, 45, 49);
    public static readonly Color PanelClaro = Color.FromArgb(55, 58, 63);
    public static readonly Color Fila = Color.FromArgb(48, 50, 55);
    public static readonly Color Borde = Color.FromArgb(85, 89, 96);
    public static readonly Color Texto = Color.FromArgb(222, 224, 228);
    public static readonly Color TextoTenue = Color.FromArgb(150, 155, 163);
    public static readonly Color Acento = Color.FromArgb(110, 165, 230);
    public static readonly Color Dorado = Color.FromArgb(220, 165, 75);
    public static readonly Color Peligro = Color.FromArgb(225, 100, 100);
    public static readonly Color Exito = Color.FromArgb(100, 185, 125);
    public static readonly Color Veneno = Color.FromArgb(160, 200, 85);
    public static readonly Color Alien = Color.FromArgb(170, 130, 220);
    public static readonly Color Escudo = Color.FromArgb(110, 165, 230);

    public const string Fuente = "Segoe UI";
    public const string FuenteMono = "Consolas";

    public static Font Crear(float tamanio, FontStyle estilo = FontStyle.Regular) => new(Fuente, tamanio, estilo);

    public static Color ColorEstado(TipoEstado estado) => estado switch
    {
        TipoEstado.Envenenado => Veneno,
        TipoEstado.Aturdido => Dorado,
        TipoEstado.Berserk => Peligro,
        _ => Borde
    };

    public static string NombreRol(Rol rol) => rol switch
    {
        Rol.Comandante => "Capitán",
        Rol.Soldado => "Soldado",
        Rol.Medico => "Médica",
        _ => "Francotiradora"
    };

    public static Color ColorVida(double porcentaje) =>
        porcentaje > 0.5 ? Exito : porcentaje > 0.25 ? Dorado : Peligro;

    public static Color Mezclar(Color a, Color b, double t)
    {
        t = Math.Clamp(t, 0, 1);
        return Color.FromArgb(
            (int)(a.R + (b.R - a.R) * t),
            (int)(a.G + (b.G - a.G) * t),
            (int)(a.B + (b.B - a.B) * t));
    }

    public static void DibujarBarra(Graphics g, Rectangle r, double porcentaje, Color color, string texto, Font fuente)
    {
        using (var fondo = new SolidBrush(PanelClaro))
        {
            g.FillRectangle(fondo, r);
        }
        int ancho = (int)(r.Width * Math.Clamp(porcentaje, 0, 1));
        if (ancho > 0)
        {
            using var relleno = new SolidBrush(Mezclar(color, Panel, 0.45));
            g.FillRectangle(relleno, r.X, r.Y, ancho, r.Height);
        }
        using (var borde = new Pen(Borde))
        {
            g.DrawRectangle(borde, r.X, r.Y, r.Width - 1, r.Height - 1);
        }
        TextRenderer.DrawText(g, texto, fuente, r, Texto,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }
}
