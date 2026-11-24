using System.ComponentModel.DataAnnotations;

namespace AspLAb1.Models;

/// <summary>
/// Represents a book entity stored in the database.
/// </summary>
public class Book
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [Required, StringLength(20)]
    public string Isbn { get; set; } = "";

    public DateTime? PublicationDate { get; set; }
    public int NumPages { get; set; }
    public decimal Price { get; set; }

    [Required, StringLength(150)]
    public string Author { get; set; } = "";

    [StringLength(20)]
    public string? PublisherCode { get; set; }

    [StringLength(150)]
    public string? Publisher { get; set; }
}
