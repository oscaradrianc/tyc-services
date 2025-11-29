using ServiceStack;
using ServiceStack.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyc.Interface.Response;
using static ServiceStack.HttpContextFactory;

namespace Tyc.Interface.Request;

[Route("/textos/{Id}", "GET")]
public class GetTexto : IReturn<TextoResponse>
{
    public int Id { get; set; }
}

[Route("/textos", "GET")]
public class ListTextos : IReturn<ApiResponse<TextoResponse>>
{
    public int? EmpresaId { get; set; }
    public bool SoloActivos { get; set; } = true;
}

[Route("/textos/Empresa/{EmpresaId}/tipo/{TipoTexto}", "GET")]
public class GetTextoByEmpresaYTipo : IReturn<TextoResponse>
{
    public int EmpresaId { get; set; }
    public string TipoTexto { get; set; }
}

[Route("/textos", "POST")]
public class CreateTexto : IReturn<TextoResponse>
{
    public int EmpresaId { get; set; }
    public string TipoTexto { get; set; }
    public string TextoTerminos { get; set; }
}

[Route("/textos/{Id}", "PUT")]
public class UpdateTexto : IReturn<TextoResponse>
{
    public int Id { get; set; }
    public string TipoTexto { get; set; }
    public string TextoTerminos { get; set; }
    public string Estado { get; set; }
}

[Route("/textos/{Id}", "DELETE")]
public class DeleteTexto : IReturn<ApiResponse<object>>
{
    public int Id { get; set; }
}

[Route("/textos/{Id}/estado", "PUT")]
public class CambiarEstadoTexto : IReturn<ApiResponse<object>>
{
    public int Id { get; set; }
    public string Estado { get; set; }
}
