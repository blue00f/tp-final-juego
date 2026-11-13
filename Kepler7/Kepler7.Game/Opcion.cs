namespace Kepler7.Game;

internal sealed class Opcion : RadioButton
{
    public Opcion(string texto, int ancho = 140, int alto = 26)
    {
        Text = texto;
        Size = new Size(ancho, alto);
    }
}
