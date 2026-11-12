namespace Kepler7.Core.Comun;

public interface IAleatorio
{
    double SiguienteDoble();

    int Entre(int minimo, int maximoExclusivo);

    bool Probabilidad(double p);
}
