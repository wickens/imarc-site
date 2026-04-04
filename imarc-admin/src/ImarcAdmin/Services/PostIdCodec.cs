using System.Text;

namespace ImarcAdmin.Services;

public static class PostIdCodec
{
    public static string Encode(string relativePath)
    {
        var bytes = Encoding.UTF8.GetBytes(relativePath);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static string Decode(string encoded)
    {
        var normalised = encoded.Replace('-', '+').Replace('_', '/');
        var padding = 4 - (normalised.Length % 4);
        if (padding is > 0 and < 4)
        {
            normalised = normalised.PadRight(normalised.Length + padding, '=');
        }

        return Encoding.UTF8.GetString(Convert.FromBase64String(normalised));
    }
}

