using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ImarcAdmin.Services;

public sealed class SimpleMarkdownRenderer
{
    private static readonly Regex ImageRegex = new("!\\[(?<alt>.*?)\\]\\((?<url>[^\\s)]+)(?:\\s+\"(?<title>[^\"]*)\")?\\)", RegexOptions.Compiled);
    private static readonly Regex LinkRegex = new("\\[(?<text>[^\\]]+)\\]\\((?<url>[^\\s)]+)(?:\\s+\"(?<title>[^\"]*)\")?\\)", RegexOptions.Compiled);
    private static readonly Regex CodeRegex = new("`(?<code>[^`]+)`", RegexOptions.Compiled);
    private static readonly Regex BoldRegex = new("(\\*\\*|__)(?<text>.+?)\\1", RegexOptions.Compiled);
    private static readonly Regex ItalicRegex = new("(?<!\\*)\\*(?<text>[^*]+)\\*(?!\\*)|_(?<u>[^_]+)_", RegexOptions.Compiled);

    public string Render(string markdown)
    {
        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        var builder = new StringBuilder();
        var inCodeFence = false;
        var inUnorderedList = false;
        var inOrderedList = false;
        var inBlockquote = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();
            if (line.StartsWith("```", StringComparison.Ordinal))
            {
                CloseTextBlocks(builder, ref inUnorderedList, ref inOrderedList, ref inBlockquote);
                builder.AppendLine(inCodeFence ? "</code></pre>" : "<pre><code>");
                inCodeFence = !inCodeFence;
                continue;
            }

            if (inCodeFence)
            {
                builder.AppendLine(WebUtility.HtmlEncode(rawLine));
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                CloseTextBlocks(builder, ref inUnorderedList, ref inOrderedList, ref inBlockquote);
                continue;
            }

            if (TryRenderHeading(builder, line))
            {
                CloseTextBlocks(builder, ref inUnorderedList, ref inOrderedList, ref inBlockquote);
                continue;
            }

            if (line.StartsWith("> ", StringComparison.Ordinal))
            {
                CloseLists(builder, ref inUnorderedList, ref inOrderedList);
                if (!inBlockquote)
                {
                    builder.AppendLine("<blockquote>");
                    inBlockquote = true;
                }

                builder.AppendLine($"<p>{RenderInline(line[2..])}</p>");
                continue;
            }

            if (TryRenderList(builder, line, ref inUnorderedList, ref inOrderedList))
            {
                if (inBlockquote)
                {
                    builder.AppendLine("</blockquote>");
                    inBlockquote = false;
                }

                continue;
            }

            CloseTextBlocks(builder, ref inUnorderedList, ref inOrderedList, ref inBlockquote);
            builder.AppendLine($"<p>{RenderInline(line)}</p>");
        }

        if (inCodeFence)
        {
            builder.AppendLine("</code></pre>");
        }

        CloseTextBlocks(builder, ref inUnorderedList, ref inOrderedList, ref inBlockquote);
        return builder.ToString();
    }

    private static bool TryRenderHeading(StringBuilder builder, string line)
    {
        var trimmed = line.TrimStart();
        var level = 0;
        while (level < trimmed.Length && trimmed[level] == '#')
        {
            level++;
        }

        if (level is < 1 or > 6 || trimmed.Length <= level || trimmed[level] != ' ')
        {
            return false;
        }

        builder.AppendLine($"<h{level}>{RenderInline(trimmed[(level + 1)..])}</h{level}>");
        return true;
    }

    private static bool TryRenderList(StringBuilder builder, string line, ref bool inUnorderedList, ref bool inOrderedList)
    {
        var trimmed = line.TrimStart();
        if (trimmed.StartsWith("- ", StringComparison.Ordinal) || trimmed.StartsWith("* ", StringComparison.Ordinal))
        {
            if (inOrderedList)
            {
                builder.AppendLine("</ol>");
                inOrderedList = false;
            }

            if (!inUnorderedList)
            {
                builder.AppendLine("<ul>");
                inUnorderedList = true;
            }

            builder.AppendLine($"<li>{RenderInline(trimmed[2..])}</li>");
            return true;
        }

        var dotIndex = trimmed.IndexOf(". ", StringComparison.Ordinal);
        if (dotIndex > 0 && int.TryParse(trimmed[..dotIndex], out _))
        {
            if (inUnorderedList)
            {
                builder.AppendLine("</ul>");
                inUnorderedList = false;
            }

            if (!inOrderedList)
            {
                builder.AppendLine("<ol>");
                inOrderedList = true;
            }

            builder.AppendLine($"<li>{RenderInline(trimmed[(dotIndex + 2)..])}</li>");
            return true;
        }

        return false;
    }

    private static void CloseTextBlocks(StringBuilder builder, ref bool inUnorderedList, ref bool inOrderedList, ref bool inBlockquote)
    {
        CloseLists(builder, ref inUnorderedList, ref inOrderedList);
        if (inBlockquote)
        {
            builder.AppendLine("</blockquote>");
            inBlockquote = false;
        }
    }

    private static void CloseLists(StringBuilder builder, ref bool inUnorderedList, ref bool inOrderedList)
    {
        if (inUnorderedList)
        {
            builder.AppendLine("</ul>");
            inUnorderedList = false;
        }

        if (inOrderedList)
        {
            builder.AppendLine("</ol>");
            inOrderedList = false;
        }
    }

    private static string RenderInline(string input)
    {
        var encoded = WebUtility.HtmlEncode(input);
        encoded = ImageRegex.Replace(encoded, match =>
        {
            var alt = match.Groups["alt"].Value;
            var url = match.Groups["url"].Value;
            var title = match.Groups["title"].Value;
            var titleAttribute = string.IsNullOrWhiteSpace(title) ? string.Empty : $" title=\"{title}\"";
            return $"<img src=\"{url}\" alt=\"{alt}\"{titleAttribute} />";
        });

        encoded = LinkRegex.Replace(encoded, match =>
        {
            var text = match.Groups["text"].Value;
            var url = match.Groups["url"].Value;
            var title = match.Groups["title"].Value;
            var titleAttribute = string.IsNullOrWhiteSpace(title) ? string.Empty : $" title=\"{title}\"";
            return $"<a href=\"{url}\" target=\"_blank\" rel=\"noreferrer\"{titleAttribute}>{text}</a>";
        });

        encoded = CodeRegex.Replace(encoded, "<code>${code}</code>");
        encoded = BoldRegex.Replace(encoded, "<strong>${text}</strong>");
        encoded = ItalicRegex.Replace(encoded, match =>
        {
            var text = match.Groups["text"].Success ? match.Groups["text"].Value : match.Groups["u"].Value;
            return $"<em>{text}</em>";
        });

        return encoded;
    }
}

