using System.Net;

namespace OSPhoto.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Encodes a string in a hexadecimal format
    /// </summary>
    /// <returns>a new encoded string</returns>
    public static string ToHex(this string str)
    {
        return string.Concat(WebUtility.UrlEncode(str).Select(c => ((int)c).ToString("X2")));
    }

    /// <summary>
    /// Decodes a hexadecimal encoded string
    /// </summary>
    /// <returns>a new decoded string</returns>
    public static string FromHex(this string hexadecimalEncodedString)
    {
        if (string.IsNullOrEmpty(hexadecimalEncodedString))
        {
            throw new ArgumentException("The string cannot be null or empty.", nameof(hexadecimalEncodedString));
        }

        if (hexadecimalEncodedString.Length % 2 != 0)
        {
            throw new ArgumentException("The hexadecimal string must have an even length.", nameof(hexadecimalEncodedString));
        }

        var bytes = Enumerable.Range(0, hexadecimalEncodedString.Length / 2)
            .Select(i => Convert.ToByte(hexadecimalEncodedString.Substring(i * 2, 2), 16))
            .ToArray();

        return WebUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(bytes));
    }
}
