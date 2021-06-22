using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace OSPhoto.Services.Extensions
{
    public static class FileSystemInfoExtensions
    {
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new FileExtensionContentTypeProvider();
        
        public static bool IsImageFileType(this FileSystemInfo fsInfo)
        {
            if (string.IsNullOrEmpty(fsInfo.Extension)) return false;

            var imageInfo = SixLabors.ImageSharp.Image.Identify(fsInfo.FullName);
            return imageInfo != null;
        }

        /// <summary>
        /// Returns the ContentType or MimeType for a given file
        /// </summary>
        public static string ContentType(this FileSystemInfo fsInfo)
        {
            // source: https://dotnetcoretutorials.com/2018/08/14/getting-a-mime-type-from-a-file-name-in-net-core/
            string contentType;
            if (!ContentTypeProvider.TryGetContentType(fsInfo.FullName, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}