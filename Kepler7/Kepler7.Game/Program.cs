using Kepler7.Core.Progreso;
using Kepler7.Game.Formularios;

namespace Kepler7.Game;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Persistencia en %AppData%\Kepler7\ (nunca en la carpeta del ejecutable).
        string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kepler7");
        var repositorio = new RepositorioProgreso(carpeta);

        Application.Run(new FormInicio(repositorio));
    }
}
