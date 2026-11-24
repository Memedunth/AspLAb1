using System.ComponentModel.DataAnnotations;

namespace AspLAb1.Models;

/// <summary>
/// Form view model for creating a book with field-level and model-level validation.
/// </summary>
public class BookFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tytuł jest wymagany")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tytuł: 2-200 znaków")]
    [Display(Name = "Tytuł")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "ISBN jest wymagany")]
    [Isbn]
    [Display(Name = "ISBN")]
    public string Isbn { get; set; } = "";

    [DataType(DataType.Date)]
    [NotInFuture]
    [Display(Name = "Data wydania")]
    public DateTime? PublicationDate { get; set; }

    [Range(1, 20000, ErrorMessage = "Liczba stron musi być z zakresu 1-20000")]
    [Display(Name = "Liczba stron")]
    public int NumPages { get; set; } = 100;

    [Range(0.01, 10000, ErrorMessage = "Cena musi być dodatnia")]
    [Display(Name = "Cena")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Autor jest wymagany")]
    [Display(Name = "Autor")]
    public string Author { get; set; } = "";

    [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "Kod wydawcy musi mieć format AAA-1234 (np. PWN-2024)")]
    [Display(Name = "Kod wydawcy")]
    public string? PublisherCode { get; set; }

    [StringLength(150)]
    [Display(Name = "Wydawnictwo")]
    public string? Publisher { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PublicationDate.HasValue && PublicationDate.Value.Year < 1900 && Price > 100m)
        {
            yield return new ValidationResult("Książka sprzed 1900 r. nie może kosztować więcej niż 100 zł.");
        }
    }
}
