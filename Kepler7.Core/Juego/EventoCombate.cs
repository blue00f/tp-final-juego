using Kepler7.Core.Modelo;

namespace Kepler7.Core.Juego;

public sealed record EventoCombate(TipoEvento Tipo, Personaje Objetivo, int Valor, string Texto);
