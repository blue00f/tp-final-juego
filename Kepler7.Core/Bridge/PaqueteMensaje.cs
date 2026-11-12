namespace Kepler7.Core.Bridge;

public sealed record PaqueteMensaje(string Tipo, string Cifrado, string TextoPlano, string TextoCifrado, string RecibidoEnTierra);
