using System.Drawing.Drawing2D;
using Kepler7.Core.Flyweight;
using Kepler7.Core.Juego;

namespace Kepler7.Game.Formularios;

internal sealed class FormRegistro : FormBase
{
    private readonly Partida _partida;
    private readonly RegistroTripulacion _registro;
    private readonly PanelMapa _mapa;
    private readonly List<Opcion> _opciones = new();
    private readonly Label _lblContadores;
    private readonly Label _lblEspecie;
    private readonly Label _lblDestacamento;
    private readonly Boton _btnAsignar = new("Asignar como destacamento de apoyo", 300, 32);
    private readonly PanelMemoria _memoria = new();
    private readonly Boton _btnComparar = new("Comparar memoria con / sin Flyweight", 300, 32);
    private readonly ListBox _lstOficiales = new();
    private readonly Boton _btnReclutar = new("Reclutar oficial", 150, 30);
    private readonly Label _lblOficial;

    public FormRegistro(Partida partida)
    {
        _partida = partida;
        _registro = partida.Registro;
        Text = "Kepler-7 · Registro de tripulación";
        ClientSize = new Size(1120, 720);
        AgregarEncabezado("Registro de tripulación", "Flyweight",
            "10.000 tripulantes, 5 especies. Cada Tripulante guarda su nombre, identificador y rango (estado extrínseco) y " +
            "apunta a un IEspecie compartido (nombre, color, planeta, dieta y retrato de 4 KB) que entrega la FabricaEspecies.");

        _mapa = new PanelMapa(_registro) { Bounds = new Rectangle(24, 112, 520, 470) };
        Controls.Add(_mapa);

        _lblContadores = CrearTexto("", 9f, Tema.Texto, new Rectangle(24, 590, 520, 116));
        Controls.Add(_lblContadores);

        var titulo = CrearEtiqueta("Especies (estado intrínseco compartido)", 9f, Tema.Texto, FontStyle.Bold);
        titulo.Location = new Point(568, 112);
        Controls.Add(titulo);
        var grupo = new Panel { Bounds = new Rectangle(568, 134, 340, 200) };
        int y = 0;
        foreach (var especie in _registro.Fabrica.Especies.OrderBy(e => e.Nombre))
        {
            var opcion = new Opcion($"{especie.Nombre} · {_registro.Contar(especie.Nombre):N0} tripulantes", 340, 26)
            {
                Location = new Point(0, y),
                Tag = especie
            };
            opcion.CheckedChanged += (_, _) => SeleccionarEspecie();
            _opciones.Add(opcion);
            grupo.Controls.Add(opcion);
            y += 30;
        }
        Controls.Add(grupo);

        _lblEspecie = CrearTexto("", 9f, Tema.Texto, new Rectangle(920, 134, 180, 200));
        Controls.Add(_lblEspecie);

        _btnAsignar.Location = new Point(568, 340);
        _btnAsignar.Click += (_, _) => AsignarDestacamento();
        Controls.Add(_btnAsignar);
        _lblDestacamento = CrearTexto("", 9f, Tema.Texto, new Rectangle(568, 380, 530, 22));
        Controls.Add(_lblDestacamento);

        _btnComparar.Location = new Point(568, 410);
        _btnComparar.Click += async (_, _) => await CompararAsync();
        Controls.Add(_btnComparar);
        _memoria.SetBounds(568, 450, 530, 92);
        Controls.Add(_memoria);

        var lblOficiales = CrearEtiqueta("Oficiales de seguridad (pueden operar la bóveda)", 9f, Tema.Texto, FontStyle.Bold);
        lblOficiales.Location = new Point(568, 552);
        Controls.Add(lblOficiales);
        _lstOficiales.SetBounds(568, 574, 360, 92);
        foreach (var oficial in _registro.Filtrar(null, RegistroTripulacion.RangoSeguridad).Take(25))
        {
            _lstOficiales.Items.Add(new ItemOficial(oficial));
        }
        _lstOficiales.SelectedIndex = 0;
        Controls.Add(_lstOficiales);
        _btnReclutar.Location = new Point(940, 574);
        _btnReclutar.Click += (_, _) => Reclutar();
        Controls.Add(_btnReclutar);
        _lblOficial = CrearTexto("", 8.5f, Tema.Texto, new Rectangle(940, 610, 160, 56));
        Controls.Add(_lblOficial);

        var cerrar = new Boton("Volver a la estación", 180, 32) { Location = new Point(918, 676) };
        cerrar.Click += (_, _) => Close();
        Controls.Add(cerrar);

        var actual = partida.Preparativos.Destacamento;
        (_opciones.FirstOrDefault(o => o.Tag == actual) ?? _opciones[0]).Checked = true;
        ActualizarTextos();
    }

    private IEspecie EspecieSeleccionada => (IEspecie)_opciones.First(o => o.Checked).Tag!;

    private void SeleccionarEspecie()
    {
        if (_opciones.Any(o => o.Checked))
        {
            var especie = EspecieSeleccionada;
            _mapa.Resaltar(especie);
            _lblEspecie.Text = $"Planeta: {especie.PlanetaOrigen}\nDieta: {especie.Dieta}\nColor: {especie.Color}\n" +
                               $"Retrato: {especie.Retrato.Length / 1024} KB\n\nBonus de destacamento:\n{especie.Bonus.Descripcion}";
        }
    }

    private void AsignarDestacamento()
    {
        _partida.AsignarDestacamento(EspecieSeleccionada);
        ActualizarTextos();
    }

    private void Reclutar()
    {
        if (_lstOficiales.SelectedItem is ItemOficial item)
        {
            _partida.ReclutarOficial(item.Tripulante);
            ActualizarTextos();
        }
    }

    private async Task CompararAsync()
    {
        _btnComparar.Enabled = false;
        _btnComparar.Text = "Midiendo 10.000 copias…";
        // Se crean 10.000 copias reales: se hace fuera del hilo de la interfaz.
        var resultado = await Task.Run(() => _registro.CompararMemoria());
        _memoria.Mostrar(resultado);
        _btnComparar.Text = "Comparar memoria con / sin Flyweight";
        _btnComparar.Enabled = true;
    }

    private void ActualizarTextos()
    {
        var fabrica = _registro.Fabrica;
        _lblContadores.Text =
            $"Tripulantes generados: {_registro.Tripulantes.Count:N0}\n" +
            $"Pedidos a FabricaEspecies.ObtenerEspecie(): {fabrica.Solicitudes:N0}\n" +
            $"Objetos IEspecie creados por la fábrica: {fabrica.Cantidad}\n" +
            $"Objetos IEspecie distintos referenciados por los tripulantes: {_registro.EspeciesDistintasReferenciadas}";
        var destacamento = _partida.Preparativos.Destacamento;
        _lblDestacamento.Text = destacamento == null
            ? "Sin destacamento: elegí una especie y asignala."
            : $"Destacamento {destacamento.Nombre}: {destacamento.Bonus.Descripcion} en las próximas misiones.";
        var oficial = _partida.Preparativos.OficialSeguridad;
        _lblOficial.Text = oficial == null ? "Sin oficial reclutado." : $"Reclutado: {oficial.Nombre}\n{oficial.Codigo}";
    }

    private sealed class ItemOficial
    {
        public ItemOficial(Tripulante tripulante)
        {
            Tripulante = tripulante;
        }

        public Tripulante Tripulante { get; }

        public override string ToString() => $"{Tripulante.Codigo} · {Tripulante.Nombre} · {Tripulante.Especie.Nombre}";
    }
}
