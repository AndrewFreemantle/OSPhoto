using System;

namespace OSPhoto.Services
{
    public class Image : ItemBase
    {
        public Image(string name, string path) : base(name, path)
        {
        }

        public Image(string name, string path, string contentType) : base(name, path)
        {
            ContentType = contentType;
        }

        public string ContentType { get; }
    }
}