using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyc.Modelo.Response;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T? Data { get; set; }
}

