using Kepler7.Core.Progreso;
using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormLogros : FormBase
{
    public FormLogros(ProgresoJugador progreso)
    {
        Text = "Kepler-7 · Logros";
        ClientSize = new Size(820, 600);

        var titulo = CrearEtiqueta("Patrones dominados", 14f, Tema.Texto, FontStyle.Bold);
        titulo.Location = new Point(24, 18);
        var sub = CrearEtiqueta($"{progreso.PatronesDominados.Count} de 12 · los patrones de la estación se desbloquean venciendo aliens",
            9.5f, Tema.TextoTenue);
        sub.Location = new Point(26, 54);

        var panel = new PanelLogros { Location = new Point(24, 86), Size = new Size(772, 440) };
        panel.Mostrar(progreso.PatronesDominados);

        var cerrar = new Boton("Cerrar", 120, 32) { Location = new Point(676, 546) };
        cerrar.Click += (_, _) => Close();

        Controls.AddRange(new Control[] { titulo, sub, panel, cerrar });
    }
}
