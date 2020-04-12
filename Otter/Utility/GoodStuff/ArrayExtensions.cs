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
using System.Linq;

namespace Otter.Utility.GoodStuff
{
    public static class ArrayExtensions
    {
        [ThreadStatic]
        static System.Random randomNumberGenerator = new Random(DateTime.Now.Millisecond + System.Threading.Thread.CurrentThread.GetHashCode());

        /// <summary>
        /// Returns the first index in the array where the target exists.  If the target cannot be found, returns -1.
        /// </summary>
        public static int IndexOf<T>(this T[] array, T target)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                if (array[i].Equals(target)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns a sub-section of the current array, starting at the specified index and continuing to the end of the array.
        /// </summary>
        public static T[] FromIndexToEnd<T>(this T[] array, int start)
        {
            var subSection = new T[array.Length - start];
            array.CopyTo(subSection, start);
            return subSection;
        }

        /// <summary>
        /// Wrapper for System.Array.FindIndex to allow it to be called directly on an array.
        /// </summary>
        public static int FindIndex<T>(this T[] array, Predicate<T> match)
        {
            return Array.FindIndex(array, match);
        }

        /// <summary>
        /// Wrapper for System.Array.FindIndex to allow it to be called directly on an array.
        /// </summary>
        public static int FindIndex<T>(this T[] array, int startIndex, Predicate<T> match)
        {
            return Array.FindIndex(array, startIndex, match);
        }

        /// <summary>
        /// Wrapper for System.Array.FindIndex to allow it to be called directly on an array.
        /// </summary>
        public static int FindIndex<T>(this T[] array, int startIndex, int count, Predicate<T> match)
        {
            return Array.FindIndex(array, startIndex, count, match);
        }

        /// Returns a randomly selected item from the array
        public static T RandomElement<T>(this T[] array)
        {
            if (array.Length == 0) throw new IndexOutOfRangeException("Cannot retrieve a random value from an empty array");

            return array[randomNumberGenerator.Next(array.Length)];
        }

        /// Returns a randomly selected item from the array determined by a float array of weights
        public static T RandomElement<T>(this T[] array, float[] weights)
        {
            return array.RandomElement(weights.ToList());
        }

        /// Returns a randomly selected item from the array determined by a List<float> of weights
        public static T RandomElement<T>(this T[] array, List<float> weights)
        {
            if (array.IsEmpty()) throw new IndexOutOfRangeException("Cannot retrieve a random value from an empty array");
            if (array.Count() != weights.Count()) throw new IndexOutOfRangeException("array of weights must be the same size as input array");

            var randomWeight = randomNumberGenerator.NextDouble() * weights.Sum();
            var totalWeight = 0f;
            var index = weights.FindIndex(weight => {
                totalWeight += weight;
                return randomWeight <= totalWeight;
            });

            return array[index];
        }

        /// <summary>
        /// Iterates over each element in the two dimensional array, passing in the element and the index to the provided callback.
        /// </summary>
        public static void EachWithIndex<T>(this T[,] collection, Action<T, int, int> callback)
        {
            for (var x = 0; x < collection.GetLength(0); ++x)
            {
                for (var y = 0; y < collection.GetLength(1); ++y)
                {
                    callback(collection[x, y], x, y);
                }
            }
        }

        /// <summary>
        /// Iterates over each element in the two dimensional array, passing in the index to the provided callback.
        /// </summary>
        public static void EachIndex<T>(this T[,] collection, Action<int, int> callback)
        {
            for (var x = 0; x < collection.GetLength(0); ++x)
            {
                for (var y = 0; y < collection.GetLength(1); ++y)
                {
                    callback(x, y);
                }
            }
        }
    }
}
