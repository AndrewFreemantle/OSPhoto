using FFMpegCore;
using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Models;

public class VideoQuality : VideoCodec
{
    public VideoQuality(IMediaAnalysis videoInfo, FileInfo fileInfo) : base(videoInfo)
    {
        // Id is the full path to the video file, hex encoded.
        Id = fileInfo.FullName.ToHex();
        FileSize = fileInfo.Length;
    }

    public string Id { get; set; }
    [JsonPropertyName("filesize")]
    public long FileSize { get; set; }

    [JsonPropertyName("video_profile")]
    public int VideoProfile { get; set; }
    [JsonPropertyName("profile_name")]
    public int ProfileName { get; set; }    //: "flv"
    [JsonPropertyName("video_level")]
    public int VideoLevel { get; set; }     //: 0

}
