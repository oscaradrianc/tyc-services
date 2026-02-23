using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;
using Tyc.Interface.Response.General;

namespace Tyc.Interface.Request;

[Route("/encuestas/asignacion", "POST")]
public class CreateAsignacionRQ : IReturn<ApiResponse<int>>
{
    public int EncuestaId { get; set; }
    public string Nombre { get; set; }
    public DateTime FechaLimiteRespuesta { get; set; }
    public string Observaciones { get; set; }
    public List<int> EmpresasIds { get; set; } // Lista de IDs de las empresas (clientes)
}

[Route("/encuestas/respuesta", "POST")]
public class SaveRespuestaRQ : IReturn<ApiResponse<int>>
{
    public int DetalleId { get; set; } // El ID que relaciona la agencia con la asignación
    public List<RespuestaItemRQ> Respuestas { get; set; }
}

public class RespuestaItemRQ
{
    public int PreguntaId { get; set; }
    public int? OpcionId { get; set; }
    public string ValorTexto { get; set; }
    public decimal? ValorNumerico { get; set; }
}

