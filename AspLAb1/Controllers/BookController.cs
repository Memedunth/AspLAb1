using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AspLAb1.Filters;
using AspLAb1.Models;

namespace AspLAb1.Controllers;

[ServiceFilter(typeof(LogActionFilter), Order = 1)]
[MeasureTime(Order = 2)]
public class BookController : Controller
{
    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    public IActionResult Status(int code = 200) => StatusCode(code);

    [HttpGet("Book/Details/{id:int}")]
    public Task<IActionResult> Details(int id)
    {
        var book = BookStoreMemory.Books.FirstOrDefault(b => b.Id == id);
        if (book is null) return Task.FromResult<IActionResult>(NotFound());

        ViewBag.Id = book.Id;
        return Task.FromResult<IActionResult>(View(book));
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

    public Task<IActionResult> ByYear(int year)
    {
        var filtered = BookStoreMemory.Books
            .Where(b => b.PublicationDate?.Year == year)
            .ToList();
        ViewBag.FilterInfo = $"Rok wydania: {year}";
        return Task.FromResult<IActionResult>(View(nameof(Index), filtered));
    }

    public Task<IActionResult> ByIsbn(string isbn)
    {
        var normalized = isbn.Replace("-", "").Replace(" ", "");
        var book = BookStoreMemory.Books.FirstOrDefault(
            b => b.Isbn.Replace("-", "").Replace(" ", "") == normalized);

        if (book is null) return Task.FromResult<IActionResult>(NotFound());
        return Task.FromResult<IActionResult>(RedirectToAction(nameof(Details), new { id = book.Id }));
    }

    [HttpGet("archive/{year:int}/{month:int:range(1,12)}")]
    public IActionResult Archive(int year, int month)
    {
        var filtered = BookStoreMemory.Books
            .Where(b => b.PublicationDate?.Year == year)
            .ToList();
        ViewBag.ArchiveInfo = $"{year}-{month:D2}";
        return View(nameof(Index), filtered);
    }

    public IActionResult Boom() => throw new InvalidOperationException("Celowy wyjątek");
}