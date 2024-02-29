using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace OSPhoto.Common.Extensions;

/// <summary>
/// Collection of helper methods for reading the Exif data from the ImageSharp <see cref="ExifProfile"/> class
/// </summary>
public static class ExifProfileExtensions
{
    /// <summary>
    /// Attempts to return the string value for the given <see cref="ExifTag"/>
    /// </summary>
    /// <param name="tag">An <see cref="ExifTag&lt;&gt;"/> where the TValueType is either <see cref="string"/> or <see cref="Rational"/></param>
    public static string? GetValueString(this ExifProfile? exifProfile, ExifTag tag)
    {
        if (exifProfile == null)
            return null;

        if (tag is ExifTag<string> exifTagString)
        {
            if (exifProfile.TryGetValue(exifTagString, out var value))
                return value.Value;
        }
        else if (tag is ExifTag<Rational> exifTagRational)
        {
            if (exifProfile.TryGetValue(exifTagRational, out var value))
                return value.Value.ToString();
        }
        else if (tag is ExifTag<ushort> exifTagUShort)
        {
            if (exifProfile.TryGetValue(exifTagUShort, out var value))
                return value.Value.ToString();
        }

        return null;
    }

    /// <summary>
    /// Attempts to return the ushort value for the given <see cref="ExifTag"/>
    /// </summary>
    /// <param name="tag">An <see cref="ExifTag&lt;&gt;"/> where the TValueType is <see cref="ushort"/></param>
    public static ushort? GetValueUShort(this ExifProfile? exifProfile, ExifTag<ushort> tag)
    {
        if (exifProfile == null)
            return null;

        if (exifProfile.TryGetValue(tag, out var valueArray))
            return valueArray.Value;

        return null;
    }

    /// <summary>
    /// Attempts to return the ushort value for the given <see cref="ExifTag"/>
    /// </summary>
    /// <param name="tag">An <see cref="ExifTag&lt;&gt;"/> where the TValueType is a <see cref="ushort"/> array</param>
    public static ushort? GetValueUShortArray(this ExifProfile? exifProfile, ExifTag<ushort[]> tag)
    {
        if (exifProfile == null)
            return null;

        if (exifProfile.TryGetValue(tag, out var valueArray))
            return valueArray.Value is { Length: > 0 } ? valueArray.Value.First() : null; // just return the first value

        return null;
    }

    /// <summary>
    /// Attempts to return the double value for the given <see cref="ExifTag"/>
    /// </summary>
    /// <param name="tag">An <see cref="ExifTag&lt;&gt;"/> where the TValueType is <see cref="Rational"/></param>
    public static double? GetValueDouble(this ExifProfile? exifProfile, ExifTag<Rational> tag)
    {
        if (exifProfile == null)
            return null;

        if (exifProfile.TryGetValue(tag, out var value))
            return value.Value.ToDouble();

        return null;
    }

    /// <summary>
    /// Attempts to return the GPS Latitude value converted from Days-Minutes-Seconds (DMS / Sexagesimal Degrees) to Decimal Degrees
    /// <remarks>https://en.wikipedia.org/wiki/Sexagesimal_degrees</remarks>
    /// <remarks>https://en.wikipedia.org/wiki/Decimal_degrees</remarks>
    /// </summary>
    public static float? GetGpsLatitudeAsDecimalDegrees(this ExifProfile? exifProfile)
    {
        if (exifProfile == null)
            return null;

        if (exifProfile.TryGetValue(ExifTag.GPSLatitude, out var lat)
            && exifProfile.TryGetValue(ExifTag.GPSLatitudeRef, out var latRef))
        {
            if (lat.Value == null)
                return null;

            // convert the DMS (Days, Minutes, Seconds) format of 3 values into Decimal Degrees
            return (lat.Value[0].ToSingle() // Days
                    + (lat.Value[1].ToSingle() / 60) // Minutes
                    + (lat.Value[2].ToSingle() / 3600)) // Seconds
                   * (latRef.Value == "N" ? 1 : -1); // positive values are north of the equator
        }

        return null;
    }

    /// <summary>
    /// Attempts to return the GPS Longitude value converted from Days-Minutes-Seconds (DMS / Sexagesimal Degrees) to Decimal Degrees
    /// <remarks>https://en.wikipedia.org/wiki/Sexagesimal_degrees</remarks>
    /// <remarks>https://en.wikipedia.org/wiki/Decimal_degrees</remarks>
    /// </summary>
    public static float? GetGpsLongitudeAsDecimalDegrees(this ExifProfile? exifProfile)
    {
        if (exifProfile == null)
            return null;

        if (exifProfile.TryGetValue(ExifTag.GPSLongitude, out var lng)
            && exifProfile.TryGetValue(ExifTag.GPSLongitudeRef, out var lngRef))
        {
            if (lng.Value == null)
                return null;

            // convert the DMS (Days, Minutes, Seconds) format of 3 values into Decimal Degrees
            return (lng.Value[0].ToSingle() // Days
                    + (lng.Value[1].ToSingle() / 60) // Minutes
                    + (lng.Value[2].ToSingle() / 3600)) // Seconds
                   * (lngRef.Value == "E" ? 1 : -1); // positive values are east of the Prime Meridian
        }

        return null;
    }
}
