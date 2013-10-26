using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsLite.Utilities
{
    public static class Ensure
    {
        public const string ArgumentNullError = "Cannot be null";
        public const string ArgumentNullOrEmptyError = "Cannot be null or empty";

        public static void ArgumentNotNull(object argument, string name)
        {
            if (argument == null) throw new ArgumentNullException(name, ArgumentNullError);
        }

        public static void ArgumentNotNullOrEmpty<T>(ICollection<T> argument, string name) where T : class
        {
            if (argument == null || !argument.Any()) throw new ArgumentException(ArgumentNullOrEmptyError, name);
        }

        public static void ArgumentNotNullOrEmpty(string argument, string name)
        {
            if (String.IsNullOrEmpty(argument)) throw new ArgumentException(ArgumentNullOrEmptyError, name);
        }
    }
}
