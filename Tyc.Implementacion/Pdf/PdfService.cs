// Tyc.Implementacion/Pdf/PdfService.cs
using AdministradorCore.Cifrar;
using DevExpress.DataAccess.Native;
using DevExpress.XtraRichEdit.Import.Doc;
using HtmlAgilityPack;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Tyc.Interface.Repositories;
using Tyc.Interface.Response.Pdf;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Pdf;

public class PdfService : IPdfService
{
    private readonly IConsentimientoRepository _consentimientoRepository;
    private readonly ITextoRepository _textoRepository;
    private readonly IFirmaRepository _firmaRepository;
    private readonly IEmpresaRepository _empresaRepository;

    // Colores corporativos
    private static readonly string ColorPrimario = "#2c3e50";
    private static readonly string ColorSecundario = "#6c757d";
    private static readonly string ColorFondo = "#f8f9fa";
    private static readonly string ColorBorde = "#dee2e6";

    public PdfService(
        IConsentimientoRepository consentimientoRepository,
        ITextoRepository textoRepository,
        IFirmaRepository firmaRepository,
        IEmpresaRepository empresaRepository)
    {
        _consentimientoRepository = consentimientoRepository;
        _textoRepository = textoRepository;
        _firmaRepository = firmaRepository;
        _empresaRepository = empresaRepository;

        // Licencia Community (gratis para empresas < $1M revenue)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerarConsentimientoPdfAsync(TycBaseContext context, Guid consentimientoId, int textoId)
    {
        var consentimiento = await _consentimientoRepository.GetByGuidAsync(context, consentimientoId)
            ?? throw new InvalidOperationException($"Consentimiento {consentimientoId} no encontrado");

        var texto = await _textoRepository.GetByIdAsync(context, textoId)
            ?? throw new InvalidOperationException($"Texto {textoId} no encontrado");

        var empresa = await _empresaRepository.GetByIdAsync(context, consentimiento.EmpresaId);
        var firma = _firmaRepository.GetByConsentimiento(context, consentimiento.Id);

        var usuario = context.GetTable<Usuario>()
            .FirstOrDefault(u => u.UsuaUsua == consentimiento.UsuarioId);

        TipoIdentificacion tipoDoc = null;
        if (consentimiento.TipoIdentificacion1.HasValue)
        {
            tipoDoc = await _consentimientoRepository.GetTipoIdentificacionAsync(
                context, consentimiento.EmpresaId, consentimiento.TipoIdentificacion1.Value);
        }

        var data = BuildPdfData(consentimiento, texto, empresa, firma, usuario, tipoDoc);

        return GenerarPdf(data);
    }

    private byte[] GenerarPdf(ConsentimientoPdfData data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                // Header
                page.Header().Element(c => ComposeHeader(c, data));

                // Content
                page.Content().Element(c => ComposeContent(c, data));

                // Footer
                page.Footer().Element(c => ComposeFooter(c, data));
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, ConsentimientoPdfData data)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // Logo
                var logoBytes = TryGetLogoBytes(data.LogoEmpresa);
                if (logoBytes != null)
                {
                    row.ConstantItem(80).Height(50).Image(logoBytes, ImageScaling.FitArea);
                }

                row.RelativeItem().Column(innerCol =>
                {
                    innerCol.Item().AlignRight().Text(data.NombreEmpresa ?? "")
                        .FontSize(14).Bold().FontColor(Color.FromHex(ColorPrimario));
                });
            });

            col.Item().PaddingTop(10).LineHorizontal(2).LineColor(Color.FromHex(ColorPrimario));

            col.Item().PaddingTop(15).AlignCenter()
                .Text("CONSTANCIA DE CONSENTIMIENTO")
                .FontSize(14).Bold().FontColor(Color.FromHex(ColorPrimario));
        });
    }

    private void ComposeContent(IContainer container, ConsentimientoPdfData data)
    {
        container.PaddingTop(20).Column(col =>
        {
            // Card de datos del cliente
            col.Item().Element(c => ComposeDatosCliente(c, data));

            // Sección de firma
            col.Item().PaddingTop(15).Element(c => ComposeFirma(c, data));

            // Política/Términos
            col.Item().PaddingTop(25).Element(c => ComposePolitica(c, data));
        });
    }

    private void ComposeDatosCliente(IContainer container, ConsentimientoPdfData data)
    {
        container
            .Background(Color.FromHex(ColorFondo))
            .Border(1)
            .BorderColor(Color.FromHex(ColorBorde))
            .Padding(15)
            .Column(col =>
            {
                col.Item().Text("DATOS DEL CONSENTIMIENTO")
                    .FontSize(11).Bold().FontColor(Color.FromHex(ColorPrimario));

                col.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    // Fila 1
                    AddDataRow(table, "Nombre del cliente:", data.NombreCliente);
                    AddDataRow(table, "Documento:", $"{data.TipoDocumento} {data.Documento}");

                    // Fila 2
                    AddDataRow(table, "Estado:", data.Estado);
                    AddDataRow(table, "Fecha de creación:", data.FechaCreacion.ToString("dd/MM/yyyy hh:mm tt"));

                    // Fila 3
                    AddDataRow(table, "Usuario creó:", data.UsuarioCreo ?? "-");
                    AddDataRow(table, "IP Firma:", data.IpFirma ?? "-");

                    // Fila 4
                    AddDataRow(table, "Medio de Firma:", data.MedioFirma ?? "-");
                    AddDataRow(table, "", ""); // Celda vacía para balance
                });
            });
    }

    private void AddDataRow(TableDescriptor table, string label, string value)
    {
        table.Cell().Padding(5).Column(col =>
        {
            if (!string.IsNullOrEmpty(label))
            {
                col.Item().Text(label).FontSize(9).FontColor(Color.FromHex(ColorSecundario));
                col.Item().Text(value ?? "-").FontSize(10).Bold();
            }
        });
    }

    private void ComposeFirma(IContainer container, ConsentimientoPdfData data)
    {
        container
            .Background(Color.FromHex(ColorFondo))
            .Border(1)
            .BorderColor(Color.FromHex(ColorBorde))
            .Padding(15)
            .Row(row =>
            {
                // Columna firma
                row.RelativeItem(2).Column(col =>
                {
                    col.Item().Text("Firma:").FontSize(9).FontColor(Color.FromHex(ColorSecundario));

                    if (data.FirmaImagen != null && data.FirmaImagen.Length > 0)
                    {
                        col.Item().PaddingTop(5).Height(60).Image(data.FirmaImagen, ImageScaling.FitArea);
                    }
                    else
                    {
                        col.Item().PaddingTop(5).Text("Sin firma")
                            .FontSize(10).Italic().FontColor(Color.FromHex(ColorSecundario));
                    }
                });

                // Columna fechas firma
                if (data.FechaFirma.HasValue)
                {
                    row.RelativeItem(1).PaddingLeft(20).Column(col =>
                    {
                        col.Item().Text("Fecha de firma:")
                            .FontSize(9).FontColor(Color.FromHex(ColorSecundario));
                        col.Item().Text(data.FechaFirma.Value.ToString("dd/MM/yyyy"))
                            .FontSize(10).Bold();

                        col.Item().PaddingTop(10).Text("Hora de firma:")
                            .FontSize(9).FontColor(Color.FromHex(ColorSecundario));
                        col.Item().Text(data.FechaFirma.Value.ToString("hh:mm tt"))
                            .FontSize(10).Bold();
                    });
                }
            });
    }

    private void ComposePolitica(IContainer container, ConsentimientoPdfData data)
    {
        container.Column(col =>
        {
            col.Item()
                .BorderBottom(1)
                .BorderColor(Color.FromHex(ColorBorde))
                .PaddingBottom(5)
                .Text("TÉRMINOS Y CONDICIONES")
                .FontSize(12).Bold().FontColor(Color.FromHex(ColorPrimario));

            col.Item().PaddingTop(10).Element(c => RenderHtmlContent(c, data.PoliticaHtml));
        });
    }

    private void RenderHtmlContent(IContainer container, string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            container.Text("Sin contenido");
            return;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        container.Column(col =>
        {
            RenderNodes(col, doc.DocumentNode.ChildNodes);
        });
    }

    private void RenderNodes(ColumnDescriptor col, HtmlNodeCollection nodes)
    {
        foreach (var node in nodes)
        {
            RenderNode(col, node);
        }
    }

    private void RenderNode(ColumnDescriptor col, HtmlNode node)
    {
        switch (node.NodeType)
        {
            case HtmlNodeType.Text:
                var text = HtmlEntity.DeEntitize(node.InnerText)?.Trim();
                if (!string.IsNullOrEmpty(text))
                {
                    col.Item().Text(text).FontSize(10);
                }
                break;

            case HtmlNodeType.Element:
                switch (node.Name.ToLower())
                {
                    case "p":
                    case "div":
                        col.Item().PaddingBottom(8).Element(c => RenderInlineContent(c, node));
                        break;

                    case "br":
                        col.Item().PaddingBottom(5);
                        break;

                    case "h1":
                        col.Item().PaddingTop(10).PaddingBottom(5)
                            .Text(GetNodeText(node)).FontSize(16).Bold();
                        break;

                    case "h2":
                        col.Item().PaddingTop(8).PaddingBottom(4)
                            .Text(GetNodeText(node)).FontSize(14).Bold();
                        break;

                    case "h3":
                        col.Item().PaddingTop(6).PaddingBottom(3)
                            .Text(GetNodeText(node)).FontSize(12).Bold();
                        break;

                    case "ul":
                        col.Item().PaddingLeft(15).PaddingBottom(8).Column(listCol =>
                        {
                            foreach (var li in node.SelectNodes(".//li") ?? Enumerable.Empty<HtmlNode>())
                            {
                                listCol.Item().Row(row =>
                                {
                                    row.ConstantItem(15).Text("•").FontSize(10);
                                    row.RelativeItem().Text(GetNodeText(li)).FontSize(10);
                                });
                            }
                        });
                        break;

                    case "ol":
                        col.Item().PaddingLeft(15).PaddingBottom(8).Column(listCol =>
                        {
                            var items = node.SelectNodes(".//li")?.ToList() ?? new List<HtmlNode>();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var idx = i;
                                listCol.Item().Row(row =>
                                {
                                    row.ConstantItem(20).Text($"{idx + 1}.").FontSize(10);
                                    row.RelativeItem().Text(GetNodeText(items[idx])).FontSize(10);
                                });
                            }
                        });
                        break;

                    case "strong":
                    case "b":
                        col.Item().Text(GetNodeText(node)).FontSize(10).Bold();
                        break;

                    case "em":
                    case "i":
                        col.Item().Text(GetNodeText(node)).FontSize(10).Italic();
                        break;

                    default:
                        // Procesar hijos recursivamente
                        if (node.HasChildNodes)
                        {
                            RenderNodes(col, node.ChildNodes);
                        }
                        break;
                }
                break;
        }
    }

    private void RenderInlineContent(IContainer container, HtmlNode node)
    {
        container.Text(text =>
        {
            RenderInlineNodes(text, node.ChildNodes);
        });
    }

    private void RenderInlineNodes(TextDescriptor text, HtmlNodeCollection nodes)
    {
        foreach (var node in nodes)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Text:
                    var content = HtmlEntity.DeEntitize(node.InnerText);
                    if (!string.IsNullOrEmpty(content))
                    {
                        text.Span(content).FontSize(10);
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name.ToLower())
                    {
                        case "strong":
                        case "b":
                            text.Span(GetNodeText(node)).FontSize(10).Bold();
                            break;

                        case "em":
                        case "i":
                            text.Span(GetNodeText(node)).FontSize(10).Italic();
                            break;

                        case "u":
                            text.Span(GetNodeText(node)).FontSize(10).Underline();
                            break;

                        case "br":
                            text.Span("\n");
                            break;

                        case "a":
                            var href = node.GetAttributeValue("href", "");
                            text.Hyperlink(GetNodeText(node), href).FontSize(10).FontColor(Colors.Blue.Medium);
                            break;

                        default:
                            if (node.HasChildNodes)
                            {
                                RenderInlineNodes(text, node.ChildNodes);
                            }
                            else
                            {
                                text.Span(GetNodeText(node)).FontSize(10);
                            }
                            break;
                    }
                    break;
            }
        }
    }

    private string GetNodeText(HtmlNode node)
    {
        return HtmlEntity.DeEntitize(node.InnerText)?.Trim() ?? "";
    }

    private void ComposeFooter(IContainer container, ConsentimientoPdfData data)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(Color.FromHex(ColorBorde));

            col.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span($"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .FontSize(8).FontColor(Color.FromHex(ColorSecundario));
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span(data.NombreEmpresa ?? "")
                        .FontSize(8).FontColor(Color.FromHex(ColorSecundario));
                });
            });

            col.Item().AlignRight().Text(text =>
            {
                text.CurrentPageNumber().FontSize(8);
                text.Span(" / ").FontSize(8);
                text.TotalPages().FontSize(8);
            });
        });
    }

    private ConsentimientoPdfData BuildPdfData(
        Consentimiento c, Texto texto, Empresa empresa,
        Firma firma, Usuario usuario, TipoIdentificacion tipoDoc)
    {
        var cifrador = new CifradoHelper(c.EmpresaId.ToString());

        string nombreCliente = c.TipoPersona == "N"
            ? $"{cifrador.Descifrar(c.NombreCliente)} {cifrador.Descifrar(c.ApellidoCliente)}".Trim()
            : cifrador.Descifrar(c.RazonSocial);

        return new ConsentimientoPdfData
        {
            NombreCliente = nombreCliente,
            Documento = cifrador.Descifrar(c.IdentificacionCliente),
            TipoDocumento = tipoDoc?.Descripcion,
            Estado = c.Estado switch { "F" => "Aceptado", "R" => "Rechazado", _ => "Pendiente" },
            FechaCreacion = c.FechaCreacion,
            FechaFirma = c.FechaAceptacion,
            UsuarioCreo = usuario != null ? $"{usuario.UsuaNombre} {usuario.UsuaApellido}" : null,
            IpFirma = c.DireccionIP,
            MedioFirma = c.MedioAceptacion,
            FirmaImagen = firma?.FirmBlob,
            PoliticaHtml = texto.TextTextoDelosTerminos,
            TipoPersona = c.TipoPersona,
            LogoEmpresa = empresa?.LogoBase64,
            NombreEmpresa = empresa?.Nombre
        };
    }

    private class CifradoHelper
    {
        private readonly string _llave;
        public CifradoHelper(string llave) => _llave = llave;

        public string Descifrar(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return valor;
            try { return new BaseCifrado(_llave).Decrypt256(valor, true); }
            catch { return valor; }
        }
    }

    private byte[] TryGetLogoBytes(string logoBase64)
    {
        if (string.IsNullOrWhiteSpace(logoBase64))
            return null;

        try
        {
            // Limpiar prefijo data:image/xxx;base64, si existe
            var base64Data = logoBase64;

            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            // Limpiar espacios y saltos de línea
            base64Data = base64Data.Trim().Replace("\n", "").Replace("\r", "").Replace(" ", "");

            return Convert.FromBase64String(base64Data);
        }
        catch (FormatException)
        {
            // Logo inválido, continuar sin logo
            return null;
        }
    }
}