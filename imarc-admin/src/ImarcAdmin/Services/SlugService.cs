using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ImarcAdmin.Services;

public sealed class SlugService
{
    private static readonly Regex NonAlphaNumeric = new("[^a-z0-9]+", RegexOptions.Compiled);
    private static readonly Regex MultiDash = new("-{2,}", RegexOptions.Compiled);

    public string Slugify(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "untitled-post";
        }

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var ch in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        var cleaned = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        cleaned = cleaned.Replace('&', ' ').Replace('\'', ' ');
        cleaned = NonAlphaNumeric.Replace(cleaned, "-");
        cleaned = MultiDash.Replace(cleaned, "-").Trim('-');
        return string.IsNullOrWhiteSpace(cleaned) ? "untitled-post" : cleaned;
    }

    public IReadOnlyList<string> SplitCommaSeparated(string? text)
        => string.IsNullOrWhiteSpace(text)
            ? Array.Empty<string>()
            : text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
}

