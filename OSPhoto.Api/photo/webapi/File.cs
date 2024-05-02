using OSPhoto.Common.Interfaces;

namespace OSPhoto.Api.photo.webapi;

public class FileRequest : RequestBase
{
    [BindFrom("mtime")]
    public string? ModifiedTime { get; set; }
    [BindFrom("dest_folder_path")]
    public string? DestinationAlbum { get; set; }
    public string? MimeType { get; set; }
    public string? Title { get; set; }
    public string? FileName { get; set; }
    public string? Description { get; set; }
    public string? Duplicate { get; set; }  // "replace"
    public string? Name { get; set; }
}

public class FileResponse
{
    public bool Success => true;
}

public class FileUpload(IPhotoService service) : Endpoint<FileRequest, FileResponse>
{
    public override void Configure()
    {
        Post("file.php");
        AllowFormData(urlEncoded: true);
        AllowFileUploads();
    }

    public override async Task HandleAsync(FileRequest req, CancellationToken ct)
    {
        Logger.LogInformation("File (method: {method})", req.Method);

        switch (req.Method)
        {
            case RequestMethod.UploadPhoto:
                if (Files.Count > 0)
                {
                    await service.Upload(Files[0], req.DestinationAlbum, req.FileName, req.Title, req.Description);
                    await SendAsync(new FileResponse());
                    break;
                }

                await SendNoContentAsync();
                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}", req.Method);
                await SendNoContentAsync();
                break;
        }
    }
}
