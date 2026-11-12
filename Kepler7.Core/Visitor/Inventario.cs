using Kepler7.Core.Modelo;

namespace Kepler7.Core.Visitor;

public sealed class Inventario
{
    private readonly List<IItem> _items = new();

    public IReadOnlyList<IItem> Items => _items;

    public void Agregar(IItem item) => _items.Add(item);

    public string Aplicar(IVisitor visitante)
    {
        visitante.Reiniciar();
        foreach (var item in _items)
        {
            item.Aceptar(visitante);
        }
        return visitante.Resultado;
    }

    public Arma? ArmaDe(Rol rol) => _items.OfType<Arma>().FirstOrDefault(a => a.Portador == rol);

    public Armadura? ArmaduraDe(Rol rol) => _items.OfType<Armadura>().FirstOrDefault(a => a.Portador == rol);

    public static Inventario CrearInicial()
    {
        var inventario = new Inventario();
        inventario.Agregar(new Arma("Pistola de pulso", 120, 4, Rol.Comandante));
        inventario.Agregar(new Arma("Rifle de asalto K-7", 180, 6, Rol.Soldado));
        inventario.Agregar(new Arma("Dardos sedantes", 60, 1, Rol.Medico));
        inventario.Agregar(new Arma("Rifle de riel", 240, 8, Rol.Francotirador));
        inventario.Agregar(new Armadura("Coraza de mando", 150, 0.15, Rol.Comandante));
        inventario.Agregar(new Armadura("Placas de titanio", 200, 0.25, Rol.Soldado));
        inventario.Agregar(new Armadura("Traje de campaña", 90, 0.10, Rol.Medico));
        inventario.Agregar(new Pocion("Nanobots de reparación", 75, 30));
        inventario.Agregar(new Pocion("Suero de estasis", 50, 15));
        return inventario;
    }
}
