using System;
using System.Collections.Generic;
using System.Text;

namespace Tyc.Modelo.Tipos;
public record ImagenEnLinea(byte[] Bytes, string ContentId, string MimeType = "image/png");
