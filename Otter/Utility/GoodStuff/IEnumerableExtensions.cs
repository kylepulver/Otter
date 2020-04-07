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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Otter.Utility.GoodStuff
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Iterates over each element in the IEnumerable, passing in the element to the provided callback.
        /// </summary>
        public static void Each<T>(this IEnumerable<T> iterable, Action<T> callback)
        {
            foreach (var value in iterable)
            {
                callback(value);
            }
        }

        /// <summary>
        /// Iterates over each element backwards in the IEnumerable, passing in the element to the provided callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iterable"></param>
        /// <param name="callback"></param>
        public static void EachReverse<T>(this IEnumerable<T> iterable, Action<T> callback)
        {
            for (var i = iterable.Count() - 1; i >= 0; i--)
            {
                callback(iterable.ElementAt(i));
            }
        }

        /// <summary>
        /// Iterates over each element in the IEnumerable, passing in the element to the provided callback.  Since the IEnumerable is
        /// not generic, a type must be specified as a type parameter to Each.
        /// </summary>
        /// <description>
        /// IEnumerable myCollection = new List<int>();
        /// ...
        /// myCollection.Each<int>(i => Debug.Log("i: " + i));
        /// </description>
        public static void Each<T>(this IEnumerable iterable, Action<T> callback)
        {
            foreach (T value in iterable)
            {
                callback(value);
            }
        }

        //			/// <summary>
        //			/// Iterates over each element in the IEnumerable, passing in the element to the provided callback.
        //			/// </summary>
        //			public static void Each(this IEnumerable iterable, Action<object> callback) {
        //				foreach(object value in iterable) {
        //					callback(value);
        //				}
        //			}

        /// <summary>
        /// Iterates over each element in the IEnumerable, passing in the element and the index to the provided callback.
        /// </summary>
        public static void EachWithIndex<T>(this IEnumerable<T> iterable, Action<T, int> callback)
        {
            var i = 0;
            foreach (var value in iterable)
            {
                callback(value, i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in the IEnumerable, passing in the element and the index to the provided callback.
        /// </summary>
        public static void EachWithIndex<T>(this IEnumerable iterable, Action<T, int> callback)
        {
            var i = 0;
            foreach (T value in iterable)
            {
                callback(value, i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in the two dimensional array, passing in the index to the provided callback.
        /// </summary>
        public static void EachIndex<T>(this IEnumerable<T> iterable, Action<int> callback)
        {
            var i = 0;
#pragma warning disable 0168
            foreach (var value in iterable)
            {
#pragma warning restore 0168
                callback(i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in the two dimensional array, passing in the index to the provided callback.
        /// </summary>
        public static void EachIndex<T>(this IEnumerable iterable, Action<int> callback)
        {
            var i = 0;
#pragma warning disable 0219
            foreach (T value in iterable)
            {
#pragma warning restore 0219
                callback(i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in both the iterable1 and iterable2 collections, passing in the current element of each collection into the provided callback.
        /// </summary>
        public static void InParallelWith<T, U>(this IEnumerable<T> iterable1, IEnumerable<U> iterable2, Action<T, U> callback)
        {
            if (iterable1.Count() != iterable2.Count()) throw new ArgumentException(string.Format("Both IEnumerables must be the same length, iterable1: {0}, iterable2: {1}", iterable1.Count(), iterable2.Count()));

            var i1Enumerator = iterable1.GetEnumerator();
            var i2Enumerator = iterable2.GetEnumerator();

            while (i1Enumerator.MoveNext())
            {
                i2Enumerator.MoveNext();
                callback(i1Enumerator.Current, i2Enumerator.Current);
            }
        }

        /// <summary>
        /// Iterates over each element in both the iterable1 and iterable2 collections, passing in the current element of each collection into the provided callback.
        /// </summary>
        public static void InParallelWith(this IEnumerable iterable1, IEnumerable iterable2, Action<object, object> callback)
        {
            var i1Enumerator = iterable1.GetEnumerator();
            var i2Enumerator = iterable2.GetEnumerator();
            var i1Count = 0;
            var i2Count = 0;
            while (i1Enumerator.MoveNext()) ++i1Count;
            while (i2Enumerator.MoveNext()) ++i2Count;
            if (i1Count != i2Count) throw new ArgumentException(string.Format("Both IEnumerables must be the same length, iterable1: {0}, iterable2: {1}", i1Count, i2Count));

            i1Enumerator.Reset();
            i2Enumerator.Reset();
            while (i1Enumerator.MoveNext())
            {
                i2Enumerator.MoveNext();
                callback(i1Enumerator.Current, i2Enumerator.Current);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> iterable)
        {
            return iterable.Count() == 0;
        }

        public static bool IsEmpty(this IEnumerable iterable)
        {
            // MoveNext returns false if we are at the end of the collection
            return !iterable.GetEnumerator().MoveNext();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> iterable)
        {
            return iterable.Count() > 0;
        }

        public static bool IsNotEmpty(this IEnumerable iterable)
        {
            // MoveNext returns false if we are at the end of the collection
            return iterable.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Matches all elements where the given condition is not true. This is the
        /// opposite of Linq's Where clause.
        /// </summary>
        public static IEnumerable<T> ExceptWhere<T>(this IEnumerable<T> iterable, Func<T, bool> condition)
        {
            return iterable.Where(element => !condition(element));
        }

        #region MoreLINQ project code
        // MinBy and MoreBy methods are provided via the MoreLINQ project (c) Jon Skeet
        // https://code.google.com/p/morelinq/source/browse/MoreLinq/MinBy.cs
        // https://code.google.com/p/morelinq/source/browse/MoreLinq/MaxBy.cs

        /// <summary>
        /// Returns the first element that has the smallest value (as determined by the selector) within the collection
        /// (as determined by the comparer).  This is equivalent to using Min except that the element itself
        /// is returned, and not the value used to make the Min determination.
        /// </summary>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the first element that has the smallest value (as determined by the selector) within the collection
        /// (as determined by the comparer).  This is equivalent to using Min except that the element itself
        /// is returned, and not the value used to make the Min determination.
        /// </summary>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var minValue = sourceIterator.Current;
                var minKey = selector(minValue);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        minValue = candidate;
                        minKey = candidateProjected;
                    }
                }
                return minValue;
            }
        }

        /// <summary>
        /// Returns the first element that has the largest value (as determined by the selector) within the collection
        /// (as determined by the comparer).  This is equivalent to using Max except that the element itself
        /// is returned, and not the value used to make the Max determination.
        /// </summary>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the first element that has the largest value (as determined by the selector) within the collection
        /// (as determined by the comparer).  This is equivalent to using Max except that the element itself
        /// is returned, and not the value used to make the Max determination.
        /// </summary>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var maxValue = sourceIterator.Current;
                var maxKey = selector(maxValue);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        maxValue = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return maxValue;
            }
        }
        #endregion
    }
}
