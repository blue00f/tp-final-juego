namespace Kepler7.Core.Builder;

public abstract class RecetaBase : INaveBuilder
{
    private Nave _nave;

    protected RecetaBase()
    {
        _nave = new Nave(NombreReceta);
    }

    public abstract string NombreReceta { get; }
    public abstract string Descripcion { get; }

    protected Nave Nave => _nave;

    public void Reiniciar()
    {
        _nave = new Nave(NombreReceta);
    }

    public abstract void ConstruirCasco();
    public abstract void ConstruirMotor();
    public abstract void ConstruirEscudo();
    public abstract void ConstruirArmas();
    public abstract void ConstruirRadar();
    public abstract void ConstruirInteligenciaArtificial();
    public abstract void ConstruirTanquesCombustible();

    public Nave ObtenerNave()
    {
        var terminada = _nave;
        Reiniciar();
        return terminada;
    }

    protected Componente Instalar(string tipo, string nombre, string efecto)
    {
        var componente = new Componente(tipo, nombre, efecto);
        _nave.Instalar(componente);
        return componente;
    }
}
