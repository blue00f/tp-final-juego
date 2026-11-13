namespace Kepler7.Game;

internal sealed class CajaTexto : TextBox
{
    public CajaTexto(bool monoespaciada = true, bool soloLectura = true)
    {
        Multiline = true;
        ReadOnly = soloLectura;
        WordWrap = true;
        ScrollBars = ScrollBars.Vertical;
        BackColor = Tema.Panel;
        ForeColor = Tema.Texto;
        BorderStyle = BorderStyle.FixedSingle;
        Font = monoespaciada ? new Font(Tema.FuenteMono, 9f) : Tema.Crear(9f);
    }
}
