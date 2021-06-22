using System;
using System.IO;
using System.Linq;
using OSPhoto.Services.Exceptions;
using OSPhoto.Services.Extensions;
using OSPhoto.Services.Interfaces;

namespace OSPhoto.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly string _contentRootPath;
        private const string MediaDirectoryName = "Media";

        public AlbumService(string contentRootPath, string mediaDirectoryName = MediaDirectoryName)
        {
            _contentRootPath = string.IsNullOrEmpty(mediaDirectoryName) || string.IsNullOrWhiteSpace(mediaDirectoryName)
                ? _contentRootPath
                : Path.Combine(contentRootPath, mediaDirectoryName);
        }

        public string GetContentRootPath()
        {
            return _contentRootPath;
        }

        public AlbumResult Get(string path = null)
        {
            try
            {
                return new AlbumResult(path,
                    _contentRootPath,
                    GetContentDirectory(path)
                    .EnumerateFileSystemInfos()
                    .Select(ConvertToItemBase)
                    .OrderByDescending(item => item.GetType() == typeof(Directory))
                    .ThenBy(item => item.Name));
            }
            catch (Exception ex)
            {
                // TODO: Log this out to stderr (with an ILogger?)
                Console.WriteLine(ex);

                throw new AlbumServiceException(path, ex.Message, ex);
            }
        }

        public Image GetImage(string path)
        {
            try
            {
                var fsInfo = new FileInfo(Path.Combine(_contentRootPath, path));
                var image = ConvertToItemBase(fsInfo);

                return new Image(image.Name, image.Path, fsInfo.ContentType());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private DirectoryInfo GetContentDirectory(string path = null)
        {
            return new DirectoryInfo(
                string.IsNullOrEmpty(path)
                || string.IsNullOrWhiteSpace(path)
                    ? _contentRootPath
                    : Path.Combine(_contentRootPath, path));
        }

        private ItemBase ConvertToItemBase(FileSystemInfo fsInfo)
        {
            var itemPath = fsInfo.FullName.Substring(_contentRootPath.Length + 1);

            if ((fsInfo.Attributes & FileAttributes.Directory) != 0)
                return new Directory(fsInfo.Name, itemPath);

            if (fsInfo.IsImageFileType())
                return new Image(fsInfo.Name, itemPath);

            return new File(fsInfo.Name, itemPath);
        }
    }
}