using SixLabors.ImageSharp;

namespace OSPhoto.Common.Extensions;

public static class FileSystemInfoExtensions
{
    private static readonly string[] NonImageFileExtensions = { ".DS_Store" };

    // source: https://www.synology.com/en-global/dsm/6.2/software_spec/photo_station
    private static readonly string[] VideoFileExtensions = ["3G2", "3GP", "ASF", "AVI", "DAT", "DIVX", "FLV", "M4V", "MOV", "MP4", "MPEG", "MPG", "MTS", "M2TS", "M2T", "QT", "WMV", "XVID", "F4V"];

    public static bool IsImageFileType(this FileSystemInfo fsInfo)
    {
        if (string.IsNullOrEmpty(fsInfo.Extension) || NonImageFileExtensions.Contains(fsInfo.Extension)) return false;

        try
        {
            var imageFormat = Image.DetectFormat(fsInfo.FullName);
            return imageFormat != null;
        }
        catch (Exception)
        {
            return false;
        }
    }


    public static bool IsVideoFileType(this FileSystemInfo fsInfo)
    {
        return VideoFileExtensions.Contains(fsInfo.Extension.ToUpperInvariant().TrimStart('.'));
    }

    public static DateTimeOffset LastModifiedOffsetUtc(this FileSystemInfo fsInfo)
    {
        return new DateTimeOffset(fsInfo.LastWriteTimeUtc);
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
