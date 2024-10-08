using FFMpegCore;

namespace OSPhoto.Common.Models;

public class VideoCodec
{
    public VideoCodec(IMediaAnalysis videoInfo)
    {
        Container = GetContainerFromFormatName(videoInfo.Format.FormatName);
        var codec = videoInfo.PrimaryVideoStream.GetCodecInfo();
        VideoCodecName = videoInfo.PrimaryVideoStream.CodecName;
        AudioCodecName = videoInfo.PrimaryAudioStream.CodecName;

        ResolutionX = videoInfo.PrimaryVideoStream.Width;
        ResolutionY = videoInfo.PrimaryVideoStream.Height;

        FrameBitrate = videoInfo.Format.BitRate;
        VideoBitrate = videoInfo.PrimaryVideoStream.BitRate;
        AudioBitrate = videoInfo.PrimaryAudioStream.BitRate;

    }

    public string Container { get; set; }
    [JsonPropertyName("vcodec")]
    public string VideoCodecName { get; set; }
    [JsonPropertyName("acodec")]
    public string AudioCodecName { get; set; }

    [JsonPropertyName("resolutionx")]
    public int ResolutionX { get; set; }
    [JsonPropertyName("resolutiony")]
    public int ResolutionY { get; set; }

    [JsonPropertyName("frame_bitrate")]
    public double FrameBitrate { get; set; }
    [JsonPropertyName("video_bitrate")]
    public double VideoBitrate { get; set; }
    [JsonPropertyName("audio_bitrate")]
    public double AudioBitrate { get; set; }

    private string GetContainerFromFormatName(string formatName)
    {
        if (formatName.Contains("flv")) return "flv";

        if (formatName.Contains("mp4")) return "mp4";

        return formatName;
    }
}
