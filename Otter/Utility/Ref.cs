//special thanks to chevy ray for this class <3

namespace Otter {
    /// <summary>
    /// Class of utility functions for ref related things.
    /// </summary>
    public static class Ref {

        #region Static Methods

        /// <summary>
        /// Swap two values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        public static void Swap<T>(ref T a, ref T b) {
            var temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Shift three values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="c">Third value.</param>
        public static void Shift<T>(ref T a, ref T b, ref T c) {
            var temp = a;
            a = b;
            b = c;
            c = temp;
        }

        /// <summary>
        /// Shift four values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="c">Third value.</param>
        /// <param name="d">Fourth value.</param>
        public static void Shift<T>(ref T a, ref T b, ref T c, ref T d) {
            var temp = a;
            a = b;
            b = c;
            c = d;
            d = temp;
        }

        /// <summary>
        /// Shift five values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="c">Third value.</param>
        /// <param name="d">Fourth value.</param>
        /// <param name="e">Fifth value.</param>
        public static void Shift<T>(ref T a, ref T b, ref T c, ref T d, ref T e) {
            var temp = a;
            a = b;
            b = c;
            c = d;
            d = e;
            e = temp;
        }

        /// <summary>
        /// Shift six values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="c">Third value.</param>
        /// <param name="d">Fourth value.</param>
        /// <param name="e">Fifth value.</param>
        /// <param name="f">Sixth value.</param>
        public static void Shift<T>(ref T a, ref T b, ref T c, ref T d, ref T e, ref T f) {
            var temp = a;
            a = b;
            b = c;
            c = d;
            d = e;
            e = f;
            f = temp;
        }

        /// <summary>
        /// Test if a value equals any value on a list.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="p">The value to check for.</param>
        /// <param name="values">The values to check.</param>
        /// <returns>True if any of the values equal the value to check for.</returns>
        public static bool EqualsAny<T>(ref T p, params T[] values) {
            foreach (var val in values)
                if (val.Equals(p))
                    return true;
            return false;
        }

        /// <summary>
        /// Test if a value equals any value on a list.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="p">The value to check for.</param>
        /// <param name="values">The values to check.</param>
        /// <returns>True if any of the values equal the value to check for.</returns>
        public static bool EqualsAll<T>(ref T p, params T[] values) {
            foreach (var val in values)
                if (!val.Equals(p))
                    return false;
            return true;
        }

        #endregion

    }
}
