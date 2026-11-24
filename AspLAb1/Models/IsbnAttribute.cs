using System.ComponentModel.DataAnnotations;

namespace AspLAb1.Models;

/// <summary>
/// Validates ISBN-10 or ISBN-13 format (digits with optional hyphens/spaces) and optional checksum.
/// </summary>
public class IsbnAttribute : ValidationAttribute
{
    public bool RequireChecksum { get; set; } = false;

    public IsbnAttribute() =>
        ErrorMessage = "Niepoprawny numer ISBN (wymagane 10 lub 13 cyfr; myślniki i spacje dozwolone).";

    public override bool IsValid(object? value)
    {
        if (value is null || (value is string s && string.IsNullOrWhiteSpace(s)))
            return true;

        if (value is not string raw)
            return false;

        var clean = raw.Replace("-", "").Replace(" ", "");

        if (clean.Length == 10)
        {
            for (var i = 0; i < 9; i++)
            {
                if (!char.IsAsciiDigit(clean[i]))
                    return false;
            }

            if (!char.IsAsciiDigit(clean[9]) && clean[9] is not ('X' or 'x'))
                return false;

            if (!RequireChecksum)
                return true;

            var sum10 = 0;
            for (var i = 0; i < 9; i++)
                sum10 += (10 - i) * (clean[i] - '0');

            sum10 += clean[9] is 'X' or 'x' ? 10 : clean[9] - '0';
            return sum10 % 11 == 0;
        }

        if (clean.Length == 13)
        {
            for (var i = 0; i < 13; i++)
            {
                if (!char.IsAsciiDigit(clean[i]))
                    return false;
            }

            if (!RequireChecksum)
                return true;

            var sum13 = 0;
            for (var i = 0; i < 13; i++)
                sum13 += (clean[i] - '0') * (i % 2 == 0 ? 1 : 3);

            return sum13 % 10 == 0;
        }

        return false;
    }
}
