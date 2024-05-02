using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
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
        _service = new AlbumService(Utilities.GetInMemoryDbContext(), logger);
    }

    [Test]
    public void CanGetTheContentsOfTheRootMediaDirectory()
    {
        var result = _service.Get();

        Assert.That(result.Items, Is.Not.Empty);
    }

    [Test]
    public void CanGetTheContentsOfAGivenDirectory()
    {
        var rootResult = _service.Get();

        var directory = rootResult.Items.OfType<Album>().First();
        var directoryResult = _service.Get(directory.Id);

        Assert.That(directoryResult.Items.Count(), Is.Not.EqualTo(rootResult.Items.Count()));
    }

    [Test]
    public void DirectoriesAndImagesAreCorrectlyIdentified()
    {
        var result = _service.Get();

        Assert.That(result.Items.OfType<Photo>().Any(item => item.Name.Contains(".txt")), Is.False);
        Assert.That(result.Items.OfType<Album>().Any(item => item.Name.Contains(".txt")), Is.False);
    }

    [Test]
    public void DirectoriesAndImagesAreSorted()
    {
        var result = _service.Get();

        Assert.That(result.Items.First().GetType() == typeof(Album));
        Assert.That(result.Items.Last().GetType() == typeof(Photo));
    }

    [Test]
    public void CannotRequestADirectoryOutsideOfRootMedia()
    {
        Assert.Throws<AlbumServiceException>(() => _service.Get("//"));
        Assert.Throws<AlbumServiceException>(() => _service.Get("../"));
        Assert.Throws<AlbumServiceException>(() => _service.Get("\\"));
    }
}
