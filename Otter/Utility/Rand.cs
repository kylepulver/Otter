using System;
using System.Collections.Generic;
using System.Linq;

//thanks to chevy ray for this class
namespace Otter {
    /// <summary>
    /// Class full of random number generation related functions.
    /// </summary>
    public static class Rand {

        #region Static Fields

        static List<Random> randoms = new List<Random>();

        #endregion

        #region Static Properties

        static Random random {
            get {
                if (randoms.Count == 0) {
                    randoms.Add(new Random());
                }
                return randoms[randoms.Count - 1];
            }
        }

        /// <summary>
        /// A raw random value.
        /// </summary>
        public static float Value {
            get { return (float)random.NextDouble(); }
        }

        /// <summary>
        /// A random float from 0 to 360.
        /// </summary>
        public static float Angle {
            get { return Float(360); }
        }

        /// <summary>
        /// Generate a random direction.
        /// </summary>
        public static Vector2 Direction {
            get { return Util.Normal(Angle); }
        }

        /// <summary>
        /// Generate a random bool.
        /// </summary>
        public static bool Bool {
            get { return random.Next(2) > 0; }
        }

        /// <summary>
        /// Generate a random Color.
        /// </summary>
        public static Color Color {
            get { return new Color(Float(1), Float(1), Float(1), 1); }
        }

        /// <summary>
        /// Generate a random Color with a random Alpha.
        /// </summary>
        public static Color ColorAlpha {
            get { return new Color(Float(1), Float(1), Float(1), Float(1)); }
        }

        /// <summary>
        /// Generate a random bool.
        /// </summary>
        public static bool Flip {
            get {
                return Bool;
            }
        }

        /// <summary>
        /// Generate a random sign.
        /// </summary>
        public static int Sign {
            get { return Bool ? 1 : -1; }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Push a random seed to use for all random number generation.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public static void PushSeed(int seed) {
            randoms.Add(new Random(seed));
        }

        /// <summary>
        /// Pop the top random seed.
        /// </summary>
        /// <returns>The random seed popped.</returns>
        public static Random PopSeed() {
            var r = random;
            randoms.RemoveAt(randoms.Count - 1);
            return r;
        }

        /// <summary>
        /// Generate a random int.
        /// </summary>
        /// <returns>A random int.</returns>
        public static int Int() {
            return random.Next();
        }

        /// <summary>
        /// Generate a random int.
        /// </summary>
        /// <param name="max">Maximum value.</param>
        /// <returns>A random int.</returns>
        public static int Int(int max) {
            return random.Next(max);
        }

        /// <summary>
        /// Generate a random int.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        /// <returns>A random int.</returns>
        public static int Int(int min, int max) {
            return random.Next(min, max);
        }

        /// <summary>
        /// Generate a random float.
        /// </summary>
        /// <returns>A random float.</returns>
        public static float Float() {
            return Value;
        }

        /// <summary>
        /// Generate a random float.
        /// </summary>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random float.</returns>
        public static float Float(float max) {
            return max * Value;
        }

        /// <summary>
        /// Generate a random float.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A random float.</returns>
        public static float Float(float min, float max) {
            return min + (max - min) * Value;
        }

        /// <summary>
        /// Generate a random float.
        /// </summary>
        /// <param name="range">A Range that will set the minimum and maximum.</param>
        /// <returns>A random float.</returns>
        public static float Float(Range range) {
            return range.Min + (range.Max - range.Min) * Value;
        }

        /// <summary>
        /// Generate a random point inside of a circle.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>A random Vector2 position inside the radius.</returns>
        public static Vector2 CircleXY(float radius) {
            return CircleXY(0, radius);
        }

        /// <summary>
        /// Generate a random point inside a circle.
        /// </summary>
        /// <param name="radiusMin">The minimum radius.</param>
        /// <param name="radiusMax">The maximum radius.</param>
        /// <returns>A random Vector2 position inside the radius.</returns>
        public static Vector2 CircleXY(float radiusMin, float radiusMax) {
            return CircleXY(radiusMin, radiusMax, 0, 360);
        }

        /// <summary>
        /// Generate a random point inside a circle.
        /// </summary>
        /// <param name="radiusMin">The minimum radius.</param>
        /// <param name="radiusMax">The maximum radius.</param>
        /// <param name="angleMin">The minimum angle.</param>
        /// <param name="angleMax">The maximum angle.</param>
        /// <returns>A random Vector2 position inside the radius and angle.</returns>
        public static Vector2 CircleXY(float radiusMin, float radiusMax, float angleMin, float angleMax) {
            var angle = Rand.Float(angleMin, angleMax);
            var radius = Rand.Float(radiusMin, radiusMax);
            return new Vector2(Util.PolarX(angle, radius), Util.PolarY(angle, radius));
        }

        /// <summary>
        /// Generate a random point in a minimum and maximum set.
        /// </summary>
        /// <param name="xMin">The minimum X value.</param>
        /// <param name="xMax">The maximum X value.</param>
        /// <param name="yMin">The minimum Y value.</param>
        /// <param name="yMax">The maximum Y value.</param>
        /// <returns>A random position inside the set.</returns>
        public static Vector2 XY(float xMin, float xMax, float yMin, float yMax) {
            return new Vector2(Float(xMin, xMax), Float(yMin, yMax));
        }

        /// <summary>
        /// Generate a random point in a Rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle the point will be in.</param>
        /// <returns>A random position inside the Rectangle.</returns>
        public static Vector2 XY(Rectangle rect) {
            return XY(rect.Left, rect.Right, rect.Top, rect.Bottom);
        }

        /// <summary>
        /// Generate a random point in a maximum set.
        /// </summary>
        /// <param name="xMax">The maximum X value.</param>
        /// <param name="yMax">The maximum Y value.</param>
        /// <returns>A random position from 0, 0 to the maximum values.</returns>
        public static Vector2 XY(float xMax, float yMax) {
            return new Vector2(Float(0, xMax), Float(0, yMax));
        }

        /// <summary>
        /// Generate a random integer point in a minimum and maximum set.
        /// </summary>
        /// <param name="xMin">The minimum X value.</param>
        /// <param name="xMax">The maximum X value.</param>
        /// <param name="yMin">The minimum Y value.</param>
        /// <param name="yMax">The maximum Y value.</param>
        /// <returns>A random integer position inside the set.</returns>
        public static Vector2 IntXY(int xMin, int xMax, int yMin, int yMax) {
            return new Vector2(Int(xMin, xMax), Int(yMin, yMax));
        }

        /// <summary>
        /// Generate a random integer point in a minimum and maximum set.
        /// </summary>
        /// <param name="xMax">The maximum X value.</param>
        /// <param name="yMax">The maximum Y value.</param>
        /// <returns>A random integer position inside the set.</returns>
        public static Vector2 IntXY(int xMax, int yMax) {
            return IntXY(0, xMax, 0, yMax);
        }

        /// <summary>
        /// Choose an element out of an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="choices">The array of possible choices.</param>
        /// <returns>The chosen object.</returns>
        public static T Choose<T>(params T[] choices) {
            return (T)choices[Int(choices.Length)];
        }

        /// <summary>
        /// Choose an element out of an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="choices">The array of possible choices.</param>
        /// <returns>The chosen object.</returns>
        public static T ChooseElement<T>(IEnumerable<T> choices) {
            return choices.ElementAt(Int(choices.Count()));
        }

        /// <summary>
        /// Choose a random character out of a string.
        /// </summary>
        /// <param name="str">The string to choose from.</param>
        /// <returns>The chosen character as a string.</returns>
        public static string Choose(string str) {
            return str.Substring(Int(str.Length), 1);
        }

        /// <summary>
        /// Choose a random element in a collection of objects, and remove the object from the collection.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="choices">The collection of possible choices.</param>
        /// <returns>The chosen element.</returns>
        public static T ChooseRemove<T>(ICollection<T> choices) {
            var choice = ChooseElement(choices);
            choices.Remove(choice);
            return choice;
        }

        /// <summary>
        /// Shuffle an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="list">The array to shuffle.</param>
        public static void Shuffle<T>(T[] list) {
            int i = list.Length;
            int j;
            T item;
            while (--i > 0) {
                item = list[i];
                list[i] = list[j = Int(i + 1)];
                list[j] = item;
            }
        }

        /// <summary>
        /// Shuffle a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(List<T> list) {
            int i = list.Count;
            int j;
            T item;
            while (--i > 0) {
                item = list[i];
                list[i] = list[j = Int(i + 1)];
                list[j] = item;
            }
        }

        /// <summary>
        /// A random percent chance from 0 to 100.
        /// </summary>
        /// <param name="percent">Percent from 0 to 100.</param>
        /// <returns>True if it succeeded.</returns>
        public static bool Chance(float percent) {
            return Value < percent * 0.01f;
        }

        /// <summary>
        /// Generate a random string.
        /// </summary>
        /// <param name="length">The length of the string to return.</param>
        /// <param name="charSet">The set of characters to pull from.</param>
        /// <returns>A string of randomly chosen characters.</returns>
        public static string String(int length, string charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
            var str = "";
            for (var i = 0; i < length; i++) {
                str += charSet[Int(charSet.Length)].ToString();
            }
            return str;
        }

        #endregion
        
    }
}
