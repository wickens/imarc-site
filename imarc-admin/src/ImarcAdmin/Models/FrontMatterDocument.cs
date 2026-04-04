using System.Collections.ObjectModel;

namespace ImarcAdmin.Models;

public sealed class FrontMatterDocument
{
    private readonly List<FrontMatterEntry> _entries = new();

    public ReadOnlyCollection<FrontMatterEntry> Entries => _entries.AsReadOnly();

    public string? GetScalar(string key)
        => _entries.FirstOrDefault(entry => string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase) && !entry.IsArray)?.ScalarValue;

    public IReadOnlyList<string> GetArray(string key)
    {
        var values = _entries.FirstOrDefault(entry => string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase) && entry.IsArray)?.ArrayValues;
        return values is null ? Array.Empty<string>() : values;
    }

    public void AddEntry(FrontMatterEntry entry)
        => _entries.Add(entry);

    public void SetScalar(string key, string value)
    {
        var entry = GetOrCreate(key);
        entry.IsArray = false;
        entry.ScalarValue = value;
        entry.ArrayValues = new List<string>();
    }

    public void SetArray(string key, IEnumerable<string> values)
    {
        var entry = GetOrCreate(key);
        entry.IsArray = true;
        entry.ScalarValue = null;
        entry.ArrayValues = values.ToList();
    }

    public void Remove(string key)
        => _entries.RemoveAll(entry => string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase));

    public FrontMatterDocument Clone()
    {
        var clone = new FrontMatterDocument();
        foreach (var entry in _entries)
        {
            clone.AddEntry(new FrontMatterEntry
            {
                Key = entry.Key,
                ScalarValue = entry.ScalarValue,
                IsArray = entry.IsArray,
                ArrayValues = new List<string>(entry.ArrayValues)
            });
        }

        return clone;
    }

    private FrontMatterEntry GetOrCreate(string key)
    {
        var existing = _entries.FirstOrDefault(entry => string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            return existing;
        }

        var entry = new FrontMatterEntry { Key = key };
        _entries.Add(entry);
        return entry;
    }
}
