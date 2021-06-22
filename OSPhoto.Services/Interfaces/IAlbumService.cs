using System.IO.Enumeration;

namespace OSPhoto.Services.Interfaces
{
    public interface IAlbumService
    {
        string GetContentRootPath();
        AlbumResult Get(string path = null);
        Image GetImage(string path);
    }
}