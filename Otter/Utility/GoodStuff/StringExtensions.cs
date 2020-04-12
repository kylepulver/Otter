#region LICENSE
// This project is licensed under The MIT License (MIT)
//
// Copyright 2013 David Koontz, Logan Barnett, Corey Nolan, Alex Burley
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//		of this software and associated documentation files (the "Software"), to deal
//		in the Software without restriction, including without limitation the rights
//		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//		copies of the Software, and to permit persons to whom the Software is
//		furnished to do so, subject to the following conditions:
//
//		The above copyright notice and this permission notice shall be included in
//		all copies or substantial portions of the Software.
//
//		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//		THE SOFTWARE.
//
// Please direct questions, patches, and suggestions to the project page at
// https://github.com/dkoontz/GoodStuff
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Otter.Utility.GoodStuff
{
    public static class StringExtensions
    {
        /// <summary>
        /// Interpolates the arguments into the string using string.Format
        /// </summary>
        /// <param name="formatString">The string to be interpolated into</param>
        /// <param name="args">The values to be interpolated into the string </param>
        public static string Interpolate(this string formatString, params object[] args)
        {
            return string.Format(formatString, args);
        }

        /// <summary>
        /// Alias for <see cref="Interpolate"/> for the typing averse
        /// </summary>
        /// <param name="formatString">The string to be interpolated into</param>
        /// <param name="args">The values to be interpolated into the string </param>
        public static string Fmt(this string formatString, params object[] args)
        {
            return Interpolate(formatString, args);
        }

        public static T ToEnum<T>(this string enumValueName)
        {
            return (T)Enum.Parse(typeof(T), enumValueName);
        }

        public static T ToEnum<T>(this string enumValueName, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), enumValueName, ignoreCase);
        }

        public static string Last(this string value, int count)
        {
            if (count > value.Length) throw new ArgumentOutOfRangeException(string.Format("Cannot return more characters than exist in the string (wanted {0} string contains {1}", count, value.Length));

            return value.Substring(value.Length - count, count);
        }

        public static string SnakeCase(this string camelizedString)
        {
            var parts = new List<string>();
            var currentWord = new StringBuilder();

            foreach (var c in camelizedString)
            {
                if (char.IsUpper(c) && currentWord.Length > 0)
                {
                    parts.Add(currentWord.ToString());
                    currentWord = new StringBuilder();
                }
                currentWord.Append(char.ToLower(c));
            }

            if (currentWord.Length > 0)
            {
                parts.Add(currentWord.ToString());
            }

            return string.Join("_", parts.ToArray());
        }

        public static string Capitalize(this string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1);
        }
    }
}
