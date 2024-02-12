namespace OSPhoto.Common.Extensions;

public static class StringUtils
{
    /// <summary>
    /// Convenience method that returns a new Guid as a string with the dashes removed
    /// </summary>
    public static string NewId() => Guid.NewGuid().ToString().Replace("-", "");
}
