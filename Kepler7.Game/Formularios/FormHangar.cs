using System.Drawing.Drawing2D;
using Kepler7.Core.Builder;
using Kepler7.Core.Juego;
using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormHangar : FormBase
{
    private readonly Partida _partida;
    private readonly List<Opcion> _recetas = new();
    private readonly List<Opcion> _planes = new();
    private readonly Label _lblReceta;
    private readonly Label _lblPlan;
    private readonly PanelNave _panelNave = new();
    private readonly ListaRegistro _bitacora = new();
    private readonly Label _lblResultado;

    private static readonly (PlanEnsamblaje Plan, string Descripcion)[] Planes =
    {
        (PlanEnsamblaje.Completo, "Los 7 componentes: casco, motor, escudo, armas, radar, IA y tanques."),
        (PlanEnsamblaje.Asalto, "Solo lo indispensable para pelear: casco, motor, escudo y armas."),
        (PlanEnsamblaje.Reconocimiento, "Sin escudo ni armas: casco, motor, radar, IA y tanques.")
    };

    public FormHangar(Partida partida)
    {
        _partida = partida;
        Text = "Kepler-7 · Hangar";
        ClientSize = new Size(1080, 680);
        AgregarEncabezado("Hangar de naves", "Builder",
            "INaveBuilder define un paso por componente. Cada receta (RecetaExploradora, RecetaCombate, RecetaCarga) es un " +
            "constructor concreto; el IngenieroJefe dirige el orden y decide qué pasos ejecutar según el plan.");

        Controls.Add(Etiqueta("1. Receta (constructor concreto)", 24, 112));
        var grupoRecetas = new Panel { Bounds = new Rectangle(24, 136, 330, 36) };
        int x = 0;
        foreach (var crear in CatalogoRecetas.Todas)
        {
            var receta = crear();
            var opcion = new Opcion(receta.NombreReceta, AnchoOpcion(receta.NombreReceta), 34) { Location = new Point(x, 0), Tag = crear };
            opcion.CheckedChanged += (_, _) => ActualizarDescripciones();
            _recetas.Add(opcion);
            grupoRecetas.Controls.Add(opcion);
            x += opcion.Width + 6;
        }
        Controls.Add(grupoRecetas);
        _lblReceta = CrearTexto("", 9f, Tema.Texto, new Rectangle(24, 176, 330, 48));
        Controls.Add(_lblReceta);

        Controls.Add(Etiqueta("2. Plan del Ingeniero Jefe (director)", 24, 232));
        var grupoPlanes = new Panel { Bounds = new Rectangle(24, 256, 330, 36) };
        x = 0;
        foreach (var (plan, _) in Planes)
        {
            var opcion = new Opcion(plan.ToString(), AnchoOpcion(plan.ToString()), 34) { Location = new Point(x, 0), Tag = plan };
            opcion.CheckedChanged += (_, _) => ActualizarDescripciones();
            _planes.Add(opcion);
            grupoPlanes.Controls.Add(opcion);
            x += opcion.Width + 6;
        }
        Controls.Add(grupoPlanes);
        _lblPlan = CrearTexto("", 9f, Tema.Texto, new Rectangle(24, 296, 330, 40));
        Controls.Add(_lblPlan);

        var btnEnsamblar = new Boton("Ensamblar nave", 160, 32) { Location = new Point(24, 350) };
        btnEnsamblar.Click += (_, _) => Ensamblar();
        Controls.Add(btnEnsamblar);

        Controls.Add(Etiqueta("Bitácora del Ingeniero Jefe", 24, 402));
        _bitacora.SetBounds(24, 424, 330, 190);
        Controls.Add(_bitacora);

        _panelNave.SetBounds(378, 112, 678, 400);
        Controls.Add(_panelNave);

        _lblResultado = CrearTexto("", 9.5f, Tema.Texto, new Rectangle(378, 522, 678, 92));
        Controls.Add(_lblResultado);

        var cerrar = new Boton("Volver a la estación", 180, 32) { Location = new Point(876, 634) };
        cerrar.Click += (_, _) => Close();
        Controls.Add(cerrar);

        var actual = partida.Preparativos.Nave;
        (_recetas.FirstOrDefault(o => ((Func<INaveBuilder>)o.Tag!)().NombreReceta == actual?.Receta) ?? _recetas[1]).Checked = true;
        (_planes.FirstOrDefault(o => o.Tag!.ToString() == actual?.Plan) ?? _planes[0]).Checked = true;
        if (actual != null)
        {
            _panelNave.Mostrar(actual, animar: false);
        }
        MostrarResultado();
    }

    private int AnchoOpcion(string texto) => TextRenderer.MeasureText(texto, Font).Width + 26;

    private static Label Etiqueta(string texto, int x, int y)
    {
        var etiqueta = CrearEtiqueta(texto, 9f, Tema.Texto, FontStyle.Bold);
        etiqueta.Location = new Point(x, y);
        return etiqueta;
    }

    private INaveBuilder RecetaElegida => ((Func<INaveBuilder>)_recetas.First(o => o.Checked).Tag!)();
    private PlanEnsamblaje PlanElegido => (PlanEnsamblaje)_planes.First(o => o.Checked).Tag!;

    private void ActualizarDescripciones()
    {
        if (_recetas.Any(o => o.Checked) && _planes.Any(o => o.Checked))
        {
            var receta = RecetaElegida;
            _lblReceta.Text = $"{receta.GetType().Name}: {receta.Descripcion}";
            _lblPlan.Text = Planes.First(p => p.Plan == PlanElegido).Descripcion;
        }
    }

    private void Ensamblar()
    {
        var nave = _partida.ConstruirNave(RecetaElegida, PlanElegido);
        _bitacora.Reemplazar(_partida.Ingeniero.Bitacora);
        _panelNave.Mostrar(nave, animar: true);
        MostrarResultado();
    }

    private void MostrarResultado()
    {
        var nave = _partida.Preparativos.Nave;
        _lblResultado.ForeColor = nave == null ? Tema.TextoTenue : Tema.Texto;
        _lblResultado.Text = nave == null
            ? "Sin nave asignada: el escuadrón despliega sin bonificaciones."
            : $"Nave asignada a las próximas misiones: {nave.Nombre}\n" +
              $"Efectos: {nave.ResumenEfectos()}\nLos aplica el Control Central al iniciar el turno.";
    }
}
