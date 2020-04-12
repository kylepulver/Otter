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
    public static class ListExtensions
    {
        [ThreadStatic]
        static System.Random randomNumberGenerator = new Random(DateTime.Now.Millisecond + System.Threading.Thread.CurrentThread.GetHashCode());

        /// <summary>
        /// Returns a sub-section of the current list, starting at the specified index and continuing to the end of the list.
        /// </summary>
        public static List<T> FromIndexToEnd<T>(this List<T> list, int start)
        {
            return list.GetRange(start, list.Count - start);
        }

        /// <summary>
        /// Returns the first index in the IList<T> where the target exists.  If the target cannot be found, returns -1.
        /// </summary>
        public static int IndexOf<T>(this IList<T> list, T target)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                if (list[i].Equals(target)) return i;
            }
            return -1;
        }

        /// Returns a randomly selected item from IList<T>
        public static T RandomElement<T>(this IList<T> list)
        {
            if (list.IsEmpty()) throw new IndexOutOfRangeException("Cannot retrieve a random value from an empty list");

            return list[Rand.Int(list.Count)]; // Using Otter's RNG for consistency
            //return list[randomNumberGenerator.Next(list.Count)];
        }

        /// <summary>
        /// Returns a randomly selected item from IList or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomElementOrDefault<T>(this IList<T> list)
        {
            if (list.IsEmpty()) return default(T);

            return RandomElement(list);
        }

        /// Returns a randomly selected item from IList<T> determined by a IEnumerable<float> of weights
        public static T RandomElement<T>(this IList<T> list, IEnumerable<float> weights)
        {
            if (list.IsEmpty()) throw new IndexOutOfRangeException("Cannot retrieve a random value from an empty list");
            if (list.Count != weights.Count()) throw new IndexOutOfRangeException("List of weights must be the same size as input list");

            var randomWeight = randomNumberGenerator.NextDouble() * weights.Sum();
            var totalWeight = 0f;
            var index = 0;
            foreach (var weight in weights)
            {
                totalWeight += weight;
                if (randomWeight <= totalWeight)
                {
                    break;
                }
            }

            return list[index];
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            // OrderBy and Sort are both broken for AOT compliation on older MonoTouch versions
            // https://bugzilla.xamarin.com/show_bug.cgi?id=2155#c11
            var shuffledList = new List<T>(list);
            T temp;
            for (var i = 0; i < shuffledList.Count; ++i)
            {
                temp = shuffledList[i];
                var swapIndex = randomNumberGenerator.Next(list.Count);
                shuffledList[i] = shuffledList[swapIndex];
                shuffledList[swapIndex] = temp;
            }
            return shuffledList;
        }

        public static IList<T> InPlaceShuffle<T>(this IList<T> list)
        {
            // OrderBy and Sort are both broken for AOT compliation on older MonoTouch versions
            // https://bugzilla.xamarin.com/show_bug.cgi?id=2155#c11

            for (var i = 0; i < list.Count; ++i)
            {
                var temp = list[i];
                var swapIndex = randomNumberGenerator.Next(list.Count);
                list[i] = list[swapIndex];
                list[swapIndex] = temp;
            }
            return list;
        }

        public static IList<T> InPlaceOrderBy<T, TKey>(this IList<T> list, Func<T, TKey> elementToSortValue) where TKey : IComparable
        {
            // Provides both and in-place sort as well as an AOT on iOS friendly replacement for OrderBy
            if (list.Count < 2)
            {
                return list;
            }

            int startIndex;
            int currentIndex;
            int smallestIndex;
            T temp;

            for (startIndex = 0; startIndex < list.Count; ++startIndex)
            {
                smallestIndex = startIndex;
                for (currentIndex = startIndex + 1; currentIndex < list.Count; ++currentIndex)
                {
                    if (elementToSortValue(list[currentIndex]).CompareTo(elementToSortValue(list[smallestIndex])) < 0)
                    {
                        smallestIndex = currentIndex;
                    }
                }
                temp = list[startIndex];
                list[startIndex] = list[smallestIndex];
                list[smallestIndex] = temp;
            }

            return list;
        }

        /// <summary>
        /// Attempts to Insert the item, but Adds it if the index is invalid.
        /// </summary>
        public static void InsertOrAdd<T>(this IList<T> list, int atIndex, T item)
        {
            if (atIndex >= 0 && atIndex < list.Count)
            {
                list.Insert(atIndex, item);
            }
            else
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Returns the element after the given element. This can wrap. If the element is the only one in the list, itself is returned.
        /// </summary>
        public static T ElementAfter<T>(this IList<T> list, T element, bool wrap = true)
        {
            var targetIndex = list.IndexOf(element) + 1;
            if (wrap)
            {
                return targetIndex >= list.Count ? list[0] : list[targetIndex];
            }
            return list[targetIndex];
        }

        /// <summary>
        /// Returns the element before the given element. This can wrap. If the element is the only one in the list, itself is returned.
        /// </summary>
        public static T ElementBefore<T>(this IList<T> list, T element, bool wrap = true)
        {
            var targetIndex = list.IndexOf(element) - 1;
            if (wrap)
            {
                return targetIndex < 0 ? list[list.Count - 1] : list[targetIndex];
            }
            return list[targetIndex];
        }
    }
}
