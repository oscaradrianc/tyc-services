using System.Collections.Generic;

namespace Tyc.Interface.Services;

public interface ITemplateRenderer
{
    string RenderTemplate(string templateName, Dictionary<string, string> valores);
}
