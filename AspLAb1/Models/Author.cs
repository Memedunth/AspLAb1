using System.ComponentModel.DataAnnotations;

namespace AspLAb1.Models;

/// <summary>
/// Represents an author managed in the Admin area.
/// </summary>
public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię i nazwisko autora jest wymagane")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Imię i nazwisko: 2-150 znaków")]
    [Display(Name = "Imię i nazwisko")]
    public string Name { get; set; } = "";

    [Display(Name = "Kraj")]
    public string? Country { get; set; }
}
