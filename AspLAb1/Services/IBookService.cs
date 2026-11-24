using AspLAb1.Models;

namespace AspLAb1.Services;

/// <summary>
/// Service abstraction for managing books in persistent storage.
/// </summary>
public interface IBookService
{
    Task<PagedResult<Book>> GetPageAsync(int page, int pageSize, string? search = null, string? sort = null, string? dir = null);
    Task<Book?> GetByIdAsync(int id);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<IReadOnlyList<Book>> GetByYearAsync(int year);
    Task<int> CountAsync();
    Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null);
    Task<Book> AddAsync(Book book);
    Task<bool> UpdateAsync(Book book);
    Task<bool> DeleteAsync(int id);
}
