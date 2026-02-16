using System;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

[Table(Name = "password_reset_tokens")]
public class PasswordResetToken
{
    [Column(Name = "id", IsPrimaryKey = true, IsDbGenerated = true)]
    public int Id { get; set; }

    [Column(Name = "usua_usua")]
    public int UsuarioId { get; set; }

    [Column(Name = "token_hash")]
    public string TokenHash { get; set; }

    [Column(Name = "expira")]
    public DateTime ExpiraEl { get; set; }

    [Column(Name = "usado")]
    public bool Usado { get; set; }

    [Column(Name = "creado")]
    public DateTime CreadoEl { get; set; }
}
