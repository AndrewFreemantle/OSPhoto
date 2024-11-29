using CsvHelper.Configuration.Attributes;
using OSPhoto.Common.Database.Models;

namespace OSPhoto.Common.Services.Models;

public class CsvPhotoCommentRecord
{
    // id,photo_id,name,email,comment,date,path
    [Name("id")]
    public int Id { get; set; }
    [Name("name")]
    public string Name { get; set; } = string.Empty;
    [Name("email")]
    public string? Email { get; set; }
    [Name("comment")]
    public string Comment { get; set; } = string.Empty;
    [Name("date")]
    public string Date { get; set; } = string.Empty;
    [Name("path")]
    public string Path { get; set; } = string.Empty;


    public CsvPhotoCommentRecord()
    {
    }

    public CsvPhotoCommentRecord(CommentFileNotFound dbComment)
    {
        Name = dbComment.Name;
        Email = dbComment.Email;
        Comment = dbComment.Comment;
        Date = dbComment.CreatedUtc.ToString("u");
        Path = dbComment.Path;
    }
}
