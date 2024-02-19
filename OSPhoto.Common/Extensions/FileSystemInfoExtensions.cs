using SixLabors.ImageSharp;

namespace OSPhoto.Common.Extensions;

public static class FileSystemInfoExtensions
{
    private static readonly string[] NonImageFileExtensions = new[] { ".DS_Store" };

    public static bool IsImageFileType(this FileSystemInfo fsInfo)
    {
        if (string.IsNullOrEmpty(fsInfo.Extension) || NonImageFileExtensions.Contains(fsInfo.Extension)) return false;

        try
        {
            var imageFormat = SixLabors.ImageSharp.Image.DetectFormat(fsInfo.FullName);
            return imageFormat != null;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the ContentType or MimeType for a given file
    /// </summary>
    public static string ContentType(this FileSystemInfo fsInfo)
    {
        string contentType;
        if (!MimeTypes.TryGetMimeType(fsInfo.FullName, out contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }
}
