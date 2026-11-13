using Kepler7.Core.Modelo;
using Kepler7.Core.Visitor;

namespace Kepler7.Core.Juego;

public static class FabricaEscuadron
{
    public static Escuadron Crear(Inventario inventario)
    {
        var miembros = new List<MiembroEscuadron>
        {
            Crear(inventario, "Cap. Kael Ortiz", Rol.Comandante, vida: 130, ataque: 15),
            Crear(inventario, "Sgto. Darío Kross", Rol.Soldado, vida: 150, ataque: 18),
            Crear(inventario, "Dra. Lía Moreau", Rol.Medico, vida: 95, ataque: 9),
            Crear(inventario, "Cabo Yara Sen", Rol.Francotirador, vida: 90, ataque: 20)
        };
        return new Escuadron(miembros);
    }

    private static MiembroEscuadron Crear(Inventario inventario, string nombre, Rol rol, int vida, int ataque)
    {
        int bonusArma = inventario.ArmaDe(rol)?.BonusDanio ?? 0;
        return new MiembroEscuadron(nombre, rol, vida, ataque + bonusArma, Escuadron.PosicionInicial(rol));
    }
}
