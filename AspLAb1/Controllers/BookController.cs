using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AspLAb1.Models; 

namespace AspLAb1.Controllers;

public class BookController : Controller
{
    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    public IActionResult Status(int code = 200) => StatusCode(code);

    public IActionResult Details(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    public IActionResult Echo([FromQuery] string name = "nieznajomy", [FromQuery] int count = 1) =>
        Content(string.Join("\n", Enumerable.Repeat($"Cześć, {name}!", Math.Clamp(count, 1, 20))));

    public IActionResult Square(int id) => Content($"{id}^2 = {id * id}");

    [HttpPost]
    public IActionResult Import([FromBody] BookDto dto) =>
        Json(new { received = dto.Title, dto.Isbn, dto.Price });

    public IActionResult Inspect()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Method: {Request.Method}");
        sb.AppendLine($"Path:   {Request.Path}{Request.QueryString}");
        foreach (var h in Request.Headers) sb.AppendLine($"{h.Key}: {h.Value}");
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    public IActionResult Sum(int a, int b) => Content((a + b).ToString());

    public IActionResult UserAgent()
    {
        var agent = Request.Headers.UserAgent.ToString();
        return Content($"Twoja przeglądarka/klient to: {agent}", "text/plain; charset=utf-8");
    }

    public IActionResult Health() => Json(new { status = "ok", time = DateTime.Now });

    public IActionResult Index() => View(BookStoreMemory.Books);

    [HttpGet]
    public IActionResult Create() => View(new BookFormViewModel());

    [HttpPost]
    public IActionResult Create(BookFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        BookStoreMemory.Books.Add(new Book
        {
            Id = BookStoreMemory.Books.Any() ? BookStoreMemory.Books.Max(b => b.Id) + 1 : 1,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author,
            PublisherCode = vm.PublisherCode
        });

        TempData["Success"] = $"Dodano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }
}