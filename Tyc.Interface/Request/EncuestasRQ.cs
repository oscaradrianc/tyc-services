using ServiceStack;
using System;
using System.Collections.Generic;
using Tyc.Interface.Response.General;

namespace Tyc.Interface.Request;

[Route("/encuestas/asignacion", "POST")]
public class CreateAsignacionRQ : IReturn<ApiResponse<int>>
{
    public int EncuestaId { get; set; }
    public string Nombre { get; set; }
    public DateTime FechaLimiteRespuesta { get; set; }
    public string Observaciones { get; set; }
    public List<Guid> EmpresasIds { get; set; } // Lista de IDs de las empresas (clientes)
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

[Route("/encuestas/{EncuestaId}/estructura", "GET")]
public class GetEncuestaEstructuraRQ : IReturn<ApiResponse<EncuestaEstructuraRS>>
{
    public int EncuestaId { get; set; }
}

// Los Responses anidados
public class EncuestaEstructuraRS
{
    public int IdEncuesta { get; set; }
    public string Nombre { get; set; }
    public string TipoEncuesta { get; set; }
    public string Descripcion { get; set; }
    public List<PreguntaEstructuraRS> Preguntas { get; set; } = new List<PreguntaEstructuraRS>();
}

public class PreguntaEstructuraRS
{
    public int IdPregunta { get; set; }
    public string Pregunta { get; set; }
    public string TipoPregunta { get; set; }
    public int Orden { get; set; }
    public bool EsObligatoria { get; set; }
    public string ReglasValidacion { get; set; } // El frontend puede hacer JSON.parse() de esto
    public List<OpcionEstructuraRS> Opciones { get; set; } = new List<OpcionEstructuraRS>();
}

public class OpcionEstructuraRS
{
    public int IdOpcion { get; set; }
    public string Etiqueta { get; set; }
    public bool ExtraTexto { get; set; }
}