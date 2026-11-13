namespace Kepler7.Core.ChainOfResponsibility;

public sealed class Danio
{
    private readonly List<PasoDanio> _recorrido = new();

    public Danio(int cantidad, string origen, string objetivo = "")
    {
        Original = Math.Max(0, cantidad);
        Restante = Original;
        Origen = origen;
        Objetivo = objetivo;
    }

    public int Original { get; }
    public int Restante { get; private set; }
    public string Origen { get; }
    public string Objetivo { get; }

    public string? AbsorbidoPor { get; private set; }

    public int VidaPerdida { get; private set; }

    public bool Esquivado { get; private set; }

    public IReadOnlyList<PasoDanio> Recorrido => _recorrido;

    public void Reducir(string eslabon, int cantidad, string detalle)
    {
        int reducido = Math.Clamp(cantidad, 0, Restante);
        Restante -= reducido;
        _recorrido.Add(new PasoDanio(eslabon, reducido, Restante, detalle));
        if (Restante == 0 && AbsorbidoPor == null)
        {
            AbsorbidoPor = eslabon;
        }
    }

    public void Transferir(string eslabon, string detalle)
    {
        _recorrido.Add(new PasoDanio(eslabon, 0, Restante, detalle));
    }

    public void Esquivar(string eslabon)
    {
        Esquivado = true;
        Reducir(eslabon, Restante, "¡esquivó el ataque por completo!");
    }

    internal void RegistrarVidaPerdida(string eslabon, int vidaPerdida)
    {
        VidaPerdida = vidaPerdida;
        Reducir(eslabon, Restante, $"pierde {vidaPerdida} de vida");
    }

    internal void AplicarMultiplicador(string fuente, double factor)
    {
        int nuevo = (int)Math.Round(Restante * factor);
        int diferencia = Restante - nuevo;
        Restante = nuevo;
        string detalle = diferencia < 0 ? $"amplifica el daño (+{-diferencia})" : $"reduce el daño (-{diferencia})";
        _recorrido.Add(new PasoDanio(fuente, diferencia, Restante, detalle));
    }

    internal void Nota(string eslabon, string detalle)
    {
        _recorrido.Add(new PasoDanio(eslabon, 0, Restante, detalle));
    }

    public string ResumenRecorrido()
    {
        var partes = _recorrido.Select(p => p.Absorbido == 0 ? $"{p.Eslabon}(0)" : $"{p.Eslabon}({p.Absorbido})");
        return string.Join(" → ", partes);
    }
}
