using AspLAb1.Middleware;
using AspLAb1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspLAb1.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly RequestStats _stats;

    public DashboardController(RequestStats stats) => _stats = stats;

    public IActionResult Index()
    {
        ViewBag.TotalRequests = _stats.Total;
        ViewBag.BooksCount = BookStoreMemory.Books.Count;
        ViewBag.AuthorsCount = AuthorStoreMemory.Authors.Count;
        return View(_stats.TopPaths(5));
    }
}
