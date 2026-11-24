using AspLAb1.Data;
using AspLAb1.Models;
using Microsoft.EntityFrameworkCore;

namespace AspLAb1.Services;

/// <summary>
/// EF Core implementation of <see cref="IBookService"/>.
/// </summary>
public class BookService : IBookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db) => _db = db;

    public async Task<PagedResult<Book>> GetPageAsync(int page, int pageSize, string? search = null, string? sort = null, string? dir = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<Book> q = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            q = q.Where(b => b.Title.Contains(term) || b.Isbn.Contains(term));
        }

        var total = await q.CountAsync();

        var descending = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);
        q = (sort?.ToLowerInvariant(), descending) switch
        {
            ("price", true)  => q.OrderByDescending(b => (double)b.Price).ThenBy(b => b.Id),
            ("price", false) => q.OrderBy(b => (double)b.Price).ThenBy(b => b.Id),
            ("date", true)   => q.OrderByDescending(b => b.PublicationDate).ThenBy(b => b.Id),
            ("date", false)  => q.OrderBy(b => b.PublicationDate).ThenBy(b => b.Id),
            ("title", true)  => q.OrderByDescending(b => b.Title).ThenBy(b => b.Id),
            _                => q.OrderBy(b => b.Title).ThenBy(b => b.Id)
        };

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Book>(items, page, pageSize, total);
    }

    public Task<Book?> GetByIdAsync(int id) =>
        _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        var normalized = NormalizeIsbn(isbn);
        return await _db.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Isbn.Replace("-", "").Replace(" ", "") == normalized);
    }

    public async Task<IReadOnlyList<Book>> GetByYearAsync(int year) =>
        await _db.Books.AsNoTracking()
            .Where(b => b.PublicationDate.HasValue && b.PublicationDate.Value.Year == year)
            .OrderBy(b => b.Title)
            .ToListAsync();

    public Task<int> CountAsync() => _db.Books.CountAsync();

    public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
    {
        var normalized = NormalizeIsbn(isbn);
        return await _db.Books.AsNoTracking()
            .AnyAsync(b => b.Isbn.Replace("-", "").Replace(" ", "") == normalized
                        && (!excludeId.HasValue || b.Id != excludeId.Value));
    }

    public async Task<Book> AddAsync(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        var existing = await _db.Books.FindAsync(book.Id);
        if (existing is null) return false;

        existing.Title = book.Title;
        existing.Isbn = book.Isbn;
        existing.PublicationDate = book.PublicationDate;
        existing.NumPages = book.NumPages;
        existing.Price = book.Price;
        existing.Author = book.Author;
        existing.PublisherCode = book.PublisherCode;
        existing.Publisher = book.Publisher;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Books.FindAsync(id);
        if (existing is null) return false;

        _db.Books.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }

    private static string NormalizeIsbn(string isbn) => isbn.Replace("-", "").Replace(" ", "");
}
