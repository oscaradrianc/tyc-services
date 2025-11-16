using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tycws.Api.Features.Consentimientos.Entities;

namespace Tycws.Api.Features.Consentimientos.Infrastructure;

public class ConsentimientoConfiguration : IEntityTypeConfiguration<Consentimiento>
{
    public void Configure(EntityTypeBuilder<Consentimiento> builder)
    {
        builder.ToTable("tgen_consentimientos", "public");

        builder.HasKey(e => e.ConsConsecuencia);

        builder.Property(e => e.ConsConsecuencia)
            .HasColumnName("cons_cons")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.AgenAgencia)
            .HasColumnName("agen_agen");

        builder.Property(e => e.UsuaUsuario)
            .HasColumnName("usua_usua");

        builder.Property(e => e.TextTerminosAgencia)
            .HasColumnName("text_textterminosagencia");

        builder.Property(e => e.TgeTextTerminoCompartirInfo)
            .HasColumnName("tge_textterminocompartirinfo");

        builder.Property(e => e.TgeTextOfertas)
            .HasColumnName("tge_textofertas");

        builder.Property(e => e.ConsNombre)
            .HasColumnName("cons_nombre")
            .HasMaxLength(200);

        builder.Property(e => e.ConsApellido)
            .HasColumnName("cons_apellido")
            .HasMaxLength(200);

        builder.Property(e => e.ConsEmail)
            .HasColumnName("cons_email")
            .HasMaxLength(200);

        builder.Property(e => e.ConsMovil)
            .HasColumnName("cons_movil")
            .HasMaxLength(200);

        builder.Property(e => e.ConsIdentificacion)
            .HasColumnName("cons_identificacion")
            .HasMaxLength(200);

        builder.Property(e => e.ConsFechaCreacionConsentimiento)
            .HasColumnName("cons_fechacreacionconsentimient");

        builder.Property(e => e.ConsFechaAceptacionConsentimiento)
            .HasColumnName("cons_fechaaceptacionconsentimie");

        builder.Property(e => e.ConsAceptoTerminosAgencia)
            .HasColumnName("cons_aceptoterminosagencia")
            .HasMaxLength(2);

        builder.Property(e => e.ConsAceptoTerminosCompartirInfo)
            .HasColumnName("cons_aceptoterminoscompartirinf")
            .HasMaxLength(2);

        builder.Property(e => e.ConsAceptoTerminosRecibirOfertas)
            .HasColumnName("cons_aceptoterminosrecibirofert")
            .HasMaxLength(2);

        builder.Property(e => e.ConsContactabilidadSms)
            .HasColumnName("cons_contactabilidadsms")
            .HasMaxLength(2);

        builder.Property(e => e.ConsContactabilidadWhatsapp)
            .HasColumnName("cons_contactabilidadwhatsappp")
            .HasMaxLength(2);

        builder.Property(e => e.ConsContactabilidadEmail)
            .HasColumnName("cons_contactabilidademail")
            .HasMaxLength(2);

        builder.Property(e => e.ConsContactabilidadMovil)
            .HasColumnName("cons_contactabilidadmovil")
            .HasMaxLength(2);

        builder.Property(e => e.ConsGuid)
            .HasColumnName("cons_guid");

        builder.Property(e => e.ConsMedio)
            .HasColumnName("cons_medio")
            .HasMaxLength(20);
    }
}
