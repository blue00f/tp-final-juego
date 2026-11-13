namespace Kepler7.Core.Builder;

public sealed class IngenieroJefe
{
    private readonly List<string> _bitacora = new();

    public IReadOnlyList<string> Bitacora => _bitacora;

    public Nave Construir(INaveBuilder receta, PlanEnsamblaje plan = PlanEnsamblaje.Completo)
    {
        _bitacora.Clear();
        _bitacora.Add($"Plan {plan} con {receta.GetType().Name}:");
        receta.Reiniciar();
        Paso(nameof(INaveBuilder.ConstruirCasco), receta.ConstruirCasco);
        Paso(nameof(INaveBuilder.ConstruirMotor), receta.ConstruirMotor);

        switch (plan)
        {
            case PlanEnsamblaje.Completo:
                Paso(nameof(INaveBuilder.ConstruirEscudo), receta.ConstruirEscudo);
                Paso(nameof(INaveBuilder.ConstruirArmas), receta.ConstruirArmas);
                Paso(nameof(INaveBuilder.ConstruirRadar), receta.ConstruirRadar);
                Paso(nameof(INaveBuilder.ConstruirInteligenciaArtificial), receta.ConstruirInteligenciaArtificial);
                Paso(nameof(INaveBuilder.ConstruirTanquesCombustible), receta.ConstruirTanquesCombustible);
                break;
            case PlanEnsamblaje.Asalto:
                Paso(nameof(INaveBuilder.ConstruirEscudo), receta.ConstruirEscudo);
                Paso(nameof(INaveBuilder.ConstruirArmas), receta.ConstruirArmas);
                break;
            case PlanEnsamblaje.Reconocimiento:
                Paso(nameof(INaveBuilder.ConstruirRadar), receta.ConstruirRadar);
                Paso(nameof(INaveBuilder.ConstruirInteligenciaArtificial), receta.ConstruirInteligenciaArtificial);
                Paso(nameof(INaveBuilder.ConstruirTanquesCombustible), receta.ConstruirTanquesCombustible);
                break;
        }

        var nave = receta.ObtenerNave();
        nave.Plan = plan.ToString();
        nave.Nombre = $"{receta.NombreReceta} · {plan}";
        _bitacora.Add($"ObtenerNave() → {nave.Nombre} ({nave.Componentes.Count} componentes).");
        return nave;
    }

    private void Paso(string metodo, Action paso)
    {
        paso();
        _bitacora.Add($"{_bitacora.Count}. {metodo}()");
    }
}
