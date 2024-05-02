using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
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
public class PhotoServiceTests
{
    private string _contentRootPath;
    private IAlbumService _albumService;
    private IPhotoService _service;

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

        var albumLogger = new Logger<AlbumService>(new LoggerFactory());
        _albumService = new AlbumService(Utilities.GetInMemoryDbContext(), albumLogger);

        var logger = new Logger<PhotoService>(new LoggerFactory());
        _service = new PhotoService(Utilities.GetInMemoryDbContext(), logger);
    }

    [Test]
    public void CanGetPhotoInfo()
    {
        var result = _albumService.Get();

        var image = _service.GetInfo(result.Items.OfType<Photo>().First().Id);
        Assert.That(image, Is.InstanceOf<Photo>());
    }
}
