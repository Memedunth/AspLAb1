using System.ComponentModel.DataAnnotations;

namespace AspLAb1.Models;

/// <summary>
/// Validates that a <see cref="DateTime"/> value is not in the future.
/// </summary>
public class NotInFutureAttribute : ValidationAttribute
{
    public NotInFutureAttribute() => ErrorMessage = "Data nie może być z przyszłości";

    public override bool IsValid(object? value) => value is not DateTime d || d.Date <= DateTime.Today;
}
