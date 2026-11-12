namespace Kepler7.Core.Command;

public sealed class GestorComandos
{
    private readonly Stack<IComando> _deshacer = new();
    private readonly Stack<IComando> _rehacer = new();
    private readonly List<string> _historial = new();

    public IReadOnlyList<string> Historial => _historial;

    public bool PuedeDeshacer => _deshacer.Count > 0;
    public bool PuedeRehacer => _rehacer.Count > 0;

    public int Ejecutados { get; private set; }
    public int Deshechos { get; private set; }

    public void Ejecutar(IComando comando)
    {
        comando.Ejecutar();
        _deshacer.Push(comando);
        _rehacer.Clear();
        Ejecutados++;
        _historial.Add($"▶ {comando.Descripcion}");
    }

    public IComando? Deshacer()
    {
        IComando? comando = null;
        if (_deshacer.Count > 0)
        {
            comando = _deshacer.Pop();
            comando.Deshacer();
            _rehacer.Push(comando);
            Deshechos++;
            _historial.Add($"↩ Deshecho: {comando.Descripcion}");
        }
        return comando;
    }

    public IComando? Rehacer()
    {
        IComando? comando = null;
        if (_rehacer.Count > 0)
        {
            comando = _rehacer.Pop();
            comando.Ejecutar();
            _deshacer.Push(comando);
            _historial.Add($"↪ Rehecho: {comando.Descripcion}");
        }
        return comando;
    }

    public void ConfirmarTurno(int ronda)
    {
        _deshacer.Clear();
        _rehacer.Clear();
        _historial.Add($"✔ Turno {ronda} confirmado");
    }
}
