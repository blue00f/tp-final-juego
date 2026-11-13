using Kepler7.Core.Juego;
using Kepler7.Core.Proxy;
using Kepler7.Game.Controles;

namespace Kepler7.Game.Formularios;

internal sealed class FormBoveda : FormBase
{
    private readonly Partida _partida;
    private readonly List<Opcion> _operadores = new();
    private readonly ListBox _lstArchivos = new();
    private readonly CajaTexto _txtContenido = new(monoespaciada: false);
    private readonly CajaTexto _txtEscribir = new(monoespaciada: false, soloLectura: false);
    private readonly Label _lblResultado;
    private readonly Label _lblBoveda;
    private readonly ListaRegistro _lstRechazos = new();
    private readonly Panel _terminal = new();
    private readonly System.Windows.Forms.Timer _timerAlarma = new() { Interval = 150 };
    private int _parpadeos;

    public FormBoveda(Partida partida)
    {
        _partida = partida;
        Text = "Kepler-7 · Bóveda de datos";
        ClientSize = new Size(1080, 680);
        AgregarEncabezado("Bóveda de datos", "Proxy",
            "La terminal habla con ProxyBoveda, que tiene la misma interfaz IBoveda que BovedaDatos. Antes de delegar verifica " +
            "el rol: solo el Capitán y el Oficial de Seguridad pueden leer o escribir. Los rechazos quedan registrados en disco.");

        Controls.Add(Etiqueta("Operador de la terminal", 24, 112));
        var grupo = new Panel { Bounds = new Rectangle(24, 136, 320, 220) };
        int y = 0;
        foreach (var usuario in partida.OperadoresDisponibles)
        {
            bool autorizado = ProxyBoveda.EstaAutorizado(usuario.Rol);
            var opcion = new Opcion($"{usuario.Nombre} · {NombreRol(usuario.Rol)}{(autorizado ? " (autorizado)" : "")}", 320, 26)
            {
                Location = new Point(0, y),
                Tag = usuario
            };
            _operadores.Add(opcion);
            grupo.Controls.Add(opcion);
            y += 30;
        }
        Controls.Add(grupo);
        if (partida.Preparativos.OficialSeguridad == null)
        {
            Controls.Add(CrearTexto("Reclutá un Oficial de Seguridad en el Registro para tener otro operador autorizado.",
                8.5f, Tema.TextoTenue, new Rectangle(24, 262, 320, 36)));
        }

        _terminal.SetBounds(364, 112, 420, 500);
        _terminal.BackColor = Tema.Panel;
        _terminal.BorderStyle = BorderStyle.FixedSingle;
        Controls.Add(_terminal);

        var lblArchivos = CrearEtiqueta("Archivos (índice público)", 9f, Tema.Texto, FontStyle.Bold);
        lblArchivos.Location = new Point(12, 10);
        _lstArchivos.SetBounds(12, 32, 396, 96);
        _lstArchivos.Font = new Font(Tema.FuenteMono, 9.5f);
        foreach (var archivo in partida.Boveda.ListarArchivos())
        {
            _lstArchivos.Items.Add(archivo);
        }
        _lstArchivos.SelectedIndex = Math.Max(0, _lstArchivos.Items.IndexOf(BovedaDatos.ArchivoIntel));

        var btnLeer = new Boton("Leer archivo", 194, 32) { Location = new Point(12, 136) };
        btnLeer.Click += (_, _) => Leer();
        var btnEscribir = new Boton("Escribir archivo", 194, 32) { Location = new Point(214, 136) };
        btnEscribir.Click += (_, _) => Escribir();

        _txtContenido.SetBounds(12, 182, 396, 150);
        _txtContenido.Text = "(contenido protegido)";

        var lblEscribir = CrearEtiqueta("Texto a escribir:", 9f, Tema.Texto);
        lblEscribir.Location = new Point(12, 340);
        _txtEscribir.SetBounds(12, 360, 396, 54);
        _txtEscribir.Text = "Claves AES rotadas por orden del Capitán.";

        _lblResultado = CrearTexto("", 9f, Tema.Texto, new Rectangle(12, 420, 396, 72));
        _terminal.Controls.AddRange(new Control[] { lblArchivos, _lstArchivos, btnLeer, btnEscribir, _txtContenido, lblEscribir, _txtEscribir, _lblResultado });

        Controls.Add(Etiqueta("Accesos denegados (se guardan en disco)", 804, 112));
        _lstRechazos.SetBounds(804, 136, 252, 400);
        Controls.Add(_lstRechazos);
        _lblBoveda = CrearTexto("", 8.5f, Tema.TextoTenue, new Rectangle(804, 540, 252, 72));
        Controls.Add(_lblBoveda);

        var cerrar = new Boton("Volver a la estación", 180, 32) { Location = new Point(876, 634) };
        cerrar.Click += (_, _) => Close();
        Controls.Add(cerrar);

        _timerAlarma.Tick += (_, _) => Parpadear();
        _operadores[0].Checked = true;
        Refrescar();
    }

    private static Label Etiqueta(string texto, int x, int y)
    {
        var etiqueta = CrearEtiqueta(texto, 9f, Tema.Texto, FontStyle.Bold);
        etiqueta.Location = new Point(x, y);
        return etiqueta;
    }

    private static string NombreRol(RolAcceso rol) => rol switch
    {
        RolAcceso.Capitan => "Capitán",
        RolAcceso.OficialSeguridad => "Oficial de Seguridad",
        RolAcceso.Medico => "Médica",
        RolAcceso.Francotirador => "Francotiradora",
        _ => "Soldado"
    };

    private Usuario Operador => (Usuario)_operadores.First(o => o.Checked).Tag!;
    private string Archivo => _lstArchivos.SelectedItem?.ToString() ?? BovedaDatos.ArchivoIntel;

    private void Leer()
    {
        try
        {
            _txtContenido.Text = _partida.LeerBoveda(Operador, Archivo).Replace("\n", Environment.NewLine);
            string efecto = Archivo == BovedaDatos.ArchivoIntel
                ? "\n→ Inteligencia cargada: el jefe final pierde Armadura y Escudo."
                : "";
            Exito($"Lectura autorizada ({NombreRol(Operador.Rol)}).{efecto}");
        }
        catch (AccesoDenegadoException ex)
        {
            _txtContenido.Text = "(acceso denegado: contenido clasificado)";
            Denegar(ex.Message);
        }
        Refrescar();
    }

    private void Escribir()
    {
        try
        {
            _partida.EscribirBoveda(Operador, Archivo, _txtEscribir.Text);
            _txtContenido.Text = _txtEscribir.Text;
            string efecto = Archivo == BovedaDatos.ArchivoClaves
                ? "\n→ Claves rotadas: AES deja de ser interceptable."
                : "";
            Exito($"Escritura autorizada en {Archivo}.{efecto}");
        }
        catch (AccesoDenegadoException ex)
        {
            Denegar(ex.Message);
        }
        Refrescar();
    }

    private void Exito(string texto)
    {
        _lblResultado.ForeColor = Tema.Exito;
        _lblResultado.Text = texto;
    }

    private void Denegar(string texto)
    {
        _lblResultado.ForeColor = Tema.Peligro;
        _lblResultado.Text = texto;
        _parpadeos = 6;
        _timerAlarma.Start();
    }

    private void Parpadear()
    {
        _parpadeos--;
        _terminal.BackColor = _parpadeos % 2 == 0 ? Tema.Panel : Tema.Mezclar(Tema.Panel, Tema.Peligro, 0.35);
        if (_parpadeos <= 0)
        {
            _timerAlarma.Stop();
            _terminal.BackColor = Tema.Panel;
        _terminal.BorderStyle = BorderStyle.FixedSingle;
        }
    }

    private void Refrescar()
    {
        var intentos = _partida.Boveda.Registro.Intentos;
        _lstRechazos.Reemplazar(intentos.Count == 0
            ? new[] { "Sin intentos denegados." }
            : intentos.Reverse().Select(i => $"✖ {i.Fecha:dd/MM HH:mm:ss} · {i.Usuario} ({NombreRol(i.Rol)}) · {i.Operacion} {i.Archivo}"));
        var prep = _partida.Preparativos;
        _lblBoveda.Text =
            $"BovedaDatos: {(_partida.Boveda.BovedaAbierta ? "abierta" : "sin abrir (el proxy la crea recién cuando hace falta)")}\n" +
            $"Inteligencia del jefe: {(prep.InteligenciaJefe ? "sí" : "no")} · Claves: {(prep.ClavesRotadas ? "rotadas" : "de fábrica")}";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timerAlarma.Dispose();
        }
        base.Dispose(disposing);
    }
}
