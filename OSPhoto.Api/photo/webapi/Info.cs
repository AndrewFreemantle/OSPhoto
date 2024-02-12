using System.Text.Json;

namespace OSPhoto.Api.photo.webapi;

public class InfoRequest : RequestBase
{
}

public class InfoResponse
{
    private static string data = @"{""version"":""3400"",""title"":"""",""about_me_title"":""About Me"",""sort_by"":""filename"",""sort_direction"":""asc"",""use_album_explorer"":false,""paging_use_bar"":true,""paging_item_count"":100,""folder_sort_direction"":""asc"",""folder_sort_only_name"":false,""allow_download_album"":false,""allow_download_orig"":false,""allow_download_video"":false,""disable_right_button"":false,""hide_search"":false,""hide_gps_from_normal_user"":false,""hide_rss_feed"":false,""enable_blog"":false,""external_host"":""https:\/\/localhost:4433"",""external_host_quickconnect"":""https:\/\/localhost:4433"",""allow_social_share"":false,""allow_social_upload"":false,""allow_social_upload_guest"":false,""social_network_list"":[{""name"":""Facebook"",""enable"":false,""allowShare"":true,""allowSingleUpload"":true,""allowMultiUpload"":true,""allowPhoto"":true,""allowVideo"":false},{""name"":""Twitter"",""enable"":false,""allowShare"":true,""allowSingleUpload"":true,""allowMultiUpload"":false,""allowPhoto"":true,""allowVideo"":false},{""name"":""Plurk"",""enable"":false,""allowShare"":true,""allowSingleUpload"":false,""allowMultiUpload"":false,""allowPhoto"":false,""allowVideo"":false},{""name"":""Weibo"",""enable"":false,""allowShare"":true,""allowSingleUpload"":true,""allowMultiUpload"":false,""allowPhoto"":true,""allowVideo"":false},{""name"":""QQ"",""enable"":false,""allowShare"":true,""allowSingleUpload"":true,""allowMultiUpload"":false,""allowPhoto"":true,""allowVideo"":false},{""name"":""YouTube"",""enable"":false,""allowShare"":false,""allowSingleUpload"":true,""allowMultiUpload"":false,""allowPhoto"":false,""allowVideo"":true},{""name"":""Flickr"",""enable"":false,""allowShare"":false,""allowSingleUpload"":true,""allowMultiUpload"":true,""allowPhoto"":true,""allowVideo"":false}],""virtual_tag"":{""desc_tag"":false,""geo_tag"":false,""people_tag"":false},""default_geo_location"":{""lng"":"""",""lat"":""""},""home_category"":null,""default_album_public"":false,""disable_aboutme"":false,""show_album_hit"":true,""use_dsm_account"":true,""collapse_left_panel"":false,""show_lightbox_information"":false,""use_pop_window_to_edit_desc"":false,""def_album_disable_conversion"":false,""support_large_file"":false,""support_moments"":false}";

    public bool Success { get; set; } = true;
    public JsonDocument Data { get; set; } = JsonDocument.Parse(data);
}


public class Info : Endpoint<InfoRequest, InfoResponse>
{
    public override void Configure()
    {
        Post("info.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(InfoRequest req, CancellationToken ct)
    {
        Console.WriteLine($"Info CALLED!");
        await SendAsync(new InfoResponse());
    }
}
