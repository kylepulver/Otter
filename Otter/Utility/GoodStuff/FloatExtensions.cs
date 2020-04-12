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
    public static class FloatExtensions
    {
        /// <summary>
        /// Maps a value in one range to the equivalent value in another range.
        /// </summary>
        public static float MapToRange(this float value, float range1Min, float range1Max, float range2Min, float range2Max)
        {
            return MapToRange(value, range1Min, range1Max, range2Min, range2Max, true);
        }

        /// <summary>
        /// Maps a value in one range to the equivalent value in another range.  Clamps the value to be valid within the range if clamp is specified as true.
        /// </summary>
        public static float MapToRange(this float value, float range1Min, float range1Max, float range2Min, float range2Max, bool clamp)
        {

            value = range2Min + ((value - range1Min) / (range1Max - range1Min)) * (range2Max - range2Min);

            if (clamp)
            {
                if (range2Min < range2Max)
                {
                    if (value > range2Max) value = range2Max;
                    if (value < range2Min) value = range2Min;
                }
                // Range that go negative are possible, for example from 0 to -1
                else
                {
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
        public static int ToPercent(this float value)
        {
            return Convert.ToInt32(value * 100);
        }
    }
}
