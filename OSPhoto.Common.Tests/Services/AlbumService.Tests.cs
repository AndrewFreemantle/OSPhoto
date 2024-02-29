using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework.Internal;
using OSPhoto.Common.Exceptions;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using OSPhoto.Common.Services;

namespace OSPhoto.Common.Tests.Services;

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
        //     /home/{user}/projects/OSPhoto/OSPhoto.Common.Tests/bin/Debug/net8.0
        //  and we need this dir  ------^
        var currentDirectory = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        _contentRootPath = currentDirectory.Parent?.Parent?.Parent?.Parent?.FullName ?? string.Empty;

        Environment.SetEnvironmentVariable("MEDIA_PATH", Path.Join(_contentRootPath, "Media"));
        var logger = new Logger<AlbumService>(new LoggerFactory());
        _service = new AlbumService(logger);
    }

    [Test]
    public void CanGetTheContentsOfTheRootMediaDirectory()
    {
        var result = _service.Get();

        Assert.IsNotEmpty(result.Items);
    }

    [Test]
    public void CanGetTheContentsOfAGivenDirectory()
    {
        var rootResult = _service.Get();

        var directory = rootResult.Items.OfType<Album>().First();
        var directoryResult = _service.Get(directory.Id);

        Assert.AreNotEqual(directoryResult.Items.Count(), rootResult.Items.Count());
    }

    [Test]
    public void DirectoriesAndImagesAreCorrectlyIdentified()
    {
        var result = _service.Get();

        Assert.IsFalse(result.Items.OfType<Photo>().Any(item => item.Name.Contains(".txt")));
        Assert.IsFalse(result.Items.OfType<Album>().Any(item => item.Name.Contains(".txt")));
    }

    [Test]
    public void DirectoriesAndImagesAreSorted()
    {
        var result = _service.Get();

        Assert.IsTrue(result.Items.First().GetType() == typeof(Album));
        Assert.IsTrue(result.Items.Last().GetType() == typeof(Photo));
    }

    [Test]
    public void CannotRequestADirectoryOutsideOfRootMedia()
    {
        Assert.Throws<AlbumServiceException>(() => _service.Get("//"));
        Assert.Throws<AlbumServiceException>(() => _service.Get("\\"));
    }

    [Test]
    public void CanGetPhoto()
    {
        var result = _service.Get();

        var image = _service.GetPhoto(result.Items.OfType<Photo>().First().Id);
        Assert.IsInstanceOf<Photo>(image);
    }
}
