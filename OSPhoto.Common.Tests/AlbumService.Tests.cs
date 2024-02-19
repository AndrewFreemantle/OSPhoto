using System.Net;
using OSPhoto.Common.Exceptions;
using OSPhoto.Common.Interfaces;

namespace OSPhoto.Common.Tests;

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

        _service = new AlbumService(Path.Join(_contentRootPath, "Media"));
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

        var directory = rootResult.Items.OfType<Directory>().First();
        var directoryResult = _service.Get(directory.Id);

        Assert.AreNotEqual(directoryResult.Items.Count(), rootResult.Items.Count());
    }

    [Test]
    public void DirectoriesAndImagesAreCorrectlyIdentified()
    {
        var result = _service.Get();

        Assert.IsFalse(result.Items.OfType<Image>().Any(item => item.Name.Contains(".txt")));
        Assert.IsFalse(result.Items.OfType<Directory>().Any(item => item.Name.Contains(".txt")));
    }

    [Test]
    public void DirectoriesAndImagesAreSorted()
    {
        var result = _service.Get();

        Assert.IsTrue(result.Items.First().GetType() == typeof(Directory));
        Assert.IsTrue(result.Items.Skip(1).First().GetType() == typeof(Image));
        Assert.IsTrue(result.Items.Last().GetType() == typeof(File));
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

        var image = _service.GetImage(result.Items.OfType<Image>().First().Path);
        Assert.IsInstanceOf<Image>(image);
    }
}
