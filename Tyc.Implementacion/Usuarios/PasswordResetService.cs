using AdministradorCore.Cifrar;
using ServiceStack;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tyc.Interface.Repositories;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Usuarios;

public class PasswordResetService : IPasswordResetService
{
    private readonly IPasswordResetRepository _resetRepo;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IEmailService _emailService; // tu servicio de email existente
    private readonly ITemplateRenderer _templateRenderer; // para construir el HTML del email

    public PasswordResetService(
        IPasswordResetRepository resetRepo,
        IUsuarioRepository usuarioRepo,
        IEmailService emailService,
        ITemplateRenderer templateRenderer)
    {
        _resetRepo = resetRepo;
        _usuarioRepo = usuarioRepo;
        _emailService = emailService;
        _templateRenderer = templateRenderer;
    }

    public void GenerateResetToken(TycBaseContext context, string usuaLogin, string email, string frontendBaseUrl)
    {
        var usuario = context.GetTable<Usuario>()
            .FirstOrDefault(x => x.UsuaLogin == usuaLogin);       

        if (usuario == null)
            return; // No revelar si el usuaLofin existe

        if(usuario.UsuaEmail.ToLower() != email.ToLower())
            return; // No revelar si el email existe

        _resetRepo.InvalidateTokensByUserId(context, usuario.UsuaUsua);

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        _resetRepo.InsertToken(context, new PasswordResetToken
        {
            UsuarioId = usuario.UsuaUsua,
            TokenHash = tokenHash,
            ExpiraEl = DateTime.UtcNow.AddMinutes(30),
            Usado = false,
            CreadoEl = DateTime.UtcNow
        });

        var resetLink = $"{frontendBaseUrl}/#/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
        var htmlBody = BuildResetEmailHtml(usuario.UsuaNombre, resetLink);
        var vista = _templateRenderer.ConstruirVistaConImagen(htmlBody, []);

        _emailService.EnviarEmailAsync(email, "Recuperación de contraseña", vista);
    }

    public void ResetPassword(TycBaseContext context, string token, string email, string newPassword)
    {
        var tokenHash = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        var resetToken = _resetRepo.GetValidToken(context, tokenHash);

        if (resetToken == null)
            throw HttpError.BadRequest("Token inválido o expirado.");

        var usuario = _usuarioRepo.GetById(context, resetToken.UsuarioId);

        if (usuario == null || usuario.UsuaEmail.ToLower() != email.ToLower())
            throw HttpError.BadRequest("Token inválido o expirado.");

        // Usar el mismo método de hash que usas actualmente para passwords
        string passwordR = new BaseCifrado(Convert.ToDateTime(usuario.UsuaFechaCreacion).ToString("yyyyMMdd")).EncryptSHA512(newPassword);

        _usuarioRepo.CambiarClave(context, usuario.UsuaUsua, passwordR); // hashear antes si aplica

        _resetRepo.MarkAsUsed(context, resetToken.Id);
    }

    private string BuildResetEmailHtml(string nombre, string resetLink)
    {
        return $@"
        <html>
        <body style='font-family: Arial, sans-serif; padding: 20px;'>
            <h2>Recuperación de contraseña</h2>
            <p>Hola {nombre},</p>
            <p>Recibimos una solicitud para restablecer tu contraseña.</p>
            <p>
                <a href='{resetLink}' 
                   style='background-color: #007bff; color: white; padding: 10px 20px; 
                          text-decoration: none; border-radius: 5px; display: inline-block;'>
                    Restablecer contraseña
                </a>
            </p>
            <p>Este enlace expira en 30 minutos.</p>
            <p>Si no solicitaste este cambio, ignora este mensaje.</p>
        </body>
        </html>";
    }

}
