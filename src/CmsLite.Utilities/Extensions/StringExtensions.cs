using System;

namespace CmsLite.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return String.IsNullOrEmpty(s);
        }

        public static string Format(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
    }
}
