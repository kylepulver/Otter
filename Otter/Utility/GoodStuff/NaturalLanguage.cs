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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {

    public delegate bool Predicate<T1, T2>(T1 item1, T2 item2);

    public static class IntExtensions {
        /// <summary>
        /// Calls the provided callback action repeatedly.
        /// </summary>
        /// <description>
        /// Used to invoke an action a fixed number of times.
        /// 
        /// 5.Times(() => Console.WriteLine("Hey!"));
        /// 
        /// is the equivalent of
        /// 
        /// for(var i = 0; i < 5; i++) {
        ///     Console.WriteLine("Hey!");
        /// }
        /// </description>
        public static void Times(this int iterations, Action callback) {
            for (var i = 0; i < iterations; ++i) {
                callback();
            }
        }

        /// <summary>
        /// Calls the provided callback action repeatedly passing in the current value of i
        /// </summary>
        /// <description>
        /// Used to invoke an action a fixed number of times.
        /// 
        /// 5.Times(i => Console.WriteLine("Hey # " + i));
        /// 
        /// is the equivalent of
        /// 
        /// for(var i = 0; i < 5; i++) {
        ///     Console.WriteLine("Hey # " + i);
        /// }
        /// </description>
        public static void Times(this int iterations, Action<int> callback) {
            for (var i = 0; i < iterations; ++i) {
                callback(i);
            }
        }

        /// <summary>
        /// Iterates from the start up to the given end value inclusive, calling the provided callback with each value in the sequence.
        /// </summary>
        /// <description>
        /// Used to iterate from a start value to a target value
        /// 
        /// 0.UpTo(5, i => Console.WriteLine(i));
        /// 
        /// is the equivalent of
        /// 
        /// for(var i = 0; i <= 5; i++) {
        ///     Console.WriteLine(i);
        /// }
        /// </description>
        public static void UpTo(this int value, int endValue, Action<int> callback) {
            for (var i = value; i <= endValue; ++i) {
                callback(i);
            }
        }

        /// <summary>
        /// Iterates from the start down to the given end value inclusive, calling the provided callback with each value in the sequence.
        /// </summary>
        /// <description>
        /// Used to iterate from a start value to a target value
        /// 
        /// 5.DownTo(0, i => Console.WriteLine(i));
        /// 
        /// is the equivalent of
        /// 
        /// for(var i = 5; i >= 0; i++) {
        ///     Console.WriteLine(i);
        /// }
        /// </description>
        public static void DownTo(this int value, int endValue, Action<int> callback) {
            for (var i = value; i >= endValue; --i) {
                callback(i);
            }
        }

        public static bool IsEven(this int value) {
            return value % 2 == 0;
        }

        public static bool IsOdd(this int value) {
            return value % 2 == 1;
        }
    }

    public static class FloatExtensions {
        /// <summary>
        /// Maps a value in one range to the equivalent value in another range.
        /// </summary>
        public static float MapToRange(this float value, float range1Min, float range1Max, float range2Min, float range2Max) {
            return MapToRange(value, range1Min, range1Max, range2Min, range2Max, true);
        }

        /// <summary>
        /// Maps a value in one range to the equivalent value in another range.  Clamps the value to be valid within the range if clamp is specified as true.
        /// </summary>
        public static float MapToRange(this float value, float range1Min, float range1Max, float range2Min, float range2Max, bool clamp) {

            value = range2Min + ((value - range1Min) / (range1Max - range1Min)) * (range2Max - range2Min);

            if (clamp) {
                if (range2Min < range2Max) {
                    if (value > range2Max) value = range2Max;
                    if (value < range2Min) value = range2Min;
                }
                // Range that go negative are possible, for example from 0 to -1
                else {
                    if (value > range2Min) value = range2Min;
                    if (value < range2Max) value = range2Max;
                }
            }
            return value;
        }

        /// <summary>
        /// Converts a float into a percent value (1 => 100%)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToPercent(this float value) {
            return Convert.ToInt32(value * 100);
        }
    }

    public static class IEnumerableExtensions {
        /// <summary>
        /// Iterates over each element in the IEnumerable, passing in the element to the provided callback.
        /// </summary>
        public static void Each<T>(this IEnumerable<T> iterable, Action<T> callback) {
            foreach (var value in iterable) {
                callback(value);
            }
        }

        /// <summary>
        /// Iterates over each element backwards in the IEnumerable, passing in the element to the provided callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iterable"></param>
        /// <param name="callback"></param>
        public static void EachReverse<T>(this IEnumerable<T> iterable, Action<T> callback) {
            for (var i = iterable.Count() - 1; i >= 0; i--) {
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
        public static void Each<T>(this IEnumerable iterable, Action<T> callback) {
            foreach (T value in iterable) {
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
        public static void EachWithIndex<T>(this IEnumerable<T> iterable, Action<T, int> callback) {
            var i = 0;
            foreach (var value in iterable) {
                callback(value, i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in the IEnumerable, passing in the element and the index to the provided callback.
        /// </summary>
        public static void EachWithIndex<T>(this IEnumerable iterable, Action<T, int> callback) {
            var i = 0;
            foreach (T value in iterable) {
                callback(value, i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in the two dimensional array, passing in the index to the provided callback.
        /// </summary>
        public static void EachIndex<T>(this IEnumerable<T> iterable, Action<int> callback) {
            var i = 0;
#pragma warning disable 0168
            foreach (var value in iterable) {
#pragma warning restore 0168
                callback(i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in the two dimensional array, passing in the index to the provided callback.
        /// </summary>
        public static void EachIndex<T>(this IEnumerable iterable, Action<int> callback) {
            var i = 0;
#pragma warning disable 0219
            foreach (T value in iterable) {
#pragma warning restore 0219
                callback(i);
                ++i;
            }
        }

        /// <summary>
        /// Iterates over each element in both the iterable1 and iterable2 collections, passing in the current element of each collection into the provided callback.
        /// </summary>
        public static void InParallelWith<T, U>(this IEnumerable<T> iterable1, IEnumerable<U> iterable2, Action<T, U> callback) {
            if (iterable1.Count() != iterable2.Count()) throw new ArgumentException(string.Format("Both IEnumerables must be the same length, iterable1: {0}, iterable2: {1}", iterable1.Count(), iterable2.Count()));

            var i1Enumerator = iterable1.GetEnumerator();
            var i2Enumerator = iterable2.GetEnumerator();

            while (i1Enumerator.MoveNext()) {
                i2Enumerator.MoveNext();
                callback(i1Enumerator.Current, i2Enumerator.Current);
            }
        }

        /// <summary>
        /// Iterates over each element in both the iterable1 and iterable2 collections, passing in the current element of each collection into the provided callback.
        /// </summary>
        public static void InParallelWith(this IEnumerable iterable1, IEnumerable iterable2, Action<object, object> callback) {
            var i1Enumerator = iterable1.GetEnumerator();
            var i2Enumerator = iterable2.GetEnumerator();
            var i1Count = 0;
            var i2Count = 0;
            while (i1Enumerator.MoveNext()) ++i1Count;
            while (i2Enumerator.MoveNext()) ++i2Count;
            if (i1Count != i2Count) throw new ArgumentException(string.Format("Both IEnumerables must be the same length, iterable1: {0}, iterable2: {1}", i1Count, i2Count));

            i1Enumerator.Reset();
            i2Enumerator.Reset();
            while (i1Enumerator.MoveNext()) {
                i2Enumerator.MoveNext();
                callback(i1Enumerator.Current, i2Enumerator.Current);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> iterable) {
            return iterable.Count() == 0;
        }

        public static bool IsEmpty(this IEnumerable iterable) {
            // MoveNext returns false if we are at the end of the collection
            return !iterable.GetEnumerator().MoveNext();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> iterable) {
            return iterable.Count() > 0;
        }

        public static bool IsNotEmpty(this IEnumerable iterable) {
            // MoveNext returns false if we are at the end of the collection
            return iterable.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Matches all elements where the given condition is not true. This is the
        /// opposite of Linq's Where clause.
        /// </summary>
        public static IEnumerable<T> ExceptWhere<T>(this IEnumerable<T> iterable, Func<T, bool> condition) {
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
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the first element that has the smallest value (as determined by the selector) within the collection 
        /// (as determined by the comparer).  This is equivalent to using Min except that the element itself
        /// is returned, and not the value used to make the Min determination.
        /// </summary>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer) {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator()) {
                if (!sourceIterator.MoveNext()) {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var minValue = sourceIterator.Current;
                var minKey = selector(minValue);
                while (sourceIterator.MoveNext()) {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0) {
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
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the first element that has the largest value (as determined by the selector) within the collection 
        /// (as determined by the comparer).  This is equivalent to using Max except that the element itself
        /// is returned, and not the value used to make the Max determination.
        /// </summary>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer) {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator()) {
                if (!sourceIterator.MoveNext()) {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var maxValue = sourceIterator.Current;
                var maxKey = selector(maxValue);
                while (sourceIterator.MoveNext()) {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0) {
                        maxValue = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return maxValue;
            }
        }
        #endregion
    }

    public static class ArrayExtensions {
        [ThreadStatic]
        static System.Random randomNumberGenerator = new Random(DateTime.Now.Millisecond + System.Threading.Thread.CurrentThread.GetHashCode());

        /// <summary>
        /// Returns the first index in the array where the target exists.  If the target cannot be found, returns -1.
        /// </summary>
        public static int IndexOf<T>(this T[] array, T target) {
            for (var i = 0; i < array.Length; ++i) {
                if (array[i].Equals(target)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns a sub-section of the current array, starting at the specified index and continuing to the end of the array.
        /// </summary>
        public static T[] FromIndexToEnd<T>(this T[] array, int start) {
            var subSection = new T[array.Length - start];
            array.CopyTo(subSection, start);
            return subSection;
        }

        /// <summary>
        /// Wrapper for System.Array.FindIndex to allow it to be called directly on an array.
        /// </summary>
        public static int FindIndex<T>(this T[] array, Predicate<T> match) {
            return Array.FindIndex(array, match);
        }

        /// <summary>
        /// Wrapper for System.Array.FindIndex to allow it to be called directly on an array.
        /// </summary>
        public static int FindIndex<T>(this T[] array, int startIndex, Predicate<T> match) {
            return Array.FindIndex(array, startIndex, match);
        }

        /// <summary>
        /// Wrapper for System.Array.FindIndex to allow it to be called directly on an array.
        /// </summary>
        public static int FindIndex<T>(this T[] array, int startIndex, int count, Predicate<T> match) {
            return Array.FindIndex(array, startIndex, count, match);
        }

        /// Returns a randomly selected item from the array
        public static T RandomElement<T>(this T[] array) {
            if (array.Length == 0) throw new IndexOutOfRangeException("Cannot retrieve a random value from an empty array");

            return array[randomNumberGenerator.Next(array.Length)];
        }

        /// Returns a randomly selected item from the array determined by a float array of weights
        public static T RandomElement<T>(this T[] array, float[] weights) {
            return array.RandomElement(weights.ToList());
        }

        /// Returns a randomly selected item from the array determined by a List<float> of weights
        public static T RandomElement<T>(this T[] array, List<float> weights) {
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
        public static void EachWithIndex<T>(this T[,] collection, Action<T, int, int> callback) {
            for (var x = 0; x < collection.GetLength(0); ++x) {
                for (var y = 0; y < collection.GetLength(1); ++y) {
                    callback(collection[x, y], x, y);
                }
            }
        }

        /// <summary>
        /// Iterates over each element in the two dimensional array, passing in the index to the provided callback.
        /// </summary>
        public static void EachIndex<T>(this T[,] collection, Action<int, int> callback) {
            for (var x = 0; x < collection.GetLength(0); ++x) {
                for (var y = 0; y < collection.GetLength(1); ++y) {
                    callback(x, y);
                }
            }
        }
    }

    public static class ListExtensions {
        [ThreadStatic]
        static System.Random randomNumberGenerator = new Random(DateTime.Now.Millisecond + System.Threading.Thread.CurrentThread.GetHashCode());

        /// <summary>
        /// Returns a sub-section of the current list, starting at the specified index and continuing to the end of the list.
        /// </summary>
        public static List<T> FromIndexToEnd<T>(this List<T> list, int start) {
            return list.GetRange(start, list.Count - start);
        }

        /// <summary>
        /// Returns the first index in the IList<T> where the target exists.  If the target cannot be found, returns -1.
        /// </summary>
        public static int IndexOf<T>(this IList<T> list, T target) {
            for (var i = 0; i < list.Count; ++i) {
                if (list[i].Equals(target)) return i;
            }
            return -1;
        }

        /// Returns a randomly selected item from IList<T>
        public static T RandomElement<T>(this IList<T> list) {
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
        public static T RandomElementOrDefault<T>(this IList<T> list) {
            if (list.IsEmpty()) return default(T);

            return RandomElement(list);
        }

        /// Returns a randomly selected item from IList<T> determined by a IEnumerable<float> of weights
        public static T RandomElement<T>(this IList<T> list, IEnumerable<float> weights) {
            if (list.IsEmpty()) throw new IndexOutOfRangeException("Cannot retrieve a random value from an empty list");
            if (list.Count != weights.Count()) throw new IndexOutOfRangeException("List of weights must be the same size as input list");

            var randomWeight = randomNumberGenerator.NextDouble() * weights.Sum();
            var totalWeight = 0f;
            var index = 0;
            foreach (var weight in weights) {
                totalWeight += weight;
                if (randomWeight <= totalWeight) {
                    break;
                }
            }

            return list[index];
        }

        public static IList<T> Shuffle<T>(this IList<T> list) {
            // OrderBy and Sort are both broken for AOT compliation on older MonoTouch versions
            // https://bugzilla.xamarin.com/show_bug.cgi?id=2155#c11
            var shuffledList = new List<T>(list);
            T temp;
            for (var i = 0; i < shuffledList.Count; ++i) {
                temp = shuffledList[i];
                var swapIndex = randomNumberGenerator.Next(list.Count);
                shuffledList[i] = shuffledList[swapIndex];
                shuffledList[swapIndex] = temp;
            }
            return shuffledList;
        }

        public static IList<T> InPlaceShuffle<T>(this IList<T> list) {
            // OrderBy and Sort are both broken for AOT compliation on older MonoTouch versions
            // https://bugzilla.xamarin.com/show_bug.cgi?id=2155#c11

            for (var i = 0; i < list.Count; ++i) {
                var temp = list[i];
                var swapIndex = randomNumberGenerator.Next(list.Count);
                list[i] = list[swapIndex];
                list[swapIndex] = temp;
            }
            return list;
        }

        public static IList<T> InPlaceOrderBy<T, TKey>(this IList<T> list, Func<T, TKey> elementToSortValue) where TKey : IComparable {
            // Provides both and in-place sort as well as an AOT on iOS friendly replacement for OrderBy
            if (list.Count < 2) {
                return list;
            }

            int startIndex;
            int currentIndex;
            int smallestIndex;
            T temp;

            for (startIndex = 0; startIndex < list.Count; ++startIndex) {
                smallestIndex = startIndex;
                for (currentIndex = startIndex + 1; currentIndex < list.Count; ++currentIndex) {
                    if (elementToSortValue(list[currentIndex]).CompareTo(elementToSortValue(list[smallestIndex])) < 0) {
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
        public static void InsertOrAdd<T>(this IList<T> list, int atIndex, T item) {
            if (atIndex >= 0 && atIndex < list.Count) {
                list.Insert(atIndex, item);
            }
            else {
                list.Add(item);
            }
        }

        /// <summary>
        /// Returns the element after the given element. This can wrap. If the element is the only one in the list, itself is returned.
        /// </summary>
        public static T ElementAfter<T>(this IList<T> list, T element, bool wrap = true) {
            var targetIndex = list.IndexOf(element) + 1;
            if (wrap) {
                return targetIndex >= list.Count ? list[0] : list[targetIndex];
            }
            return list[targetIndex];
        }

        /// <summary>
        /// Returns the element before the given element. This can wrap. If the element is the only one in the list, itself is returned.
        /// </summary>
        public static T ElementBefore<T>(this IList<T> list, T element, bool wrap = true) {
            var targetIndex = list.IndexOf(element) - 1;
            if (wrap) {
                return targetIndex < 0 ? list[list.Count - 1] : list[targetIndex];
            }
            return list[targetIndex];
        }
    }

    public static class DictionaryExtensions {
        /// <summary>
        /// Iterates over a Dictionary<T> passing in both the key and value to the provided callback.
        /// </summary>
        public static void Each<T1, T2>(this Dictionary<T1, T2> dictionary, Action<T1, T2> callback) {
            foreach (var keyValuePair in dictionary) {
                callback(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        /// Iterates over a Dictionary<T> passing in both the key and value to the provided callback.
        /// </summary>
        public static void EachWithIndex<T1, T2>(this Dictionary<T1, T2> dictionary, Action<T1, T2, int> callback) {
            var i = 0;
            foreach (var keyValuePair in dictionary) {
                callback(keyValuePair.Key, keyValuePair.Value, i++);
            }
        }

        public static void RemoveAll<T1, T2>(this Dictionary<T1, T2> dictionary, Predicate<T1, T2> callback) {
            var keysToRemove = new List<T1>();
            foreach (var keyValuePair in dictionary) {
                if (callback(keyValuePair.Key, keyValuePair.Value)) {
                    keysToRemove.Add(keyValuePair.Key);
                }
            }

            foreach (var key in keysToRemove) {
                dictionary.Remove(key);
            }
        }
    }

    public static class StringExtensions {
        /// <summary>
        /// Interpolates the arguments into the string using string.Format
        /// </summary>
        /// <param name="formatString">The string to be interpolated into</param>
        /// <param name="args">The values to be interpolated into the string </param>
        public static string Interpolate(this string formatString, params object[] args) {
            return string.Format(formatString, args);
        }

        /// <summary>
        /// Alias for <see cref="Interpolate"/> for the typing averse
        /// </summary>
        /// <param name="formatString">The string to be interpolated into</param>
        /// <param name="args">The values to be interpolated into the string </param>
        public static string Fmt(this string formatString, params object[] args) {
            return Interpolate(formatString, args);
        }

        public static T ToEnum<T>(this string enumValueName) {
            return (T)Enum.Parse(typeof(T), enumValueName);
        }

        public static T ToEnum<T>(this string enumValueName, bool ignoreCase) {
            return (T)Enum.Parse(typeof(T), enumValueName, ignoreCase);
        }

        public static string Last(this string value, int count) {
            if (count > value.Length) throw new ArgumentOutOfRangeException(string.Format("Cannot return more characters than exist in the string (wanted {0} string contains {1}", count, value.Length));

            return value.Substring(value.Length - count, count);
        }

        public static string SnakeCase(this string camelizedString) {
            var parts = new List<string>();
            var currentWord = new StringBuilder();

            foreach (var c in camelizedString) {
                if (char.IsUpper(c) && currentWord.Length > 0) {
                    parts.Add(currentWord.ToString());
                    currentWord = new StringBuilder();
                }
                currentWord.Append(char.ToLower(c));
            }

            if (currentWord.Length > 0) {
                parts.Add(currentWord.ToString());
            }

            return string.Join("_", parts.ToArray());
        }

        public static string Capitalize(this string word) {
            return word.Substring(0, 1).ToUpper() + word.Substring(1);
        }
    }

    public static class TypeExtensions {
        /// <summary>
        /// Returns an array of all concrete subclasses of the provided type.
        /// </summary>
        public static Type[] Subclasses(this Type type) {
            var typeList = new List<System.Type>();
            System.AppDomain.CurrentDomain.GetAssemblies().Each(a => typeList.AddRange(a.GetTypes()));
            return typeList.Where(t => t.IsSubclassOf(type) && !t.IsAbstract).ToArray();
        }

        /// <summary>
        /// Returns an array of the provided type and all concrete subclasses of that type.
        /// </summary>
        public static Type[] TypeAndSubclasses(this Type type) {
            var typeList = new List<System.Type>();
            System.AppDomain.CurrentDomain.GetAssemblies().Each(a => typeList.AddRange(a.GetTypes()));
            return typeList.Where(t => (t == type || t.IsSubclassOf(type)) && !t.IsAbstract).ToArray();
        }
    }

    public static class EnumExtensions {
        /// <summary>
        /// Returns the next enum value wrapping to the first value if passed the last
        /// </summary>
        public static T Next<T>(this Enum enumValue) {
            var values = Enum.GetValues(enumValue.GetType());
            var enumerator = values.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (enumerator.Current.Equals(enumValue)) {
                    if (enumerator.MoveNext()) {
                        return (T)enumerator.Current;
                    }
                }
            }
            return default(T);
        }
    }
}