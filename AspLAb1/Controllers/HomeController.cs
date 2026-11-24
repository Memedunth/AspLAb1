using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AspLAb1.Models;

namespace AspLAb1.Controllers;

public class HomeController : Controller
{
    private readonly IActionDescriptorCollectionProvider _actionProvider;

    public HomeController(IActionDescriptorCollectionProvider actionProvider)
    {
        _actionProvider = actionProvider;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Docs(string? path) =>
        Content($"path = {path ?? "(brak)"}", "text/plain; charset=utf-8");

    public IActionResult Routes()
    {
        var sb = new StringBuilder();
        sb.AppendLine("| Adres | Kontroler.Akcja | Metoda HTTP |");
        sb.AppendLine("| --- | --- | --- |");

        var descriptors = _actionProvider.ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .OrderBy(d => d.ControllerName)
            .ThenBy(d => d.ActionName);

        foreach (var d in descriptors)
        {
            var area = d.RouteValues.TryGetValue("area", out var a) && !string.IsNullOrEmpty(a) ? a : null;
            var actionFullName = area is not null
                ? $"{area}/{d.ControllerName}.{d.ActionName}"
                : $"{d.ControllerName}.{d.ActionName}";

            string address;
            if (d.AttributeRouteInfo?.Template is string attrTemplate)
            {
                address = "/" + attrTemplate.TrimStart('/');
            }
            else if (d.ControllerName == "Book" && d.ActionName == "ByYear")
            {
                address = "/books/year/{year:int:min(1450)}";
            }
            else if (d.ControllerName == "Book" && d.ActionName == "ByIsbn")
            {
                address = "/books/isbn/{isbn:regex(^\\d{13}$)}";
            }
            else if (d.ControllerName == "Home" && d.ActionName == "Docs")
            {
                address = "/docs/{*path}";
            }
            else if (area is not null)
            {
                address = $"/{area}/{d.ControllerName}/{d.ActionName}/{{id?}}";
            }
            else
            {
                address = $"/{d.ControllerName}/{d.ActionName}/{{id?}}";
            }

            var methods = d.ActionConstraints?
                .OfType<HttpMethodActionConstraint>()
                .SelectMany(c => c.HttpMethods)
                .Distinct()
                .ToList();
            var httpMethod = methods is { Count: > 0 } ? string.Join(", ", methods) : "ANY";

            sb.AppendLine($"| `{address}` | `{actionFullName}` | {httpMethod} |");
        }

        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    public IActionResult HttpStatus(int code)
    {
        Response.StatusCode = code;
        ViewBag.StatusCode = code;
        ViewBag.Message = code switch
        {
            404 => "Nie znaleziono strony lub zasobu.",
            403 => "Brak uprawnień do tego zasobu.",
            401 => "Wymagane logowanie.",
            _ => "Serwer zwrócił kod błędu."
        };
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
