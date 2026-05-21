using System;

namespace Tyc.Implementacion.Helpers;

internal static class IdUtils
{
    /// <summary>
    /// El usua_usua en BD transaccional viene concatenado con el ID de empresa como prefijo.
    /// Este método extrae el ID local del usuario.
    /// </summary>
    internal static int ExtraerIdUsuario(int usuarioId, int empresaId)
    {
        var concatenado = usuarioId.ToString().AsSpan();
        var prefijo = empresaId.ToString().AsSpan();

        if (!concatenado.StartsWith(prefijo))
            throw new InvalidOperationException("Prefijo inválido");

        return int.Parse(concatenado[prefijo.Length..]);
    }
}
