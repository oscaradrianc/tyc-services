using ServiceStack;

namespace Tyc.ws.Features.Examples;

public class GetExamples : Service
{

    [Route("/hello")]
    public sealed class MyRequest : IReturn<MyResponse>
    {
        public required string Name { get; set; }
    }

    public object Any(MyRequest request) 
    {
        var resp = new MyResponse { Result = "Hello, " + request.Name };

        return resp;
    }

    internal sealed class MyResponse
    {
        public required string Result { get; set; }
    }



}

