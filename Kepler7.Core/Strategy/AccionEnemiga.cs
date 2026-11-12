using Kepler7.Core.Modelo;

namespace Kepler7.Core.Strategy;

public sealed record AccionEnemiga(TipoAccion Tipo, MiembroEscuadron? Objetivo, string Razon);
