using System.Drawing.Drawing2D;
using Kepler7.Core.Modelo;
using Kepler7.Core.State;

namespace Kepler7.Game.Controles;

internal sealed class TarjetaAlien : TarjetaBase
{
    private readonly Font _fuenteNombre = Tema.Crear(11f, FontStyle.Bold);
    private readonly Font _fuenteChica = Tema.Crear(8f);
    private readonly Color _color;

    public TarjetaAlien(Enemigo enemigo) : base(enemigo)
    {
        Enemigo = enemigo;
        _color = ColorTranslator.FromHtml(enemigo.Apariencia.Color);
        Size = new Size(280, 300);
    }

    public Enemigo Enemigo { get; }

    public void Embestir() => Destellar(Tema.Alien);

    protected override void PintarContenido(Graphics g, Rectangle area)
    {
        TextRenderer.DrawText(g, Enemigo.Nombre, _fuenteNombre, new Rectangle(area.X, area.Y, area.Width, 22),
            Tema.Texto, TextFormatFlags.HorizontalCenter);

        PintarCriatura(g, new Rectangle(area.X + 30, area.Y + 30, area.Width - 60, 150));

        var barra = new Rectangle(area.X, area.Y + 190, area.Width, 16);
        Tema.DibujarBarra(g, barra, Enemigo.PorcentajeVida, Tema.Peligro, $"Vida {Enemigo.Vida} / {Enemigo.VidaMaxima}", _fuenteChica);

        int escudo = Enemigo.PuntosEscudo;
        if (escudo > 0)
        {
            TextRenderer.DrawText(g, $"Caparazón: {escudo}", _fuenteChica, new Point(area.X, area.Y + 210), Tema.Escudo);
        }
        PintarEstado(g, new Point(area.X, area.Y + 226), _fuenteChica);

        string habilidades =
            $"Ácido: {Enfriamiento(TipoAccion.Envenenar)}   Rugido: {Enfriamiento(TipoAccion.Aturdir)}   Caparazón: {Enfriamiento(TipoAccion.Defender)}";
        TextRenderer.DrawText(g, habilidades, _fuenteChica, new Point(area.X, area.Y + 246), Tema.TextoTenue);
    }

    private string Enfriamiento(TipoAccion habilidad)
    {
        int rondas = Enemigo.RondasDeEnfriamiento(habilidad);
        return rondas == 0 ? "lista" : $"{rondas} r";
    }

    private void PintarCriatura(Graphics g, Rectangle area)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var apariencia = Enemigo.Apariencia;
        float escala = Math.Min((float)apariencia.Escala, 1.15f);
        float cx = area.X + area.Width / 2f;
        float cy = area.Y + area.Height / 2f - 10;
        float ancho = 110 * escala;
        float alto = 90 * escala;

        Color piel = Enemigo.Estado.Tipo switch
        {
            TipoEstado.Envenenado => Tema.Mezclar(_color, Tema.Veneno, 0.5),
            TipoEstado.Aturdido => Tema.Mezclar(_color, Tema.Dorado, 0.4),
            TipoEstado.Berserk => Tema.Mezclar(_color, Tema.Peligro, 0.4),
            _ => _color
        };
        Color contorno = Tema.Mezclar(piel, Color.Black, 0.45);

        using (var lapiz = new Pen(contorno, 3))
        {
            int patas = Math.Max(1, apariencia.Tentaculos);
            for (int i = 0; i < patas; i++)
            {
                float px = cx - ancho / 2 + 10 + i * (ancho - 20) / Math.Max(1, patas - 1);
                g.DrawLine(lapiz, px, cy + alto / 3, px + (px < cx ? -8 : 8), cy + alto / 2 + 16);
            }
        }

        var cuerpo = new RectangleF(cx - ancho / 2, cy - alto / 2, ancho, alto);
        using (var pincel = new SolidBrush(piel))
        using (var lapiz = new Pen(contorno, 2))
        {
            g.FillEllipse(pincel, cuerpo);
            g.DrawEllipse(lapiz, cuerpo);
        }

        int ojos = Math.Max(1, apariencia.Ojos);
        float separacion = Math.Min(22f, 70f / ojos);
        using (var blanco = new SolidBrush(Color.White))
        using (var negro = new SolidBrush(Color.Black))
        {
            for (int n = 0; n < ojos; n++)
            {
                float ox = cx + (n - (ojos - 1) / 2f) * separacion;
                float oy = cy - alto / 6;
                g.FillEllipse(blanco, ox - 7, oy - 7, 14, 14);
                g.FillEllipse(negro, ox - 3, oy - 3, 6, 6);
            }
        }

        using (var lapiz = new Pen(contorno, 2))
        {
            g.DrawArc(lapiz, cx - 22, cy, 44, 18, 10, 160);
        }

        if (Enemigo.PuntosEscudo > 0)
        {
            using var lapiz = new Pen(Tema.Escudo, 2) { DashStyle = DashStyle.Dash };
            g.DrawEllipse(lapiz, cuerpo.X - 8, cuerpo.Y - 8, cuerpo.Width + 16, cuerpo.Height + 16);
        }
        g.SmoothingMode = SmoothingMode.Default;
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
