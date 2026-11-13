namespace Kepler7.Core.Builder;

public sealed class RecetaCombate : RecetaBase
{
    public override string NombreReceta => "Combate";
    public override string Descripcion => "Cañones pesados y blindaje. Más daño y un escudo inicial grande.";

    public override void ConstruirCasco()
    {
        Nave.Casco = Instalar("Casco", "Blindaje de titanio", "escudo inicial +10");
        Nave.EscudoInicial += 10;
    }

    public override void ConstruirMotor()
    {
        Nave.Motor = Instalar("Motor", "Propulsor de fusión", "sin efecto en combate");
    }

    public override void ConstruirEscudo()
    {
        Nave.Escudo = Instalar("Escudo", "Generador de escudo militar", "escudo inicial +15");
        Nave.EscudoInicial += 15;
    }

    public override void ConstruirArmas()
    {
        Nave.Armas = Instalar("Armas", "Cañones de plasma", "ataque +20%");
        Nave.MultiplicadorAtaque += 0.20;
    }

    public override void ConstruirRadar()
    {
        Nave.Radar = Instalar("Radar", "Radar de tiro", "esquiva +2%");
        Nave.EsquivaExtra += 0.02;
    }

    public override void ConstruirInteligenciaArtificial()
    {
        Nave.InteligenciaArtificial = Instalar("IA", "Computadora de tiro ARES", "ataque +5%");
        Nave.MultiplicadorAtaque += 0.05;
    }

    public override void ConstruirTanquesCombustible()
    {
        Nave.TanquesCombustible = Instalar("Tanques", "Tanque estándar", "sin efecto en combate");
    }
}
