namespace Tyc.Modelo
{    public static class ConstantesTyc
    {
        // Llave de cifrado de los links públicos de firma y de las credenciales SMTP.
        // OJO: rotarla invalida los links ya enviados a clientes y exige re-cifrar
        // SmtpUsuario/SmtpClave con la llave nueva. El fallback existe solo por
        // compatibilidad mientras se rota (ver SECRETOS-ROTACION.md).
        public static readonly string llaveParametroLink =
            System.Environment.GetEnvironmentVariable("TYC_LLAVE_PARAMETRO_LINK") ?? "aF8S-Z";
        public const string tipoTextoSaludoCorreo = "CORREO_SALUDO";
        public const string tipoTextoTextoAlternoCorreo = "CORREO_PIE";
        public const string TEMPLATE_CONSENTIMIENTO = "consentimiento-creado";        
        public const string TEMPLATE_CONSENTIMIENTO_FIRMADO = "consentimiento-firmado";
        public const string TEMPLATE_NUEVA_ENCUESTA = "mail-invitacion";
        public const string TEMPLATE_AGRADECIMIENTO_ENCUESTA = "mail-agradecimiento";
    }
}
