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

namespace Otter.Utility.GoodStuff
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Iterates over a Dictionary<T> passing in both the key and value to the provided callback.
        /// </summary>
        public static void Each<T1, T2>(this Dictionary<T1, T2> dictionary, Action<T1, T2> callback)
        {
            foreach (var keyValuePair in dictionary)
            {
                callback(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        /// Iterates over a Dictionary<T> passing in both the key and value to the provided callback.
        /// </summary>
        public static void EachWithIndex<T1, T2>(this Dictionary<T1, T2> dictionary, Action<T1, T2, int> callback)
        {
            var i = 0;
            foreach (var keyValuePair in dictionary)
            {
                callback(keyValuePair.Key, keyValuePair.Value, i++);
            }
        }

        public static void RemoveAll<T1, T2>(this Dictionary<T1, T2> dictionary, Predicate<T1, T2> callback)
        {
            var keysToRemove = new List<T1>();
            foreach (var keyValuePair in dictionary)
            {
                if (callback(keyValuePair.Key, keyValuePair.Value))
                {
                    keysToRemove.Add(keyValuePair.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                dictionary.Remove(key);
            }
        }
    }
}
