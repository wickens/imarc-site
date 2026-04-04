namespace ImarcAdmin.Models;

public sealed class FrontMatterEntry
{
    public required string Key { get; init; }
    public string? ScalarValue { get; set; }
    public bool IsArray { get; set; }
    public List<string> ArrayValues { get; set; } = new();
}

