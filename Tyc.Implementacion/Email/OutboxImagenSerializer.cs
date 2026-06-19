using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Tyc.Modelo.Tipos;

namespace Tyc.Implementacion.Email;

/// <summary>
/// (De)serializa las imágenes inline de un correo del outbox (auditoría F8).
/// Formato persistido en <c>email_outbox.imagenes_json</c>: <c>[{cid, mime, b64}]</c>.
/// Compartido por el servicio de encolado y el worker de drenado para que ambos
/// usen exactamente la misma forma.
/// </summary>
public static class OutboxImagenSerializer
{
    private sealed record ImagenDto(string Cid, string Mime, string B64);

    public static string Serializar(IReadOnlyList<ImagenEnLinea> imagenes)
    {
        if (imagenes == null || imagenes.Count == 0)
            return null;

        var dtos = imagenes
            .Where(i => i?.Bytes != null && i.Bytes.Length > 0)
            .Select(i => new ImagenDto(i.ContentId, i.MimeType, Convert.ToBase64String(i.Bytes)))
            .ToList();

        return dtos.Count == 0 ? null : JsonSerializer.Serialize(dtos);
    }

    public static List<ImagenEnLinea> Deserializar(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<ImagenEnLinea>();

        var dtos = JsonSerializer.Deserialize<List<ImagenDto>>(json) ?? new List<ImagenDto>();
        return dtos
            .Select(d => new ImagenEnLinea(Convert.FromBase64String(d.B64), d.Cid, d.Mime ?? "image/png"))
            .ToList();
    }
}
