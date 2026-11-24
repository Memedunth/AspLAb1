namespace AspLAb1.Models;

/// <summary>
/// In-memory store for authors used in Lab 3 Admin area.
/// </summary>
public static class AuthorStoreMemory
{
    public static readonly List<Author> Authors = new()
    {
        new() { Id = 1, Name = "Stanisław Lem", Country = "Polska" },
        new() { Id = 2, Name = "Bolesław Prus", Country = "Polska" },
        new() { Id = 3, Name = "Frank Herbert", Country = "USA" }
    };
}
