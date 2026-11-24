namespace AspLAb1.Models;

/// <summary>
/// Represents a book stored in the application.
/// </summary>
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Isbn { get; set; } = "";
    public DateTime? PublicationDate { get; set; }
    public int NumPages { get; set; }
    public decimal Price { get; set; }
    public string Author { get; set; } = "";
    public string? PublisherCode { get; set; }
}
