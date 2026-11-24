using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using AspLAb1.Filters;
using AspLAb1.Models;
using AspLAb1.Services;

namespace AspLAb1.Controllers;

[ServiceFilter(typeof(LogActionFilter), Order = 1)]
[MeasureTime(Order = 2)]
public class BookController : Controller
{
    private readonly IBookService _books;

    public BookController(IBookService books) => _books = books;

    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    public IActionResult Status(int code = 200) => StatusCode(code);

    [HttpGet("Book/Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null) return NotFound();

        ViewBag.Id = book.Id;
        return View(book);
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

    public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string? search = null, string? sort = null, string? dir = null)
    {
        var result = await _books.GetPageAsync(page, pageSize, search, sort, dir);
        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.Dir = dir;
        ViewBag.PageSize = pageSize;
        return View(result);
    }

    [HttpGet]
    public IActionResult Create() => View(new BookFormViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(BookFormViewModel vm)
    {
        if (ModelState.IsValid && await _books.IsbnExistsAsync(vm.Isbn))
            ModelState.AddModelError(nameof(vm.Isbn), "Książka z takim numerem ISBN już istnieje.");

        if (!ModelState.IsValid) return View(vm);

        await _books.AddAsync(ToEntity(vm));
        TempData["Success"] = $"Dodano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null) return NotFound();

        return View(ToViewModel(book));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, BookFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (ModelState.IsValid && await _books.IsbnExistsAsync(vm.Isbn, excludeId: id))
            ModelState.AddModelError(nameof(vm.Isbn), "Książka z takim numerem ISBN już istnieje.");

        if (!ModelState.IsValid) return View(vm);

        var updated = await _books.UpdateAsync(ToEntity(vm, id));
        if (!updated) return NotFound();

        TempData["Success"] = $"Zaktualizowano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null) return NotFound();

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null) return NotFound();

        await _books.DeleteAsync(id);
        TempData["Success"] = $"Usunięto książkę „{book.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ByYear(int year)
    {
        var filtered = await _books.GetByYearAsync(year);
        ViewBag.FilterInfo = $"Rok wydania: {year}";
        return View(nameof(Index), new PagedResult<Book>(filtered, 1, Math.Max(1, filtered.Count), filtered.Count));
    }

    public async Task<IActionResult> ByIsbn(string isbn)
    {
        var book = await _books.GetByIsbnAsync(isbn);
        if (book is null) return NotFound();
        return RedirectToAction(nameof(Details), new { id = book.Id });
    }

    [HttpGet("archive/{year:int}/{month:int:range(1,12)}")]
    public async Task<IActionResult> Archive(int year, int month)
    {
        var filtered = await _books.GetByYearAsync(year);
        ViewBag.ArchiveInfo = $"{year}-{month:D2}";
        return View(nameof(Index), new PagedResult<Book>(filtered, 1, Math.Max(1, filtered.Count), filtered.Count));
    }

    public IActionResult Boom() => throw new InvalidOperationException("Celowy wyjątek");

    private static Book ToEntity(BookFormViewModel vm, int id = 0) => new()
    {
        Id = id,
        Title = vm.Title,
        Isbn = vm.Isbn,
        PublicationDate = vm.PublicationDate,
        NumPages = vm.NumPages,
        Price = vm.Price,
        Author = vm.Author,
        PublisherCode = vm.PublisherCode,
        Publisher = vm.Publisher
    };

    private static BookFormViewModel ToViewModel(Book b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        Isbn = b.Isbn,
        PublicationDate = b.PublicationDate,
        NumPages = b.NumPages,
        Price = b.Price,
        Author = b.Author,
        PublisherCode = b.PublisherCode,
        Publisher = b.Publisher
    };
}
