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

namespace Otter.Utility.GoodStuff
{
    public static class IntExtensions
    {
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
        public static void Times(this int iterations, Action callback)
        {
            for (var i = 0; i < iterations; ++i)
            {
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
        public static void Times(this int iterations, Action<int> callback)
        {
            for (var i = 0; i < iterations; ++i)
            {
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
        public static void UpTo(this int value, int endValue, Action<int> callback)
        {
            for (var i = value; i <= endValue; ++i)
            {
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
        public static void DownTo(this int value, int endValue, Action<int> callback)
        {
            for (var i = value; i >= endValue; --i)
            {
                callback(i);
            }
        }

        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        public static bool IsOdd(this int value)
        {
            return value % 2 == 1;
        }
    }
}
