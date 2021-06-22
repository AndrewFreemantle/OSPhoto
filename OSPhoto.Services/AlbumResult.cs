using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OSPhoto.Services
{
    public class AlbumResult
    {
        public AlbumResult(string path, string contentRootPath, IEnumerable<ItemBase> contents)
        {
            Location = SplitPathIntoBreadcrumbs(path, contentRootPath);
            Contents = contents;
        }

        private IEnumerable<Location> SplitPathIntoBreadcrumbs(string path, string contentRootPath)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) return Enumerable.Empty<Location>();

            var fullPath = new DirectoryInfo(Path.Combine(contentRootPath, path));
            var location = new List<Location>();

            var currentPath = fullPath;
            while (currentPath != null)
            {
                location.Add(new Location(currentPath, contentRootPath));
                currentPath = currentPath.Parent;

                // Stop walking when we reach the content root
                if (currentPath != null && currentPath.FullName == contentRootPath) currentPath = null;
            }

            location.Reverse();
            return location;
        }

        public IEnumerable<Location> Location { get; }
        public IEnumerable<ItemBase> Contents { get;}
    }
}