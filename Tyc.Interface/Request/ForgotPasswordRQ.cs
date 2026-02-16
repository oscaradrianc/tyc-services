using ServiceStack;

namespace Tyc.Interface.Request;

[Route("/auth/forgot-password", "POST")]
public class ForgotPasswordRQ : IReturnVoid
{
    public string Email { get; set; }
    public string UsuaLogin { get; set; } 
}

[Route("/auth/reset-password", "POST")]
public class ResetPasswordRQ : IReturnVoid
{
    public string Token { get; set; }
    public string Email { get; set; }
    public string NewPassword { get; set; }
    public string EmpresaSubdomain { get; set; }
}
