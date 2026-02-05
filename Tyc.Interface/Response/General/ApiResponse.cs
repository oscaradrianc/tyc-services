namespace Tyc.Interface.Response.General;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T Data { get; set; }
}

