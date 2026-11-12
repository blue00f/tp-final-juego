namespace Kepler7.Core.Progreso;

public static class Patrones
{
    public const string Builder = "Builder";
    public const string Prototype = "Prototype";
    public const string Bridge = "Bridge";
    public const string Facade = "Facade";
    public const string Flyweight = "Flyweight";
    public const string Proxy = "Proxy";
    public const string ChainOfResponsibility = "Chain of Responsibility";
    public const string Command = "Command";
    public const string Visitor = "Visitor";
    public const string State = "State";
    public const string TemplateMethod = "Template Method";
    public const string Strategy = "Strategy";

    public sealed record InfoPatron(string Nombre, FamiliaPatron Familia, string Mecanica, string ComoSeDomina);

    public static IReadOnlyList<InfoPatron> Todos { get; } = new[]
    {
        new InfoPatron(Builder, FamiliaPatron.Creacional, "Hangar: naves para cada misión", "Hangar (alien 4): construí una nave"),
        new InfoPatron(Prototype, FamiliaPatron.Creacional, "Laboratorio: clones de reserva", "Laboratorio (alien 2): mejorá un clon"),
        new InfoPatron(Bridge, FamiliaPatron.Estructural, "Mensajes cifrados a la Tierra", "Comunicaciones (alien 1): enviá un mensaje"),
        new InfoPatron(Facade, FamiliaPatron.Estructural, "Control Central de la estación", "Desplegá con preparativos o activá la emergencia"),
        new InfoPatron(Flyweight, FamiliaPatron.Estructural, "Registro de 10.000 tripulantes", "Registro (alien 3): asigná un destacamento"),
        new InfoPatron(Proxy, FamiliaPatron.Estructural, "Bóveda de datos protegida", "Bóveda (alien 5): operá la terminal"),
        new InfoPatron(ChainOfResponsibility, FamiliaPatron.Comportamiento, "Capas de defensa", "Una defensa absorbe daño antes de la vida"),
        new InfoPatron(Command, FamiliaPatron.Comportamiento, "Acciones con deshacer/rehacer", "Deshacé una acción en combate"),
        new InfoPatron(Visitor, FamiliaPatron.Comportamiento, "Analizador del arsenal", "Aplicá un analizador al arsenal"),
        new InfoPatron(State, FamiliaPatron.Comportamiento, "Estados del personaje", "Una unidad cambia de estado"),
        new InfoPatron(TemplateMethod, FamiliaPatron.Comportamiento, "Estructura de turnos", "Completá turnos con los 4 roles"),
        new InfoPatron(Strategy, FamiliaPatron.Comportamiento, "IA intercambiable del alien", "Cambiá la estrategia del alien")
    };
}
