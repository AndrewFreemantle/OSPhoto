using System;

namespace OSPhoto.Services
{
    public abstract class ItemBase
    {
        public ItemBase(string name, string path)
        {
            Name = name;
            Path = path;
        }

        // public bool IsDirectory { get; set; }
        // public bool IsImage { get; set; }
        public string Name { get; }
        public string Path { get; }
    }
}