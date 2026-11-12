namespace Kepler7.Game;

internal sealed class Boton : Button
{
    public Boton(string texto, int ancho = 140, int alto = 32)
    {
        Text = texto;
        Size = new Size(ancho, alto);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderColor = Tema.Borde;
        FlatAppearance.MouseOverBackColor = Tema.Mezclar(Tema.PanelClaro, Color.White, 0.08);
        FlatAppearance.MouseDownBackColor = Tema.Mezclar(Tema.PanelClaro, Color.White, 0.15);
        BackColor = Tema.PanelClaro;
        ForeColor = Tema.Texto;
        UseVisualStyleBackColor = false;
    }
}
