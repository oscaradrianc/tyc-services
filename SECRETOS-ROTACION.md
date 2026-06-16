# Rotación de secretos expuestos — pendiente manual

Origen: auditoría 2026-06-10, hallazgo F1 (secretos productivos commiteados).
El código ya quedó preparado: `appsettings.json` tiene los valores en blanco y todo
se lee de variables de entorno (ver `produccion.env.example`). **Falta la rotación
manual de cada secreto y la purga del historial git.**

## Secretos comprometidos (asumir públicos)

| Secreto | Dónde estaba | Acción de rotación |
|---|---|---|
| `jwt.AuthKeyBase64` ("tiralaliratararalala") | appsettings.json | Generar 256 bits aleatorios: `openssl rand -base64 32`. Rotar invalida sesiones activas (los usuarios re-loguean). |
| `secret` (valor de ejemplo de jwt.io) | appsettings.json | Igual que el anterior. |
| reCAPTCHA SecretKey | appsettings.json | Consola Google reCAPTCHA → regenerar o crear sitio nuevo. |
| SMTP AWS SES (SmtpUsuario/SmtpClave) | appsettings.json (cifradas con "aF8S-Z" del mismo repo) | Consola AWS IAM → borrar credenciales SMTP y crear nuevas. Decidir: guardarlas en claro en el env (recomendado, el env ya es secreto) o re-cifradas con la llave nueva. |
| Twilio WPSid/WPToken | appsettings.json | Consola Twilio → rotar auth token. |
| VapidPrivate | appsettings.json | Generar par VAPID nuevo (invalida suscripciones push existentes; regenerarlas). |
| `llaveParametroLink` ("aF8S-Z") | ConstantesTyc.cs | Definir `TYC_LLAVE_PARAMETRO_LINK` fuerte. **OJO**: los links de firma ya enviados quedan inválidos → rotar en ventana coordinada o re-emitir links pendientes. Alternativa de fondo (Fase 2): reemplazar por `cons_guid` opaco. |
| `produccion.env` completo (connection string BD, Redis, licencias) | repo raíz | Rotar clave del usuario de BD y config Redis. Ya está gitignored; sacarlo del índice: `git rm --cached produccion.env`. |
| `NUGET_KEY` / clave DevExpress en URL | Dockerfile (ARG) | Verificar que no quedó en historial de imágenes; rotar PAT de GitHub si aplica. |

## Purga del historial git

Después de rotar (no antes — purgar sin rotar no sirve):

```bash
# Con BFG (https://rtyley.github.io/bfg-repo-cleaner/)
bfg --replace-text secretos.txt   # archivo con los valores viejos
git reflog expire --expire=now --all && git gc --prune=now --aggressive
git push --force
```

Avisar al equipo: todos deben re-clonar después del force push.

## Verificación post-rotación

1. `dotnet run` local con `produccion.env` nuevo → login admin funciona (JWT nuevo).
2. Crear consentimiento de prueba → correo llega (SMTP nuevo).
3. Link del correo abre el formulario (llave de link consistente).
4. Notificación push de prueba (VAPID nuevo).
