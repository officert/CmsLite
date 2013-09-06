using System;

namespace CmsLite.Web.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return String.IsNullOrEmpty(s);
        }
    }
}
