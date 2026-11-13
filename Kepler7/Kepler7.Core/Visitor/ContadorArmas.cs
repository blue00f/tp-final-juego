namespace Kepler7.Core.Visitor;

public sealed class ContadorArmas : IVisitor
{
    private int _armas;
    private int _envenenadas;
    private int _otros;

    public string Nombre => "Contador de armas";
    public string Descripcion => "Cuenta armas operativas y envenenadas.";

    public void Visitar(Arma arma)
    {
        _armas++;
        if (arma.Envenenada)
        {
            _envenenadas++;
        }
    }

    public void Visitar(Pocion pocion) => _otros++;
    public void Visitar(Armadura armadura) => _otros++;

    public string Resultado => $"{_armas} armas ({_envenenadas} envenenadas) · {_otros} ítems que no son armas";

    public void Reiniciar()
    {
        _armas = 0;
        _envenenadas = 0;
        _otros = 0;
    }
}
