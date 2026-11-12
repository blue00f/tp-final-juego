namespace Kepler7.Core.Builder;

public sealed class RecetaCarga : RecetaBase
{
    public override string NombreReceta => "Carga";
    public override string Descripcion => "Lenta pero llena de suministros. Curaciones potenciadas y buen escudo.";

    public override void ConstruirCasco()
    {
        Nave.Casco = Instalar("Casco", "Casco de carga reforzado", "escudo inicial +10");
        Nave.EscudoInicial += 10;
    }

    public override void ConstruirMotor()
    {
        Nave.Motor = Instalar("Motor", "Motor de empuje lento", "sin efecto en combate");
    }

    public override void ConstruirEscudo()
    {
        Nave.Escudo = Instalar("Escudo", "Escudo de contención", "escudo inicial +10");
        Nave.EscudoInicial += 10;
    }

    public override void ConstruirArmas()
    {
        Nave.Armas = Instalar("Armas", "Torreta defensiva", "ataque +5%");
        Nave.MultiplicadorAtaque += 0.05;
    }

    public override void ConstruirRadar()
    {
        Nave.Radar = Instalar("Radar", "Radar civil", "sin efecto en combate");
    }

    public override void ConstruirInteligenciaArtificial()
    {
        Nave.InteligenciaArtificial = Instalar("IA", "Intendente logística", "XP +10%");
        Nave.MultiplicadorXp += 0.10;
    }

    public override void ConstruirTanquesCombustible()
    {
        Nave.TanquesCombustible = Instalar("Tanques", "Bodega médica presurizada", "curación +35%");
        Nave.MultiplicadorCuracion += 0.35;
    }
}
