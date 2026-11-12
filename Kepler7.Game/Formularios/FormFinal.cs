using Kepler7.Core.Juego;
using Kepler7.Core.Progreso;
using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormFinal : FormBase
{
    private readonly ResumenCombate _resumen;
    private readonly ProgresoJugador _progreso;
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 30 };
    private readonly ProgressBar _barraXp = new();
    private readonly Label _lblNivel;
    private double _xpAnimada;

    public FormFinal(ResumenCombate resumen, ProgresoJugador progreso, IReadOnlyCollection<string> patronesNuevos)
    {
        _resumen = resumen;
        _progreso = progreso;
        _xpAnimada = progreso.Experiencia - resumen.XpGanada;

        Text = "Kepler-7 · Resumen de la misión";
        ClientSize = new Size(820, 680);

        bool victoria = resumen.Resultado == ResultadoCombate.Victoria;
        var lblResultado = CrearEtiqueta(victoria ? "Victoria" : "Derrota", 20f, victoria ? Tema.Exito : Tema.Peligro, FontStyle.Bold);
        lblResultado.Location = new Point(24, 16);
        string subtitulo = victoria
            ? $"{resumen.Enemigo} neutralizado. {CatalogoAliens.Info(resumen.Sector).Recompensa}."
            : $"El Capitán cayó ante {resumen.Enemigo}. La estación sigue en peligro.";
        var lblSub = CrearEtiqueta(subtitulo, 10f, Tema.TextoTenue);
        lblSub.Location = new Point(26, 58);

        var datos = new (string Etiqueta, string Valor)[]
        {
            ("Rondas", resumen.Rondas.ToString()),
            ("Daño infligido", resumen.DanioInfligido.ToString()),
            ("Daño recibido", resumen.DanioRecibido.ToString()),
            ("Curación", resumen.CuracionTotal.ToString()),
            ("Órdenes ejecutadas", resumen.ComandosEjecutados.ToString()),
            ("Órdenes deshechas", resumen.AccionesDeshechas.ToString()),
            ("Bajas propias", resumen.BajasPropias.ToString()),
            ("XP ganada", $"+{resumen.XpGanada}")
        };
        var grupo = new GroupBox { Text = "Resumen del combate", Bounds = new Rectangle(24, 90, 772, 200) };
        for (int i = 0; i < datos.Length; i++)
        {
            var etiqueta = CrearEtiqueta($"{datos[i].Etiqueta}: {datos[i].Valor}", 10f, Tema.Texto);
            etiqueta.Location = new Point(16 + (i % 2) * 380, 30 + (i / 2) * 26);
            grupo.Controls.Add(etiqueta);
        }
        _lblNivel = CrearEtiqueta("", 10f, Tema.Texto, FontStyle.Bold);
        _lblNivel.Location = new Point(16, 140);
        _barraXp.SetBounds(16, 166, 740, 18);
        _barraXp.Maximum = ProgresoJugador.XpPorNivel;
        grupo.Controls.AddRange(new Control[] { _lblNivel, _barraXp });

        var lblLogros = CrearEtiqueta(patronesNuevos.Count > 0
                ? $"Nuevos patrones dominados: {string.Join(", ", patronesNuevos)}"
                : "Logros", 10f, Tema.Texto, FontStyle.Bold);
        lblLogros.Location = new Point(24, 304);

        var panel = new PanelLogros { Location = new Point(24, 330), Size = new Size(772, 290), Detallado = false };
        panel.Mostrar(progreso.PatronesDominados, patronesNuevos);

        var volver = new Boton("Volver a la estación", 200, 32) { Location = new Point(596, 636) };
        volver.Click += (_, _) => Close();

        Controls.AddRange(new Control[] { lblResultado, lblSub, grupo, lblLogros, panel, volver });

        MostrarXp();
        _timer.Tick += (_, _) => AvanzarXp();
        Shown += (_, _) => _timer.Start();
    }

    private void AvanzarXp()
    {
        if (_xpAnimada < _progreso.Experiencia)
        {
            _xpAnimada = Math.Min(_progreso.Experiencia, _xpAnimada + Math.Max(1, _resumen.XpGanada / 60.0));
            MostrarXp();
        }
        else
        {
            _timer.Stop();
        }
    }

    private void MostrarXp()
    {
        int xp = (int)_xpAnimada;
        int nivel = 1 + xp / ProgresoJugador.XpPorNivel;
        int enNivel = xp % ProgresoJugador.XpPorNivel;
        _lblNivel.Text = $"Nivel {nivel} · {_progreso.Rango} · {enNivel} / {ProgresoJugador.XpPorNivel} XP";
        _barraXp.Value = enNivel;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
        }
        base.Dispose(disposing);
    }
}
