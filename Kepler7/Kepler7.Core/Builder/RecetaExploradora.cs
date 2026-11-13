namespace Kepler7.Core.Builder;


public sealed class RecetaExploradora : RecetaBase
{
    public override string NombreReceta => "Exploradora";
    public override string Descripcion => "Ligera y difícil de alcanzar. El radar y la IA mejoran la esquiva y la XP.";

    public override void ConstruirCasco()
    {
        Nave.Casco = Instalar("Casco", "Fibra de carbono", "liviano, sin blindaje");
    }

    public override void ConstruirMotor()
    {
        Nave.Motor = Instalar("Motor", "Impulsor iónico", "esquiva +5%");
        Nave.EsquivaExtra += 0.05;
    }

    public override void ConstruirEscudo()
    {
        Nave.Escudo = Instalar("Escudo", "Deflector básico", "escudo inicial +5");
        Nave.EscudoInicial += 5;
    }

    public override void ConstruirArmas()
    {
        Nave.Armas = Instalar("Armas", "Torreta ligera", "ataque +5%");
        Nave.MultiplicadorAtaque += 0.05;
    }

    public override void ConstruirRadar()
    {
        Nave.Radar = Instalar("Radar", "Radar de largo alcance", "esquiva +10%");
        Nave.EsquivaExtra += 0.10;
    }

    public override void ConstruirInteligenciaArtificial()
    {
        Nave.InteligenciaArtificial = Instalar("IA", "Cartógrafa ORION", "XP +25%");
        Nave.MultiplicadorXp += 0.25;
    }

    public override void ConstruirTanquesCombustible()
    {
        Nave.TanquesCombustible = Instalar("Tanques", "Tanques de autonomía extendida", "curación +5%");
        Nave.MultiplicadorCuracion += 0.05;
    }
}
