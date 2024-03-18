using System.Text;

namespace OSPhoto.Api.Extensions;

/// <summary>
/// Stream Utility Extensions
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Returns a stream as a UTF-8 encoded string, using a StreamReader
    /// </summary>
    public static async Task<string> ReadAsStringAsync(this Stream stream)
    {
        string result = string.Empty;
        if (stream.CanRead && stream.Length > 0)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = await reader.ReadToEndAsync();
            }
        }

        return result;
    }
}
