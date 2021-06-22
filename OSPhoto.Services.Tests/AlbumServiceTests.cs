using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using OSPhoto.Services.Exceptions;
using OSPhoto.Services.Interfaces;

namespace OSPhoto.Services.Tests
{
    /// <summary>
    /// AlbumService Tests
    /// <remarks>these tests assume the contents and structure of the included Sample Album</remarks>  
    /// </summary>
    public class AlbumServiceTests
    {
        private string _contentRootPath;
        private IAlbumService _service;
        
        [SetUp]
        public void Setup()
        {
            // In development / unit testing, the call to
            // `System.IO.Directory.GetCurrentDirectory()` returns something like
            //     /home/{user}/projects/OSPhoto/OSPhoto.Services.Tests/bin/Debug/net5.0
            //  and we need this dir  ------^
            var currentDirectory = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
            _contentRootPath = currentDirectory.Parent?.Parent?.Parent?.Parent?.FullName ?? string.Empty;

            _service = new AlbumService(_contentRootPath);
        }

        [Test]
        public void CanGetTheContentsOfTheRootMediaDirectory()
        {
            var result = _service.Get();

            Assert.IsNotEmpty(result.Contents);
        }

        [Test]
        public void CanGetTheContentsOfAGivenDirectory()
        {
            var rootResult = _service.Get();

            var directory = rootResult.Contents.OfType<Directory>().First();
            var directoryResult = _service.Get(WebUtility.UrlDecode(directory.Path));

            Assert.AreNotEqual(directoryResult.Contents.Count(), rootResult.Contents.Count());
        }

        [Test]
        public void DirectoriesAndImagesAreCorrectlyIdentified()
        {
            var result = _service.Get();
            
            Assert.IsFalse(result.Contents.OfType<Image>().Any(item => item.Name.Contains(".txt")));
            Assert.IsFalse(result.Contents.OfType<Directory>().Any(item => item.Name.Contains(".txt")));
        }
        
        [Test]
        public void DirectoriesAndImagesAreSorted()
        {
            var result = _service.Get();
            
            Assert.IsTrue(result.Contents.First().GetType() == typeof(Directory));
            Assert.IsTrue(result.Contents.Skip(1).First().GetType() == typeof(Image));
            Assert.IsTrue(result.Contents.Last().GetType() == typeof(File));
        }

        [Test]
        public void CannotRequestADirectoryOutsideOfRootMedia()
        {
            Assert.Throws<AlbumServiceException>(() => _service.Get("//"));
            Assert.Throws<AlbumServiceException>(() => _service.Get("\\"));
        }

        [Test]
        public void CanGetImage()
        {
            var result = _service.Get();

            var image = _service.GetImage(result.Contents.OfType<Image>().First().Path);
            Assert.IsInstanceOf<Image>(image);
        }

        [Test]
        public void LocationShouldBeEmptyForTheRootMediaDirectory()
        {
            var result = _service.Get();
            
            Assert.IsEmpty(result.Location);
        }
        
        [Test]
        public void LocationShouldBeTurnedIntoBreadcrumbsForSubDirectories()
        {
            var result = _service.Get();
            var directoryPath = result.Contents.OfType<Directory>().First().Path;
            result = _service.Get(directoryPath);
            var depth = 1;

            do
            {
                Assert.AreEqual(depth, result.Location.Count());

                directoryPath = result.Contents.OfType<Directory>().First().Path;
                result = _service.Get(directoryPath);
                depth += 1;
            } while (result.Contents.OfType<Directory>().Any());
        }
    }
}