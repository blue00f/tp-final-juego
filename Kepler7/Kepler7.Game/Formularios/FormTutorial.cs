namespace Kepler7.Game.Formularios;

internal sealed class FormTutorial : FormBase
{
    private sealed record Pagina(string Titulo, string Cuerpo);

    private static readonly Pagina[] Paginas =
    {
        new("1 · La crisis de Kepler-7",
            "Una plaga alienígena tomó los conductos de la Estación Orbital Kepler-7 y dejó fuera de servicio sus subsistemas.\n\n" +
            "Sos el Capitán Kael Ortiz y comandás un escuadrón de cuatro contra seis aliens, uno por sector. Cada alien que " +
            "neutralizan restaura un subsistema de la estación y desbloquea una nueva función. Al vencer al sexto, el Leviatán " +
            "del Vacío, Kepler-7 está a salvo y la campaña termina.\n\n" +
            "Regla de oro: si el Capitán cae, la misión fracasa. El resto del escuadrón puede caer; vos no."),
        new("2 · Tu escuadrón",
            "Cada ronda, cada tripulante actúa una vez. Hacé clic en su tarjeta y elegí una acción:\n\n" +
            "• Capitán (Comandante): Atacar, Construir (despliega un generador de escudo sobre un aliado) y Mover.\n" +
            "• Soldado: Atacar (puede aturdir al alien) y Mover. Con poca vida entra en furia.\n" +
            "• Médica: Curar (también quita el veneno), Atacar y Mover.\n" +
            "• Francotiradora: Atacar, con probabilidad de crítico x2. No pierde daño desde atrás.\n\n" +
            "Vanguardia recibe el daño completo. Retaguardia recibe 60% pero inflige 70% (salvo la francotiradora).\n" +
            "Cuando terminás, presioná Fin de turno y actúa el alien."),
        new("3 · Defensas y estados",
            "Todo golpe atraviesa la cadena de defensas de la unidad, en orden: por ejemplo Esquiva → Armadura → Escudo → Vida. " +
            "Cada capa decide si absorbe el daño o lo pasa a la siguiente. Abajo del campo vas a ver el recorrido del último golpe.\n\n" +
            "En la pestaña Defensas podés reordenar las capas de cualquier tripulante: no es lo mismo que la armadura actúe antes o después del bloqueo.\n\n" +
            "Estados (el borde de la tarjeta cambia de color):\n" +
            "• Envenenado (verde): pierde vida cada turno y ataca al 80%.\n" +
            "• Aturdido (naranja): pierde el turno y recibe +25% de daño.\n" +
            "• Berserk (rojo): ataca x1.5 pero recibe +25% y solo puede atacar."),
        new("4 · Tácticas de mando",
            "• Deshacer / rehacer: mientras sea tu turno podés deshacer cualquier orden. Al confirmar el turno, las acciones quedan fijas.\n" +
            "• IA del alien: el alien decide con una estrategia (agresiva, defensiva, aleatoria o calculadora). Con la Interferencia " +
            "neural podés cambiársela en plena pelea y ver cómo cambian sus decisiones.\n" +
            "• Arsenal: aplicá analizadores sobre el inventario. El Aplicador de veneno hace que sus golpes envenenen al alien.\n" +
            "• Control Central: si el Capitán baja del 30% de vida, la estación entra sola en modo emergencia y lo protege. Pasa una sola vez por combate.\n\n" +
            "Cada patrón que veas en acción queda registrado en tus logros."),
        new("5 · La estación entre misiones",
            "Entre combate y combate volvés al hub de la estación. Cada subsistema restaurado te deja preparar la próxima misión:\n\n" +
            "• Comunicaciones (alien 1): mandá un mensaje a la Tierra. Elegís tipo (Texto, Alerta, Emergencia) y cifrado " +
            "(Sin cifrado, AES, Cuántico). Sin cifrar, el alien lo intercepta y empieza en Berserk.\n" +
            "• Laboratorio (alien 2): cloná un tripulante y mejorá el clon. Espera en criogenia y releva al primer caído.\n" +
            "• Registro (alien 3): 10.000 tripulantes de 5 especies. Asigná un destacamento de apoyo y reclutá un oficial de seguridad.\n" +
            "• Hangar (alien 4): el Ingeniero Jefe ensambla una nave según receta y plan. Cada pieza da una bonificación.\n" +
            "• Bóveda (alien 5): solo el Capitán o un Oficial de Seguridad pueden leer el informe del jefe final.\n\n" +
            "Al desplegar, el Control Central aplica todo con un solo IniciarTurno().")
    };

    private readonly Label _titulo;
    private readonly Label _cuerpo;
    private readonly Boton _anterior;
    private readonly Boton _siguiente;
    private readonly Label _numero;
    private int _pagina;

    public FormTutorial()
    {
        Text = "Kepler-7 · Tutorial";
        ClientSize = new Size(840, 560);

        _titulo = CrearEtiqueta("", 15f, Tema.Texto, FontStyle.Bold);
        _titulo.Location = new Point(36, 30);

        _cuerpo = new Label
        {
            Location = new Point(38, 80),
            Size = new Size(764, 390),
            ForeColor = Tema.Texto,
            Font = Tema.Crear(10.5f),
            BackColor = Color.Transparent
        };

        _anterior = new Boton("Anterior", 110, 32) { Location = new Point(36, 500) };
        _anterior.Click += (_, _) => Ir(_pagina - 1);
        _siguiente = new Boton("Siguiente", 110, 32) { Location = new Point(692, 500) };
        _siguiente.Click += (_, _) =>
        {
            if (_pagina == Paginas.Length - 1)
            {
                Close();
            }
            else
            {
                Ir(_pagina + 1);
            }
        };

        _numero = CrearTexto("", 9f, Tema.TextoTenue, new Rectangle(320, 506, 200, 20));
        _numero.TextAlign = ContentAlignment.MiddleCenter;

        Controls.AddRange(new Control[] { _titulo, _cuerpo, _anterior, _siguiente, _numero });
        Ir(0);
    }

    private void Ir(int pagina)
    {
        _pagina = Math.Clamp(pagina, 0, Paginas.Length - 1);
        var p = Paginas[_pagina];
        _titulo.Text = p.Titulo;
        _cuerpo.Text = p.Cuerpo;
        _anterior.Enabled = _pagina > 0;
        _siguiente.Text = _pagina == Paginas.Length - 1 ? "Cerrar" : "Siguiente";
        _numero.Text = $"Página {_pagina + 1} de {Paginas.Length}";
    }
}
