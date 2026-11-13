using Kepler7.Core.Juego;
using Kepler7.Core.Progreso;
using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormCampania : FormBase
{
    public FormCampania(ProgresoJugador progreso)
    {
        Text = "Kepler-7 · Campaña completada";
        ClientSize = new Size(900, 740);

        var lblTitulo = CrearEtiqueta("Campaña completada: Kepler-7 está a salvo", 16f, Tema.Exito, FontStyle.Bold);
        lblTitulo.Location = new Point(24, 18);
        var lblSub = CrearEtiqueta("El Leviatán del Vacío cayó. Los seis subsistemas de la estación vuelven a estar en línea.", 10f, Tema.TextoTenue);
        lblSub.Location = new Point(26, 54);

        var historial = progreso.Historial;
        var datos = new (string Etiqueta, string Valor)[]
        {
            ("Misiones jugadas", historial.Count.ToString()),
            ("Victorias / derrotas", $"{historial.Count(h => h.Resultado == ResultadoCombate.Victoria)} / {historial.Count(h => h.Resultado == ResultadoCombate.Derrota)}"),
            ("Rondas de combate", historial.Sum(h => h.Rondas).ToString()),
            ("Daño infligido total", historial.Sum(h => h.DanioInfligido).ToString("N0")),
            ("XP total", progreso.Experiencia.ToString("N0")),
            ("Rango final", $"{progreso.Rango} (nivel {progreso.Nivel})")
        };
        var grupoDatos = new GroupBox { Text = "Resumen", Bounds = new Rectangle(24, 86, 400, 210) };
        for (int i = 0; i < datos.Length; i++)
        {
            var etiqueta = CrearEtiqueta($"{datos[i].Etiqueta}: {datos[i].Valor}", 10f, Tema.Texto);
            etiqueta.Location = new Point(14, 28 + i * 28);
            grupoDatos.Controls.Add(etiqueta);
        }

        var grupoSectores = new GroupBox { Text = "Sectores liberados", Bounds = new Rectangle(440, 86, 436, 210) };
        var lista = new ListBox { Bounds = new Rectangle(12, 24, 412, 172), BorderStyle = BorderStyle.None };
        foreach (var sector in CatalogoAliens.Sectores)
        {
            var victoria = historial.FirstOrDefault(h => h.Sector == sector.Numero && h.Resultado == ResultadoCombate.Victoria);
            lista.Items.Add(victoria == null
                ? $"{sector.Numero}. {sector.Alien} (pendiente)"
                : $"{sector.Numero}. {sector.Alien}: vencido en {victoria.Rondas} rondas");
        }
        grupoSectores.Controls.Add(lista);

        var lblLogros = CrearEtiqueta($"Patrones dominados: {progreso.PatronesDominados.Count}/12", 10f, Tema.Texto, FontStyle.Bold);
        lblLogros.Location = new Point(24, 312);
        var panel = new PanelLogros { Location = new Point(24, 338), Size = new Size(852, 330), Detallado = false };
        panel.Mostrar(progreso.PatronesDominados);

        var cerrar = new Boton("Volver a la estación", 200, 32) { Location = new Point(676, 692) };
        cerrar.Click += (_, _) => Close();
        Controls.AddRange(new Control[] { lblTitulo, lblSub, grupoDatos, grupoSectores, lblLogros, panel, cerrar });
    }
}
