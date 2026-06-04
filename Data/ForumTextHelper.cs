using System.Text.RegularExpressions;

namespace E_project_DVD_Shop.Data;

internal static partial class ForumTextHelper
{
    // UTF-8 dash bytes misread as Windows-1252 (shows as â€" in the browser)
    [GeneratedRegex(@"\u00E2\u20AC[\u0093\u0094\u2013\u2014\u201C\u201D]?", RegexOptions.CultureInvariant)]
    private static partial Regex MojibakeDashRegex();

    /// <summary>Normalizes fancy dashes and common UTF-8 mojibake to plain ASCII hyphens.</summary>
    public static string ToAsciiDashes(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var result = text
            .Replace("\u2014", " - ", StringComparison.Ordinal)
            .Replace("\u2013", "-", StringComparison.Ordinal);

        result = MojibakeDashRegex().Replace(result, m =>
            m.Value.Length >= 3 && (m.Value[2] == '\u2013' || m.Value[2] == '\u0093' || m.Value[2] == '\u201C')
                ? "-"
                : " - ");

        result = result
            .Replace("â€\"", " - ", StringComparison.Ordinal)
            .Replace("â€“", "-", StringComparison.Ordinal)
            .Replace("â€”", " - ", StringComparison.Ordinal);

        while (result.Contains("  -  ", StringComparison.Ordinal))
            result = result.Replace("  -  ", " - ", StringComparison.Ordinal);

        return result;
    }

    public static bool HasProblematicDashes(string? text) =>
        !string.IsNullOrEmpty(text) &&
        (text.Contains('\u2014') || text.Contains('\u2013') ||
         text.Contains('\u00E2') || text.Contains("â€", StringComparison.Ordinal));
}
