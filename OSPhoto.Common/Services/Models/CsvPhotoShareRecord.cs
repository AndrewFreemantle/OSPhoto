using CsvHelper.Configuration.Attributes;

namespace OSPhoto.Common.Services.Models;

public class CsvPhotoShareRecord
{
    // shareid,sharename,title,description,public,hits,cover,comment,is_subdir,updated,password,conversion,ref_shareid
    [Name("shareid")]
    public int Id { get; set; }
    [Name("sharename")]
    public string ShareName { get; set; }
    [Name("title")]
    public string Title { get; set; }
    [Name("description")]
    public string Description { get; set; }
}
