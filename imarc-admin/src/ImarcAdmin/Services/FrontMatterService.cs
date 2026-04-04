using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ImarcAdmin.Models;

namespace ImarcAdmin.Services;

public sealed class FrontMatterService
{
    private static readonly Regex FrontMatterRegex = new("^---\\r?\\n(?<front>[\\s\\S]*?)\\r?\\n---\\r?\\n?", RegexOptions.Compiled);
    private static readonly Regex KeyRegex = new("^(?<key>[A-Za-z0-9_-]+):(?:\\s*(?<value>.*))?$", RegexOptions.Compiled);

    private readonly SlugService _slugService;

    public FrontMatterService(SlugService slugService)
    {
        _slugService = slugService;
    }

    public EditablePost ParseEditablePost(string relativePath, string postId, string rawContent, string baseCommit, string sessionId)
    {
        var (document, body) = Parse(rawContent);
        var title = document.GetScalar("title") ?? Path.GetFileNameWithoutExtension(relativePath);
        var slug = document.GetScalar("slug");
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = _slugService.Slugify(Path.GetFileNameWithoutExtension(relativePath));
        }

        var publishDate = document.GetScalar("date");
        DateTimeOffset? parsedTimestamp = null;
        var dateValue = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var timeValue = "12:00";
        if (DateTimeOffset.TryParse(publishDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed))
        {
            parsedTimestamp = parsed;
            dateValue = parsed.UtcDateTime.ToString("yyyy-MM-dd");
            timeValue = parsed.UtcDateTime.ToString("HH:mm");
        }

        var permalink = document.GetScalar("permalink");
        if (string.IsNullOrWhiteSpace(permalink))
        {
            permalink = BuildPermalink(dateValue, slug);
        }

        return new EditablePost
        {
            PostId = postId,
            OriginalRelativePath = relativePath,
            Title = title,
            PublishDate = dateValue,
            PublishTime = timeValue,
            Slug = slug,
            CategoriesText = string.Join(", ", document.GetArray("categories")),
            TagsText = string.Join(", ", document.GetArray("tags")),
            Excerpt = document.GetScalar("excerpt") ?? string.Empty,
            MarkdownBody = body,
            Permalink = permalink,
            Layout = document.GetScalar("layout") ?? "post.njk",
            BaseCommit = baseCommit,
            SessionId = sessionId,
            IsNew = false,
            SlugLocked = true,
            OriginalTimestamp = parsedTimestamp,
            FrontMatter = document
        };
    }

    public EditablePost CreateNewEditablePost(string baseCommit, string sessionId)
    {
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var slug = "new-post";

        return new EditablePost
        {
            Title = string.Empty,
            PublishDate = date,
            PublishTime = "12:00",
            Slug = slug,
            CategoriesText = string.Empty,
            TagsText = string.Empty,
            Excerpt = string.Empty,
            MarkdownBody = string.Empty,
            Permalink = BuildPermalink(date, slug),
            Layout = "post.njk",
            BaseCommit = baseCommit,
            SessionId = sessionId,
            IsNew = true,
            SlugLocked = false,
            FrontMatter = new FrontMatterDocument()
        };
    }

    public string BuildPermalink(string date, string slug)
    {
        var parsed = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        return $"/{parsed:yyyy}/{parsed:MM}/{parsed:dd}/{slug}/";
    }

    public string Serialize(EditablePost post)
    {
        var document = post.FrontMatter.Clone();
        document.SetScalar("title", post.Title.Trim());
        document.SetScalar("date", ComposeTimestamp(post));
        document.SetScalar("slug", post.Slug.Trim());
        document.SetScalar("layout", "post.njk");
        document.SetScalar("excerpt", post.Excerpt.Trim());
        document.SetArray("categories", _slugService.SplitCommaSeparated(post.CategoriesText));

        var tags = _slugService.SplitCommaSeparated(post.TagsText);
        if (tags.Count > 0)
        {
            document.SetArray("tags", tags);
        }
        else
        {
            document.Remove("tags");
        }

        if (post.IsNew || string.IsNullOrWhiteSpace(document.GetScalar("permalink")))
        {
            document.SetScalar("permalink", BuildPermalink(post.PublishDate, post.Slug));
        }
        else
        {
            document.SetScalar("permalink", post.Permalink);
        }

        if (post.IsNew)
        {
            document.Remove("wordpress_id");
        }

        var builder = new StringBuilder();
        builder.AppendLine("---");
        foreach (var entry in document.Entries)
        {
            if (entry.IsArray)
            {
                if (entry.ArrayValues.Count == 0)
                {
                    builder.AppendLine($"{entry.Key}: []");
                }
                else
                {
                    builder.AppendLine($"{entry.Key}:");
                    foreach (var item in entry.ArrayValues)
                    {
                        builder.AppendLine($"  - {Quote(item)}");
                    }
                }
            }
            else
            {
                builder.AppendLine($"{entry.Key}: {Quote(entry.ScalarValue ?? string.Empty)}");
            }
        }

        builder.AppendLine("---");
        builder.AppendLine();
        builder.Append(post.MarkdownBody.TrimEnd());
        builder.AppendLine();
        return builder.ToString();
    }

    private static string ComposeTimestamp(EditablePost post)
    {
        var date = DateTime.ParseExact(post.PublishDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        var time = TimeOnly.TryParseExact(post.PublishTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime)
            ? parsedTime
            : new TimeOnly(12, 0);

        return new DateTimeOffset(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0, TimeSpan.Zero)
            .ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
    }

    private static (FrontMatterDocument Document, string Body) Parse(string rawContent)
    {
        var match = FrontMatterRegex.Match(rawContent);
        if (!match.Success)
        {
            return (new FrontMatterDocument(), rawContent);
        }

        var document = new FrontMatterDocument();
        var lines = match.Groups["front"].Value.Replace("\r\n", "\n").Split('\n');
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var keyMatch = KeyRegex.Match(line);
            if (!keyMatch.Success)
            {
                continue;
            }

            var key = keyMatch.Groups["key"].Value;
            var rawValue = keyMatch.Groups["value"].Value ?? string.Empty;

            if (string.Equals(rawValue.Trim(), "[]", StringComparison.Ordinal))
            {
                document.AddEntry(new FrontMatterEntry
                {
                    Key = key,
                    IsArray = true,
                    ArrayValues = new List<string>()
                });
                continue;
            }

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                var values = new List<string>();
                var lookahead = index + 1;
                while (lookahead < lines.Length)
                {
                    var next = lines[lookahead].TrimStart();
                    if (!next.StartsWith("- ", StringComparison.Ordinal))
                    {
                        break;
                    }

                    values.Add(Unquote(next[2..].Trim()));
                    lookahead++;
                }

                if (values.Count > 0)
                {
                    document.AddEntry(new FrontMatterEntry
                    {
                        Key = key,
                        IsArray = true,
                        ArrayValues = values
                    });
                    index = lookahead - 1;
                    continue;
                }

                document.AddEntry(new FrontMatterEntry
                {
                    Key = key,
                    ScalarValue = string.Empty
                });
                continue;
            }

            document.AddEntry(new FrontMatterEntry
            {
                Key = key,
                ScalarValue = Unquote(rawValue.Trim())
            });
        }

        var body = rawContent[match.Length..].TrimStart('\r', '\n');
        return (document, body);
    }

    private static string Quote(string value)
        => $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";

    private static string Unquote(string value)
    {
        if (value.Length >= 2 &&
            ((value.StartsWith('"') && value.EndsWith('"')) || (value.StartsWith('\'') && value.EndsWith('\''))))
        {
            value = value[1..^1];
        }

        return value.Replace("\\\"", "\"", StringComparison.Ordinal).Replace("\\\\", "\\", StringComparison.Ordinal);
    }
}
