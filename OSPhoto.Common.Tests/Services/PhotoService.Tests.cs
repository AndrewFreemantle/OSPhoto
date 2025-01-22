using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
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
    private IFileSystem _fileSystem;
    private ICommentService _commentService;
    private ILogger<PhotoService> _logger;
    private IPhotoService _service;

    [SetUp]
    public void Setup()
    {
        // In development / unit testing, the call to
        // `System.IO.Directory.GetCurrentDirectory()` returns something like
        //     /home/{user}/projects/OSPhoto/OSPhoto.Common.Tests/bin/Debug/net8.0
        //  and we need this dir  ------^
        // TODO: replace this with the System.IO.Abstractions equivalent (if there is one)
        var currentDirectory = new FileSystem().DirectoryInfo.New(Directory.GetCurrentDirectory());
        _contentRootPath = currentDirectory.Parent?.Parent?.Parent?.Parent?.FullName ?? string.Empty;

        Environment.SetEnvironmentVariable("MEDIA_PATH", Path.Join(_contentRootPath, "Media"));

        _fileSystem = new FileSystem();

        var albumLogger = new Logger<AlbumService>(new LoggerFactory());
        _albumService = new AlbumService(Utilities.GetInMemoryDbContext(), _fileSystem, albumLogger);

        var commentLogger = new Logger<CommentService>(new LoggerFactory());
        _commentService = new CommentService(Utilities.GetInMemoryDbContext(), commentLogger);

        _logger = new Logger<PhotoService>(new LoggerFactory());
        _service = new PhotoService(Utilities.GetInMemoryDbContext(), _commentService, _fileSystem, _logger);
    }

    [Test]
    public void CanGetPhotoInfo()
    {
        var result = _albumService.Get();

        var image = _service.GetInfo(result.Items.OfType<Photo>().First().Id);
        Assert.That(image, Is.InstanceOf<Photo>());
    }

    [Test]
    public async Task ChecksDestinationFileExists_DoesNotExist()
    {
        // Arrange
        var filename = "/Media/Album 1/does-not-exist.jpeg";

        var mockFileSystem = new Mock<IFileSystem>();
        var mockFileInfo = new Mock<IFileInfo>();

        mockFileInfo.Setup(fi => fi.Exists).Returns(false);
        mockFileInfo.Setup(fi => fi.FullName).Returns(filename);
        mockFileInfo.Setup(fi => fi.Name).Returns("does-not-exist.jpeg");
        mockFileInfo.Setup(fi => fi.Extension).Returns(".jpeg");
        mockFileInfo.Setup(fi => fi.DirectoryName).Returns("/Media/Album 1");

        mockFileSystem.Setup(fs => fs.FileInfo.New(It.IsAny<string>())).Returns(mockFileInfo.Object);

        _fileSystem = mockFileSystem.Object;
        _service = new PhotoService(Utilities.GetInMemoryDbContext(), _commentService, _fileSystem, _logger);

        // Act
        var result = await _service.CheckIfDestinationExists(filename);

        // Assert
        Assert.That(result, Is.EqualTo(filename));
    }

    [Test]
    public async Task ChecksDestinationFileExists_Exists()
    {
        // Arrange
        var filename       = "/Media/Album 1/photo-exists.jpeg";
        var expectedResult = "/Media/Album 1/photo-exists_1.jpeg";

        var mockFileSystem = new Mock<IFileSystem>();
        var mockFileInfo = new Mock<IFileInfo>();

        mockFileInfo.Setup(fi => fi.Exists).Returns(true);
        mockFileInfo.Setup(fi => fi.FullName).Returns(filename);
        mockFileInfo.Setup(fi => fi.Name).Returns("photo-exists.jpeg");
        mockFileInfo.Setup(fi => fi.Extension).Returns(".jpeg");
        mockFileInfo.Setup(fi => fi.DirectoryName).Returns("/Media/Album 1");

        var secondMockFileInfo = new Mock<IFileInfo>();
        secondMockFileInfo.Setup(fi => fi.Exists).Returns(false);

        mockFileSystem.Setup(fs => fs.FileInfo.New(It.Is<string>(s => s == filename))).Returns(mockFileInfo.Object);
        mockFileSystem.Setup(fs => fs.FileInfo.New(It.Is<string>(s => s == expectedResult))).Returns(secondMockFileInfo.Object);

        _fileSystem = mockFileSystem.Object;
        _service = new PhotoService(Utilities.GetInMemoryDbContext(), _commentService, _fileSystem, _logger);

        // Act
        var result = await _service.CheckIfDestinationExists(filename);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task ChecksDestinationFileExists_MultipleExists()
    {
        var filename       = "/Media/Album 1/photo-exists_4.jpeg";
        var expectedResult = "/Media/Album 1/photo-exists_5.jpeg";

        var mockFileSystem = new Mock<IFileSystem>();
        var mockFileInfo = new Mock<IFileInfo>();

        mockFileInfo.Setup(fi => fi.Exists).Returns(true);
        mockFileInfo.Setup(fi => fi.FullName).Returns(filename);
        mockFileInfo.Setup(fi => fi.Name).Returns("photo-exists_4.jpeg");
        mockFileInfo.Setup(fi => fi.Extension).Returns(".jpeg");
        mockFileInfo.Setup(fi => fi.DirectoryName).Returns("/Media/Album 1");

        var secondMockFileInfo = new Mock<IFileInfo>();
        secondMockFileInfo.Setup(fi => fi.Exists).Returns(false);

        mockFileSystem.Setup(fs => fs.FileInfo.New(It.Is<string>(s => s == filename))).Returns(mockFileInfo.Object);
        mockFileSystem.Setup(fs => fs.FileInfo.New(It.Is<string>(s => s == expectedResult))).Returns(secondMockFileInfo.Object);

        _fileSystem = mockFileSystem.Object;
        _service = new PhotoService(Utilities.GetInMemoryDbContext(), _commentService, _fileSystem, _logger);

        // Act
        var result = await _service.CheckIfDestinationExists(filename);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

}
