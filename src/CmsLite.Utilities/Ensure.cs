using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CmsLite.Utilities
{
    public static class Ensure
    {
        private const string ArgumentIsNullMessage = "Cannot be null";
        private const string ArgumentIsNullOrEmptyMessage = "Cannot be null or empty";
        private const string ArgumentIsNotInCollectionMessage = "Collection does not contain {0}";
        private const string ArgumentIsNotGreaterThanMessage = "Cannot be greater than {0}";
        private const string ArgumentIsNotLessThanMessage = "Cannot be less than {0}";
        private const string ArgumentDoesNotMatchRegexMessage = "Does not match pattern";
        private const string ArgumentDoesAlreadyExistMessage = "Entry does not exist";

        public static string ArgumentIsNullMessageFormatMessage = ArgumentIsNullMessage + @"
Parameter name: {0}";
        public static string ArgumentIsNullOrEmptyMessageFormat = ArgumentIsNullOrEmptyMessage + @"
Parameter name: {0}";
        public static string ArgumentIsNotInCollectionFormat = ArgumentIsNotInCollectionMessage + @"
Parameter name: {0}";
        public static string ArgumentIsNotGreaterThanMessageFormat = ArgumentIsNotGreaterThanMessage + @"
Parameter name: {1}";
        public static string ArgumentIsNotLessThanMessageFormat = ArgumentIsNotLessThanMessage + @"
Parameter name: {1}";
        public static string ArgumentDoesNotMatchRegexMessageFormat = ArgumentDoesNotMatchRegexMessage + @"
Parameter name: {0}";
        public static string ArgumentDoesAlreadyExistMessageFormat = ArgumentDoesAlreadyExistMessage + @"
Parameter name: {0}";

        /// <summary>
        /// Throws an null exception if the argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ArgumentIsNotNull(object argument, string paramName)
        {
            if (argument == null) throw new ArgumentNullException(paramName, ArgumentIsNullMessage);
        }

        /// <summary>
        /// Throws an exception if the collection argument is null or empty.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotNullOrEmpty<T>(ICollection<T> argument, string paramName)
        {
            if (argument == null || !argument.Any()) throw new ArgumentException(ArgumentIsNullOrEmptyMessage, paramName);
        }

        /// <summary>
        /// Throws an exception if the collection argument is null or empty.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotNullOrEmpty<T>(IEnumerable<T> argument, string paramName)
        {
            if (argument == null || !argument.Any()) throw new ArgumentException(ArgumentIsNullOrEmptyMessage, paramName);
        }

        /// <summary>
        /// Throws an exception if the string argument is null or empty.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotNullOrEmpty(string argument, string paramName)
        {
            if (string.IsNullOrEmpty(argument)) throw new ArgumentException(ArgumentIsNullOrEmptyMessage, paramName);
        }

        /// <summary>
        /// Throws an exception if the argument is not in the collection.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsInInMemoryCollection<T>(ICollection<T> collection, T argument, string paramName)
        {
            if (!collection.Contains(argument)) throw new ArgumentException(string.Format(ArgumentIsNotInCollectionMessage, paramName), paramName);
        }

        /// <summary>
        /// Throws an exception if the argument is not in the collection.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsInInMemoryCollection<T>(IEnumerable<T> collection, T argument, string paramName)
        {
            if (!collection.Contains(argument)) throw new ArgumentException(string.Format(ArgumentIsNotInCollectionMessage, paramName), paramName);
        }

        /// <summary>
        /// Throws an exception if the argument is not greater than the specified max value.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotGreaterThan(string argument, string paramName, int maxLength)
        {
            ArgumentIsNotNull(argument, paramName);

            if (argument.Length > maxLength) throw new ArgumentException(string.Format(ArgumentIsNotGreaterThanMessage, maxLength), paramName);
        }

        /// <summary>
        /// Throws an exception if the argument is not greater than the specified max value.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotGreaterThan(int argument, string paramName, int maxValue)
        {
            if (argument > maxValue) throw new ArgumentException(string.Format(ArgumentIsNotGreaterThanMessage, maxValue), paramName);
        }

        /// <summary>
        /// Throws an exception if the argument is not less than the specified max value.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotLessThan(int argument, string paramName, int minValue)
        {
            if (argument < minValue) throw new ArgumentException(string.Format(ArgumentIsNotLessThanMessage, minValue), paramName);
        }

        /// <summary>
        /// Throws an exception if the argument is not greater than the specified max value.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotGreaterThan(DateTime argument, string paramName, DateTime maxValue)
        {
            if (argument > maxValue) throw new ArgumentException(string.Format(ArgumentIsNotGreaterThanMessage, maxValue), paramName);
        }

        /// <summary>
        /// Throws an exception if the argument is not less than the specified max value.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentIsNotLessThan(DateTime argument, string paramName, DateTime minValue)
        {
            if (argument < minValue) throw new ArgumentException(string.Format(ArgumentIsNotLessThanMessage, minValue), paramName);
        }

        /// <summary>
        /// Throws an exception if the argument does not match the specified Regex pattern.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentMatchesRegex(string argument, string paramName, Regex pattern)
        {
            ArgumentIsNotNullOrEmpty(argument, paramName);

            if (!pattern.IsMatch(argument)) throw new ArgumentException(ArgumentDoesNotMatchRegexMessage, paramName);
        }

        /// <summary>
        /// Throws an exception if the argument does not already exist
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static void ArgumentExists<T>(T argument, string paramName) where T : class
        {
            if (argument == null)
                throw new ArgumentException(ArgumentDoesAlreadyExistMessage, paramName);
        }
    }
}
