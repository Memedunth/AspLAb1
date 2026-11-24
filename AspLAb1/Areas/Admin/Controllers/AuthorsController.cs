using AspLAb1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspLAb1.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthorsController : Controller
{
    public IActionResult Index() => View(AuthorStoreMemory.Authors);

    [HttpGet]
    public IActionResult Create() => View(new Author());

    [HttpPost]
    public IActionResult Create(Author author)
    {
        if (!ModelState.IsValid) return View(author);

        AuthorStoreMemory.Authors.Add(new Author
        {
            Id = AuthorStoreMemory.Authors.Any() ? AuthorStoreMemory.Authors.Max(a => a.Id) + 1 : 1,
            Name = author.Name,
            Country = author.Country
        });

        TempData["Success"] = $"Dodano autora „{author.Name}”.";
        return RedirectToAction(nameof(Index));
    }
}
