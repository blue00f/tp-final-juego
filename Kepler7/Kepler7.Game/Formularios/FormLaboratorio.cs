using Kepler7.Core.Juego;
using Kepler7.Core.Modelo;
using Kepler7.Core.Prototype;

namespace Kepler7.Game.Formularios;

internal sealed class FormLaboratorio : FormBase
{
    private readonly Partida _partida;
    private readonly List<Opcion> _opciones = new();
    private readonly List<Boton> _botonesMejora = new();
    private readonly Boton _btnClonar = new("Clonar", 160, 32);
    private readonly Label _lblMejoras;
    private readonly Label _lblEstado;
    private readonly PanelComparacion _comparacion = new();
    private MiembroEscuadron? _original;
    private string? _error;

    public FormLaboratorio(Partida partida)
    {
        _partida = partida;
        Text = "Kepler-7 · Laboratorio";
        ClientSize = new Size(1040, 660);
        AgregarEncabezado("Laboratorio de biotecnología", "Prototype",
            "MiembroEscuadron implementa IPrototipo.Clonar() con copia profunda escrita a mano: lista de habilidades, estado y " +
            "cadena de defensas son objetos nuevos. Modificá el clon y comprobá que el original no cambia.");

        Controls.Add(Etiqueta("1. Tripulante a clonar", 24, 114));
        var grupo = new Panel { Bounds = new Rectangle(24, 138, 300, 130) };
        int y = 0;
        foreach (var miembro in partida.Escuadron.Titulares.Where(m => !m.EsCapitan))
        {
            var opcion = new Opcion($"{miembro.Nombre} · {Tema.NombreRol(miembro.Rol)}", 300, 26)
            {
                Location = new Point(0, y),
                Tag = miembro
            };
            opcion.CheckedChanged += (_, _) => MostrarOriginal();
            _opciones.Add(opcion);
            grupo.Controls.Add(opcion);
            y += 30;
        }
        Controls.Add(grupo);
        Controls.Add(CrearTexto("El Capitán no se puede clonar (protocolo 7-B): si cae, la misión termina.", 8.5f,
            Tema.TextoTenue, new Rectangle(24, 232, 300, 34)));

        _btnClonar.Location = new Point(24, 272);
        _btnClonar.Click += (_, _) => Clonar();
        Controls.Add(_btnClonar);

        Controls.Add(Etiqueta("2. Mejoras genéticas (solo al clon)", 24, 324));
        y = 348;
        foreach (var mejora in LaboratorioBiotecnologia.Mejoras)
        {
            var boton = new Boton($"{mejora.Nombre}: {mejora.Descripcion}", 300, 32)
            {
                Location = new Point(24, y),
                TextAlign = ContentAlignment.MiddleLeft,
                Tag = mejora
            };
            boton.Click += (_, _) => Mejorar((MejoraGenetica)boton.Tag!);
            _botonesMejora.Add(boton);
            Controls.Add(boton);
            y += 38;
        }
        _lblMejoras = CrearTexto("", 9f, Tema.Texto, new Rectangle(24, 504, 300, 22), FontStyle.Bold);
        Controls.Add(_lblMejoras);

        Controls.Add(Etiqueta("Comparación original / clon", 352, 114));
        _comparacion.SetBounds(352, 138, 664, 360);
        Controls.Add(_comparacion);

        _lblEstado = CrearTexto("", 9.5f, Tema.Texto, new Rectangle(352, 510, 664, 70));
        Controls.Add(_lblEstado);

        var cerrar = new Boton("Volver a la estación", 180, 32) { Location = new Point(836, 610) };
        cerrar.Click += (_, _) => Close();
        Controls.Add(cerrar);

        var clon = partida.Preparativos.ClonReserva;
        _original = clon == null ? null : partida.Escuadron.Titulares.FirstOrDefault(t => clon.Nombre.StartsWith(t.Nombre));
        var seleccion = _opciones.FirstOrDefault(o => o.Tag == _original) ?? _opciones[0];
        seleccion.Checked = true;
        Refrescar();
    }

    private static Label Etiqueta(string texto, int x, int y)
    {
        var etiqueta = CrearEtiqueta(texto, 9f, Tema.Texto, FontStyle.Bold);
        etiqueta.Location = new Point(x, y);
        return etiqueta;
    }

    private MiembroEscuadron Seleccionado => (MiembroEscuadron)_opciones.First(o => o.Checked).Tag!;

    private void MostrarOriginal()
    {
        if (_opciones.Any(o => o.Checked))
        {
            Refrescar();
        }
    }

    private void Clonar()
    {
        _original = Seleccionado;
        _error = null;
        _partida.ClonarTripulante(_original);
        Refrescar();
    }

    private void Mejorar(MejoraGenetica mejora)
    {
        try
        {
            _partida.MejorarClon(mejora);
            _error = null;
            }
        catch (InvalidOperationException ex)
        {
            _error = ex.Message;
        }
        Refrescar();
    }

    private void Refrescar()
    {
        var clon = _partida.Preparativos.ClonReserva;
        bool hayClon = clon != null && _original != null;
        var original = hayClon ? _original! : Seleccionado;

        _comparacion.Mostrar(original, hayClon ? clon : null);
        foreach (var boton in _botonesMejora)
        {
            boton.Enabled = hayClon && _partida.MejorasDisponibles > 0;
        }
        _lblMejoras.Text = hayClon ? $"Mejoras disponibles: {_partida.MejorasDisponibles}/{Partida.MaximoMejorasPorClon}" : "";
        _btnClonar.Text = hayClon ? "Clonar de nuevo" : "Clonar";

        if (_error != null)
        {
            _lblEstado.ForeColor = Tema.Peligro;
            _lblEstado.Text = _error;
        }
        else
        {
            _lblEstado.ForeColor = hayClon ? Tema.Exito : Tema.TextoTenue;
            _lblEstado.Text = hayClon
                ? $"{clon!.Nombre} queda en criogenia. En la próxima misión relevará al primer tripulante que caiga."
                : "Elegí un tripulante y presioná Clonar. Sin clon, un tripulante caído queda fuera hasta la próxima misión.";
        }
    }
}
