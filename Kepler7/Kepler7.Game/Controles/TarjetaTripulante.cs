using System.Drawing.Drawing2D;
using Kepler7.Core.Modelo;

namespace Kepler7.Game.Controles;

internal sealed class TarjetaTripulante : TarjetaBase
{
    private readonly Font _fuenteNombre = Tema.Crear(9.5f, FontStyle.Bold);
    private readonly Font _fuenteChica = Tema.Crear(8f);

    public TarjetaTripulante(MiembroEscuadron miembro) : base(miembro)
    {
        Miembro = miembro;
        Size = new Size(196, 100);
    }

    public MiembroEscuadron Miembro { get; }

    protected override void PintarContenido(Graphics g, Rectangle area)
    {
        int x = area.X;
        string nombre = Miembro.EsCapitan ? $"{Miembro.Nombre} (Capitán)" : Miembro.Nombre;
        TextRenderer.DrawText(g, nombre, _fuenteNombre, new Point(x, area.Y), Tema.Texto);
        TextRenderer.DrawText(g, Tema.NombreRol(Miembro.Rol), _fuenteChica, new Point(x, area.Y + 18), Tema.TextoTenue);

        Tema.DibujarBarra(g, new Rectangle(x, area.Y + 36, area.Width, 14), Miembro.PorcentajeVida,
            Tema.ColorVida(Miembro.PorcentajeVida), $"Vida {Miembro.Vida} / {Miembro.VidaMaxima}", _fuenteChica);

        int escudo = Miembro.PuntosEscudo;
        string extra = escudo > 0 ? $"Escudo: {escudo}" : "";
        if (Miembro.YaActuo && Miembro.EstaVivo)
        {
            extra = (extra.Length > 0 ? extra + "  ·  " : "") + "ya actuó";
        }
        TextRenderer.DrawText(g, extra, _fuenteChica, new Point(x, area.Y + 54), Tema.Escudo);
        PintarEstado(g, new Point(x, area.Y + 70), _fuenteChica);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fuenteNombre.Dispose();
            _fuenteChica.Dispose();
        }
        base.Dispose(disposing);
    }
}
