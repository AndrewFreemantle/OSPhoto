using System.Net;
using Microsoft.AspNetCore.WebUtilities;

namespace OSPhoto.Web.Extensions
{
    public static class StringExtensions
    {
        public static string UrlEncode(this string str)
        {
            return WebUtility.UrlEncode(str);
        }

        public static string UrlDecode(this string str)
        {
            return WebUtility.UrlDecode(str);
        }
    }
}