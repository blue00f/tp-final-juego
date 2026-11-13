namespace Kepler7.Core.Builder;

public interface INaveBuilder
{
    string NombreReceta { get; }
    string Descripcion { get; }

    void Reiniciar();
    void ConstruirCasco();
    void ConstruirMotor();
    void ConstruirEscudo();
    void ConstruirArmas();
    void ConstruirRadar();
    void ConstruirInteligenciaArtificial();
    void ConstruirTanquesCombustible();

    Nave ObtenerNave();
}
