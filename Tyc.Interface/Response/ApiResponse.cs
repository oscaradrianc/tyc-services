namespace Tyc.Interface.Response;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T? Data { get; set; }
}

