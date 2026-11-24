using AspLAb1.Middleware;
using AspLAb1.Models;
using AspLAb1.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspLAb1.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly RequestStats _stats;
    private readonly IBookService _books;

    public DashboardController(RequestStats stats, IBookService books)
    {
        _stats = stats;
        _books = books;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalRequests = _stats.Total;
        ViewBag.BooksCount = await _books.CountAsync();
        ViewBag.AuthorsCount = AuthorStoreMemory.Authors.Count;
        return View(_stats.TopPaths(5));
    }
}
