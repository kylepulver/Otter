using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Otter {
    /// <summary>
    /// Main utility function class. Various useful functions for 2d game development and Otter stuff.
    /// </summary>
    public static class Util {

        #region Constants

        /// <summary>
        /// Right
        /// </summary>
        public const float RIGHT = 0;

        /// <summary>
        /// Up
        /// </summary>
        public const float UP = (float)Math.PI * -.5f;

        /// <summary>
        /// Left
        /// </summary>
        public const float LEFT = (float)Math.PI;

        /// <summary>
        /// Down
        /// </summary>
        public const float DOWN = (float)Math.PI * .5f;

        /// <summary>
        /// Up Right
        /// </summary>
        public const float UP_RIGHT = (float)Math.PI * -.25f;

        /// <summary>
        /// Up Left
        /// </summary>
        public const float UP_LEFT = (float)Math.PI * -.75f;

        /// <summary>
        /// Down Right
        /// </summary>
        public const float DOWN_RIGHT = (float)Math.PI * .25f;

        /// <summary>
        /// Down Left
        /// </summary>
        public const float DOWN_LEFT = (float)Math.PI * .75f;

        /// <summary>
        /// Degrees to Radians
        /// </summary>
        public const float DEG_TO_RAD = (float)Math.PI / 180f;

        /// <summary>
        /// Radians to Degrees
        /// </summary>
        public const float RAD_TO_DEG = 180f / (float)Math.PI;

        private const string HEX = "0123456789ABCDEF";

        #endregion

        #region Static Methods

        /// <summary>
        /// A shortcut function to send text to the debugger log.
        /// </summary>
        /// <param name="str">The string to send.</param>
        public static void Log(object str) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Log(str);
        }

        /// <summary>
        /// A shortcut function to send text to the debugger log.
        /// </summary>
        /// <param name="tag">The tag to log with.</param>
        /// <param name="str">The string to send.</param>
        public static void LogTag(string tag, object str) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Log(tag, str);
        }

        public static void LogTag(string tag, string str, params object[] obj) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Log(tag, string.Format(str, obj));
        }

        public static void Log(string str, params object[] obj) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Log("", string.Format(str, obj));
        }

        /// <summary>
        /// Summon the Debugger.
        /// </summary>
        public static void ShowDebugger() {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Summon();
        }

        /// <summary>
        /// A shortcut function to watch a value in the debugger. This must be called every update to
        /// keep the value updated (not an automatic watch.)
        /// </summary>
        /// <param name="str">The name of the value.</param>
        /// <param name="obj">The value.</param>
        public static void Watch(string str, object obj) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Watch(str, obj);
        }       

        /// <summary>
        /// Interpolate between two values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The progress of the interpolation.</param>
        /// <returns>The interpolated value.</returns>
        public static float Lerp(float a, float b, float t = 1) {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Interpolate between two values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="t">The progress of the interpolation.</param>
        /// <returns>The interpolated value.</returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t = 1) {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Interpolate through a set of numbers.
        /// </summary>
        /// <param name="amount">The amount of completion of the lerp. (0 - 1)</param>
        /// <param name="numbers">The numbers to interpolate through.</param>
        /// <returns>The interpolated number.</returns>
        public static float LerpSet(float amount, params float[] numbers) {
            if (amount <= 0) return numbers[0];
            if (amount >= 1) return numbers[numbers.Length - 1];

            int fromIndex = (int)Util.ScaleClamp(amount, 0, 1, 0, numbers.Length - 1);
            int toIndex = fromIndex + 1;

            float length = 1f / (numbers.Length - 1);
            float lerp = Util.ScaleClamp(amount % length, 0, length, 0, 1);

            // This is a fix for odd numbered color amounts. When fromIndex was
            // odd, lerp would evaluate to 1 when it should be 0.
            if (lerp >= 0.9999f && fromIndex % 2 == 1) {
                lerp = 0;
            }

            return Lerp(numbers[fromIndex], numbers[toIndex], lerp);
        }

        /// <summary>
        /// Interpolate through a looping set of numbers.
        /// </summary>
        /// <param name="amount">The amount of completion of the lerp. (0 - 1)</param>
        /// <param name="numbers">The numbers to interpolate through.</param>
        /// <returns>The interpolated number.</returns>
        public static float LerpSetLoop(float amount, params float[] numbers) {
            //convert numbers to looping set

            List<float> set = new List<float>();
            List<float> numberSet = new List<float>(numbers);
            numberSet.Add(numbers[0]);

            for (var i = 0; i < numberSet.Count; i++) {
                var current = numberSet[i];
                set.Add(current);
                
                if (i + 1 < numberSet.Count) {
                    var next = numberSet[i + 1];
                    var nextSet = (current + next) / 2f;
                    set.Add(nextSet);
                }
            }

            return LerpSet(amount, set.ToArray());
        }

        /// <summary>
        /// Interpolate from one Color to another.
        /// </summary>
        /// <param name="from">The start Color.</param>
        /// <param name="to">The end Color.</param>
        /// <param name="amount">The amount of completion on the lerp. (0 - 1)</param>
        /// <returns>The interpolated Color.</returns>
        public static Color LerpColor(Color from, Color to, float amount) {
            if (amount <= 0) return new Color(from);
            if (amount >= 1) return new Color(to);

            var c = new Color(from);
            c.R = from.R + (to.R - from.R) * amount;
            c.G = from.G + (to.G - from.G) * amount;
            c.B = from.B + (to.B - from.B) * amount;
            c.A = from.A + (to.A - from.A) * amount;

            return c;
        }

        /// <summary>
        /// Interpolate through a set of Colors.
        /// </summary>
        /// <param name="amount">The amount of completion on the lerp. (0 - 1)</param>
        /// <param name="colors">The Colors to interpolate through.</param>
        /// <returns>The interpolated Color.</returns>
        public static Color LerpColor(float amount, params Color[] colors) {
            if (amount <= 0) return colors[0];
            if (amount >= 1) return colors[colors.Length - 1];

            int fromIndex = (int)Util.ScaleClamp(amount, 0, 1, 0, colors.Length - 1);
            int toIndex = fromIndex + 1;

            float length = 1f / (colors.Length - 1);
            float lerp = Util.ScaleClamp(amount % length, 0, length, 0, 1);

            // This is a fix for odd numbered color amounts. When fromIndex was
            // odd, lerp would evaluate to 1 when it should be 0.
            if (lerp >= 0.9999f && fromIndex % 2 == 1) {
                lerp = 0;
            }

            return LerpColor(colors[fromIndex], colors[toIndex], lerp);
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">Min clamp.</param>
        /// <param name="max">Max clamp.</param>
        /// <returns>The new value between min and max.</returns>
        public static float Clamp(float value, float min, float max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">Min clamp.</param>
        /// <param name="max">Max clamp.</param>
        /// <returns>The new value between min and max.</returns>
        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) {
            return new Vector2(
                Util.Clamp(value.X, min.X, max.X),
                Util.Clamp(value.Y, min.Y, max.Y)
                );
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="max">Max clamp.</param>
        /// <returns>The new value between 0 and max.</returns>
        public static Vector2 Clamp(Vector2 value, Vector2 max) {
            return Clamp(value, Vector2.Zero, max);
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="max">Max clamp</param>
        /// <returns>The new value between 0 and max.</returns>
        public static float Clamp(float value, float max) {
            return Clamp(value, 0, max);
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="range">The range.</param>
        /// <returns>The clamped value in the range.</returns>
        public static float Clamp(float value, Range range) {
            return Clamp(value, range.Min, range.Max);
        }

        /// <summary>
        /// Steps an angle value toward a target based on a certain amount.
        /// </summary>
        /// <param name="from">The angle value to step.</param>
        /// <param name="to">The target value to approach.</param>
        /// <param name="amount">The amount to approach by.</param>
        /// <returns>The new angle value approaching the target from 0 to 360.</returns>
        public static float ApproachAngle(float from, float to, float amount) {
            var sign = AngleDifferenceSign(from, to);
            var diff = AngleDifference(from, to);
            var maxMove = Math.Min(Math.Abs(diff), amount);
            return (from + maxMove * sign);
        }

        /// <summary>
        /// Steps a value toward a target based on a certain amount.
        /// </summary>
        /// <param name="val">The value to step.</param>
        /// <param name="target">The target to approach.</param>
        /// <param name="maxMove">The maximum increment toward the target.</param>
        /// <returns>The new value approaching the target.</returns>
        static public float Approach(float val, float target, float maxMove) {
            return val > target ? Math.Max(val - maxMove, target) : Math.Min(val + maxMove, target);
        }

        /// <summary>
        /// Steps a value toward a target based on a certain amount.
        /// </summary>
        /// <param name="val">The value to step.</param>
        /// <param name="target">The target to approach.</param>
        /// <param name="maxMove">The maximum increment toward the target.</param>
        /// <returns>The new value approaching the target.</returns>
        static public Vector2 Approach(Vector2 val, Vector2 target, Vector2 maxMove) {
            return new Vector2(
                Approach(val.X, target.X, maxMove.X),
                Approach(val.Y, target.Y, maxMove.Y)
                );
        }

        static public Vector2 Approach(Vector2 val, Vector2 target, float maxMove) {
            return Approach(val, target, new Vector2(maxMove, maxMove));
        }

        /// <summary>
        /// Snaps a value to the nearest value on a grid.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="increment">The size of each grid space.</param>
        /// <param name="offset">The offset to apply after the snap.</param>
        /// <returns>The snapped value.</returns>
        static public float SnapToGrid(float value, float increment, float offset = 0) {
            return ((float)Math.Floor(value / increment) * increment) + offset;
        }

        /// <summary>
        /// Converts a hex character to a byte.
        /// </summary>
        /// <param name="c">The input character.</param>
        /// <returns>The byte.</returns>
        static public byte HexToByte(char c) {
            return (byte)HEX.IndexOf(char.ToUpper(c));
        }

        /// <summary>
        /// Get the minimum value from a set of values.
        /// </summary>
        /// <param name="values">The values to test.</param>
        /// <returns>The minimum value.</returns>
        static public float Min(params float[] values) {
            float min = values[0];
            for (int i = 1; i < values.Length; i++)
                min = Math.Min(values[i], min);
            return min;
        }

        /// <summary>
        /// Get the maximum value from a set of values.
        /// </summary>
        /// <param name="values">The values to test.</param>
        /// <returns>The maximum value.</returns>
        static public float Max(params float[] values) {
            float max = values[0];
            for (int i = 1; i < values.Length; i++)
                max = Math.Max(values[i], max);
            return max;
        }

        /// <summary>
        /// Convert a number in a range of min, max to a number in the range min2, max2.
        /// Also known as RemapRange in some frameworks.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="min">The original minimum.</param>
        /// <param name="max">The original maximum.</param>
        /// <param name="min2">The new minimum.</param>
        /// <param name="max2">The new maximum.</param>
        /// <returns>A value scaled from the original min and max to the new min and max.</returns>
        public static float Scale(float value, float min, float max, float min2, float max2) {
            return min2 + ((value - min) / (max - min)) * (max2 - min2);
        }

        /// <summary>
        /// Convert a number in range of min, max to a number in the range min2, max2, but also
        /// clamp the result inside min2 and max2.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="min">The original minimum.</param>
        /// <param name="max">The original maximum.</param>
        /// <param name="min2">The new minimum.</param>
        /// <param name="max2">The new maximum.</param>
        /// <returns>A value scaled from the original min and max to the new min and max, and clamped to the new min and max.</returns>
        public static float ScaleClamp(float value, float min, float max, float min2, float max2) {
            value = min2 + ((value - min) / (max - min)) * (max2 - min2);
            if (max2 > min2) {
                value = value < max2 ? value : max2;
                return value > min2 ? value : min2;
            }
            value = value < min2 ? value : min2;
            return value > max2 ? value : max2;
        }

        /// <summary>
        /// Shortcut to Scale the value from a sine wave.  Original min and max are -1 and 1.
        /// </summary>
        /// <param name="value">The input value to sine.</param>
        /// <param name="min">The new minimum.</param>
        /// <param name="max">The new maximum.</param>
        /// <returns>A value scaled from -1 and 1 to the new min and max.</returns>
        public static float SinScale(float value, float min, float max) {
            return Scale((float)Math.Sin(value * DEG_TO_RAD), -1f, 1f, min, max);
        }

        /// <summary>
        /// Shortcut to ScaleClamp the value from a sine wave.  Original min and max are -1 and 1.
        /// </summary>
        /// <param name="value">The input value to sine.</param>
        /// <param name="min">The new minimum.</param>
        /// <param name="max">The new maximum.</param>
        /// <returns>A value scaled from -1 and 1 to the new min and max, and clamped to the new min and max.</returns>
        public static float SinScaleClamp(float value, float min, float max) {
            return ScaleClamp((float)Math.Sin(value * DEG_TO_RAD), -1f, 1f, min, max);
        }

        /// <summary>
        /// Round down.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value rounded down.</returns>
        public static float Floor(float value) {
            return (float)Math.Floor(value);
        }

        /// <summary>
        /// Round up.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value rounded up.</returns>
        public static float Ceil(float value) {
            return (float)Math.Ceiling(value);
        }

        /// <summary>
        /// Round.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value rounded.</returns>
        public static float Round(float value) {
            return (float)Math.Round(value);
        }

        /// <summary>
        /// The angle of a x and y coordinate.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <returns>An angle between 0 - 360 degrees.</returns>
        public static float Angle(float x, float y) {
            //y is negative since y is DOWN in video games
            //return degrees by default
            return (float)Math.Atan2(-y, x) * Util.RAD_TO_DEG;
        }

        /// <summary>
        /// Get the angle between two Entities.
        /// </summary>
        /// <param name="e1">The first Entity.</param>
        /// <param name="e2">The second Entity.</param>
        /// <returns>The angle between the two Entity's positions between 0 - 360 degrees.</returns>
        public static float Angle(Entity e1, Entity e2) {
            return Angle(e1.X, e1.Y, e2.X, e2.Y);
        }

        /// <summary>
        /// The angle of a vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>An angle between 0 - 360 degrees.</returns>
        public static float Angle(Vector2 vector) {
            return Angle((float)vector.X, (float)vector.Y);
        }

        /// <summary>
        /// Get the angle between two vectors.
        /// </summary>
        /// <param name="from">The first vector.</param>
        /// <param name="to">The second vector.</param>
        /// <returns>The angle between the two vectors.</returns>
        public static float Angle(Vector2 from, Vector2 to) {
            return Angle((float)(to.X - from.X), (float)(to.Y - from.Y));
        }

        /// <summary>
        /// Get the angle between two positions.
        /// </summary>
        /// <param name="x1">The first X position.</param>
        /// <param name="y1">The first Y position.</param>
        /// <param name="x2">The second X position.</param>
        /// <param name="y2">The second Y position.</param>
        /// <returns>The angle between the two positions.</returns>
        public static float Angle(float x1, float y1, float x2, float y2) {
            return Angle(x2 - x1, y2 - y1);
        }

        /// <summary>
        /// Get the difference between two angles from -180 to 180.
        /// </summary>
        /// <param name="a">The first angle.</param>
        /// <param name="b">The second angle.</param>
        /// <returns>The difference between the angles from -180 to 180.</returns>
        public static float AngleDifference(float a, float b) {
            var diff = b - a;

            while (diff > 180) { diff -= 360; }
            while (diff <= -180) { diff += 360; }

            return diff;
        }

        /// <summary>
        /// Get the shortest direction from angle a to angle b.
        /// </summary>
        /// <param name="a">The first angle.</param>
        /// <param name="b">The second angle..</param>
        /// <returns>1 for clockwise, -1 for counter clockwise, 0 if angles are the same.</returns>
        public static int AngleDifferenceSign(float a, float b) {
            if (a == b) return 0;
            var dif = AngleDifference(a, b);
            return (int)Math.Sign(dif);
        }

        /// <summary>
        /// Rotate a position by an angle.
        /// </summary>
        /// <param name="vector">The position to rotate.</param>
        /// <param name="amount">The amount to rotate the position in degrees.</param>
        /// <returns>The new rotated position.</returns>
        public static Vector2 Rotate(Vector2 vector, float amount) {
            return Rotate(vector.X, vector.Y, amount);
        }

        /// <summary>
        /// Rotate a position by an angle.
        /// </summary>
        /// <param name="x">The X position to rotate.</param>
        /// <param name="y">The Y position to rotate.</param>
        /// <param name="amount">The amount to rotate the position in degrees.</param>
        /// <returns>The new rotated position.</returns>
        public static Vector2 Rotate(float x, float y, float amount) {
            amount *= -1;  // Flip Y because of video game coordinates.
            return new Vector2(x * Cos(amount) - y * Sin(amount), x * Sin(amount) + y * Cos(amount)); // Wow this is fancy
            /*
            // old rotate code just in case I broke something
            var v = new Vector2(x, y);
            var length = v.Length;
            var angle = Angle(x, y) + amount;
            v.X = PolarX(angle, length);
            v.Y = PolarY(angle, length);
            return v;
             */
        }

        /// <summary>
        /// Rotate a position by an angle around an anchor point.
        /// </summary>
        /// <param name="x">The X position to rotate.</param>
        /// <param name="y">The Y position to rotate.</param>
        /// <param name="aroundX">The X position to rotate around.</param>
        /// <param name="aroundY">The Y position to rotate around.</param>
        /// <param name="amount">The amount to rotate the position in degrees.</param>
        /// <returns>The new rotated position.</returns>
        public static Vector2 RotateAround(float x, float y, float aroundX, float aroundY, float amount) {
            var vec = Rotate(x - aroundX, y - aroundY, amount);
            vec.X += aroundX;
            vec.Y += aroundY;
            return vec;
        }

        /// <summary>
        /// Rotate a position by an angle around an anchor point.
        /// </summary>
        /// <param name="point">The position to rotate.</param>
        /// <param name="around">The position to rotate around.</param>
        /// <param name="amount">The amount to rotate the position in degrees.</param>
        /// <returns>The new rotated position.</returns>
        public static Vector2 RotateAround(Vector2 point, Vector2 around, float amount) {
            return RotateAround(point.X, point.Y, around.X, around.Y, amount);
        }

        /// <summary>
        /// Scale a position by an amount around an anchor point.
        /// </summary>
        /// <param name="x">The X position to scale.</param>
        /// <param name="y">The Y position to scale.</param>
        /// <param name="aroundX">The X position to scale around.</param>
        /// <param name="aroundY">The Y position to scale around.</param>
        /// <param name="amountX">The X amount to scale by.</param>
        /// <param name="amountY">The Y amount to scale by.</param>
        /// <returns>The new scaled position.</returns>
        public static Vector2 ScaleAround(float x, float y, float aroundX, float aroundY, float amountX, float amountY) {
            x -= aroundX;
            y -= aroundY;

            x *= amountX;
            y *= amountY;

            x += aroundX;
            y += aroundY;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Scale a position by an amount around an anchor point.
        /// </summary>
        /// <param name="point">The position to scale.</param>
        /// <param name="around">The position to scale around.</param>
        /// <param name="amountX">The X amount to scale by.</param>
        /// <param name="amountY">The Y amount to scale by.</param>
        /// <returns>The new scaled position.</returns>
        public static Vector2 ScaleAround(Vector2 point, Vector2 around, float amountX, float amountY) {
            return ScaleAround(point.X, point.Y, around.X, around.Y, amountX, amountY);
        }

        /// <summary>
        /// Scale a position by an amount around an anchor point.
        /// </summary>
        /// <param name="point">The position to scale.</param>
        /// <param name="around">The position to scale around.</param>
        /// <param name="amount">The amount to scale by.</param>
        /// <returns>The new scaled position.</returns>
        public static Vector2 ScaleAround(Vector2 point, Vector2 around, float amount) {
            return ScaleAround(point, around, amount, amount);
        }

        /// <summary>
        /// Scale a position by an amount around an anchor point.
        /// </summary>
        /// <param name="point">The position to scale.</param>
        /// <param name="around">The position to scale around.</param>
        /// <param name="amount">The amount to scale by.</param>
        /// <returns>The new scaled position.</returns>
        public static Vector2 ScaleAround(Vector2 point, Vector2 around, Vector2 amount) {
            return ScaleAround(point.X, point.Y, around.X, around.Y, amount.X, amount.Y);
        }

        /// <summary>
        /// Distance check.
        /// </summary>
        /// <param name="x1">The first X position.</param>
        /// <param name="y1">The first Y position.</param>
        /// <param name="x2">The second X position.</param>
        /// <param name="y2">The second Y position.</param>
        /// <returns>The distance between the two points.</returns>
        public static float Distance(float x1, float y1, float x2, float y2) {
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        /// <summary>
        /// Distance check.
        /// </summary>
        /// <param name="from">The first position.</param>
        /// <param name="to">The second position.</param>
        /// <returns>The distance between the two positions.</returns>
        public static float Distance(Vector2 from, Vector2 to) {
            return Distance(from.X, from.Y, to.X, to.Y);
        }

        /// <summary>
        /// Distance check.
        /// </summary>
        /// <param name="e1">The first Entity.</param>
        /// <param name="e2">The second Entity.</param>
        /// <returns>The distance between the Entities.</returns>
        public static float Distance(Entity e1, Entity e2) {
            return Distance(e1.X, e1.Y, e2.X, e2.Y);
        }

        /// <summary>
        /// Check for a point in a rectangle defined by min and max points.
        /// </summary>
        /// <param name="p">The point to check.</param>
        /// <param name="min">The top left corner of the rectangle.</param>
        /// <param name="max">The bottom right corner of the rectangle.</param>
        /// <returns>True if the point is in the rectangle.</returns>
        public static bool InRect(Vector2 p, Vector2 min, Vector2 max) {
            return p.X > min.X && p.Y > min.Y && p.X < max.X && p.Y < max.Y;
        }

        /// <summary>
        /// Check for a point in a rectangle.
        /// </summary>
        /// <param name="x">The X position of the point to check.</param>
        /// <param name="y">The Y position of the point to check.</param>
        /// <param name="rect">The rectangle.</param>
        /// <returns>True if the point is in the rectangle.</returns>
        public static bool InRect(float x, float y, Rectangle rect) {
            if (x <= rect.X) return false;
            if (x >= rect.X + rect.Width) return false;
            if (y <= rect.Y) return false;
            if (y >= rect.Y + rect.Height) return false;
            return true;
        }

        /// <summary>
        /// Check for a point in a rectangle.
        /// </summary>
        /// <param name="x">The X position of the point to check.</param>
        /// <param name="y">The Y position of the point to check.</param>
        /// <param name="rx">The left of the rectangle.</param>
        /// <param name="ry">The top of the rectangle.</param>
        /// <param name="rw">The width of the rectangle.</param>
        /// <param name="rh">The height of the rectangle.</param>
        /// <returns>True if the point is in the rectangle.</returns>
        public static bool InRect(float x, float y, float rx, float ry, float rw, float rh) {
            if (x <= rx) return false;
            if (x >= rx + rw) return false;
            if (y <= ry) return false;
            if (y >= ry + rh) return false;
            return true;
        }

        /// <summary>
        /// Check for a point in a rectangle.
        /// </summary>
        /// <param name="xy">The X and Y position of the point to check.</param>
        /// <param name="rect">The rectangle.</param>
        /// <returns>True if the point is in the rectangle.</returns>
        public static bool InRect(Vector2 xy, Rectangle rect) {
            return InRect((float)xy.X, (float)xy.Y, rect);
        }

        /// <summary>
        /// Check for a point inside of a circle.
        /// </summary>
        /// <param name="p">The point to check.</param>
        /// <param name="circleP">The center point of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>True if the point is inside the circle.</returns>
        public static bool InCircle(Vector2 p, Vector2 circleP, float radius) {
            return Distance((float)p.X, (float)p.Y, (float)circleP.X, (float)circleP.Y) <= radius;
        }

        /// <summary>
        /// Check for a point inside of a circle.
        /// </summary>
        /// <param name="x">The X position to check.</param>
        /// <param name="y">The Y position to check.</param>
        /// <param name="circleX">The center X position of the circle.</param>
        /// <param name="circleY">The center Y position of the check.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>True if the point is inside the circle.</returns>
        public static bool InCircle(float x, float y, float circleX, float circleY, float radius) {
            return Distance(x, y, circleX, circleY) <= radius;
        }

        /// <summary>
        /// Check an intersection between two rectangles.
        /// </summary>
        /// <param name="x1">The left of the first rectangle.</param>
        /// <param name="y1">The top of the first rectangle.</param>
        /// <param name="w1">The width of the first rectangle.</param>
        /// <param name="h1">The height of the first rectangle.</param>
        /// <param name="x2">The left of the second rectangle.</param>
        /// <param name="y2">The top of the second rectangle.</param>
        /// <param name="w2">The width of the second rectangle.</param>
        /// <param name="h2">The height of the second rectangle.</param>
        /// <returns>True if the rectangles intersect.</returns>
        public static bool IntersectRectangles(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2) {
            if (x1 + w1 <= x2) return false;
            if (x1 >= x2 + w2) return false;
            if (y1 + h1 <= y2) return false;
            if (y1 >= y2 + h2) return false;
            return true;
        }

        /// <summary>
        /// The X component of a vector represented by an angle and radius.
        /// </summary>
        /// <param name="angle">The angle of the vector.</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>The X component.</returns>
        public static float PolarX(float angle, float length) {
            return Cos(angle) * length;
        }

        /// <summary>
        /// The Y component of a vector represented by an angle and radius.
        /// </summary>
        /// <param name="angle">The angle of the vector.</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>The Y component.</returns>
        public static float PolarY(float angle, float length) {
            //Radius is negative since Y positive is DOWN in video games.
            return Sin(angle) * -length;
        }

        /// <summary>
        /// Wrapper for the sin function that uses degrees.
        /// </summary>
        /// <param name="degrees">The angle.</param>
        /// <returns>The sine of the angle.</returns>
        public static float Sin(float degrees) {
            return (float)Math.Sin(degrees * DEG_TO_RAD);
        }

        /// <summary>
        /// Wrapper for the cos function that uses degrees.
        /// </summary>
        /// <param name="degrees">The angle.</param>
        /// <returns>The cosine of the angle.</returns>
        public static float Cos(float degrees) {
            return (float)Math.Cos(degrees * DEG_TO_RAD);
        }

        /// <summary>
        /// Convert a two dimensional position to a one dimensional index.
        /// </summary>
        /// <param name="width">The width of the two dimensional set.</param>
        /// <param name="x">The X position in the two dimensional set.</param>
        /// <param name="y">The Y position in the two dimensional set.</param>
        /// <returns>The one dimensional index in a two dimensional set.</returns>
        public static int OneDee(int width, int x, int y) {
            return y * width + x;
        }

        /// <summary>
        /// Convert a one dimensional index to a two dimensional position.
        /// </summary>
        /// <param name="index">The one dimensional index in the two dimensional set.</param>
        /// <param name="width">The width of the two dimensional set.</param>
        /// <returns>The X and Y position in the two dimensional set.</returns>
        public static Vector2 TwoDee(int index, int width) {
            if (width == 0) {
                return new Vector2(index, 0);
            }
            return new Vector2(index % width, index / width);
        }

        /// <summary>
        /// The X position from converting an index to a two dimensional position.
        /// </summary>
        /// <param name="index">The one dimensional index in the two dimensional set.</param>
        /// <param name="width">The width of the two dimensional set.</param>
        /// <returns>The X position in the two dimensional set.</returns>
        public static int TwoDeeX(int index, int width) {
            return (int)TwoDee(index, width).X;
        }

        /// <summary>
        /// The Y position from converting an index to a two dimensional position.
        /// </summary>
        /// <param name="index">The one dimensional index in the two dimensional set.</param>
        /// <param name="width">The width of the two dimensional set.</param>
        /// <returns>The Y position in the two dimensional set.</returns>
        public static int TwoDeeY(int index, int width) {
            return (int)TwoDee(index, width).Y;
        }

        /// <summary>
        /// Normal vector.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The normal vector of that angle.</returns>
        public static Vector2 Normal(float angle) {
            return new Vector2(Cos(angle), Sin(angle));
        }

        /// <summary>
        /// Checks if an object contains a method.
        /// </summary>
        /// <param name="objectToCheck">The object to check for the method on.</param>
        /// <param name="methodName">The name of the method to check for.</param>
        /// <returns>True if the object has that method.</returns>
        public static bool HasMethod(object objectToCheck, string methodName) {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName) != null || type.BaseType.GetMethod(methodName) != null;
        }

        /// <summary>
        /// Checks if an object contains a property.
        /// </summary>
        /// <param name="objectToCheck">The object to check for the property on.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>True if the object has that property.</returns>
        public static bool HasProperty(object objectToCheck, string propertyName) {
            var type = objectToCheck.GetType();
            return type.GetProperty(propertyName) != null || type.BaseType.GetProperty(propertyName) != null;
        }

        /// <summary>
        /// Get the value of a property from an object.
        /// </summary>
        /// <param name="source">The object to get the property from.</param>
        /// <param name="propName">The name of the property.</param>
        /// <returns>The property value.</returns>
        public static object GetPropValue(object source, string propName) {
            return source.GetType().GetProperty(propName).GetValue(source, null);
        }

        /// <summary>
        /// Checks to see if an object has a field by name.
        /// </summary>
        /// <param name="objectToCheck">The object to check for the field on.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>True if the object has that property.</returns>
        public static bool HasField(object objectToCheck, string fieldName) {
            return objectToCheck.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public) != null;
        }

        /// <summary>
        /// Get the value of a field by name from an object.
        /// </summary>
        /// <param name="source">The object to get the field from.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>The value of the field.</returns>
        public static object GetFieldValue(object source, string fieldName) {
            return source.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).GetValue(source);
        }

        /// <summary>
        /// Get the static value of a field by name from an object.
        /// </summary>
        /// <param name="type">The type to look for the field in.</param>
        /// <param name="fieldName">The name of the static field.</param>
        /// <returns>The value of the static field.</returns>
        public static object GetFieldValue(Type type, string fieldName) {
            return type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        }

        /// <summary>
        /// Get the value of a field by name from an object with a default return.
        /// </summary>
        /// <param name="source">The object to get the field from.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="returnOnNull">The value to return if the field is not found.</param>
        /// <returns>The field value or the value to return if the field is not found.</returns>
        public static object GetFieldValue(object src, string fieldName, object returnOnNull) {
            if (src == null) return returnOnNull;
            if (!Util.HasField(src, fieldName)) return returnOnNull;
            return GetFieldValue(src, fieldName);
        }

        /// <summary>
        /// Set the value of a field on an object by name.
        /// </summary>
        /// <param name="src">The object to set the field on.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The new value of the field.</param>
        public static void SetFieldValue(object src, string fieldName, object value) {
            var fi = src.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            fi.SetValue(src, value);
        }
        
        /// <summary>
        /// Checks if a value is in a specified range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>True if the value is in the range.</returns>
        public static bool InRange(float value, float min, float max) {
            if (value >= min && value <= max) return true;
            return false;
        }

        /// <summary>
        /// Returns a value according to a value's position in a range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>0 if the value is in the set.  -1 if the value is below the minimum.  1 if the value is above the maximum.</returns>
        public static int Subset(float value, float min, float max) {
            if (value < min) return -1;
            if (value > max) return 1;
            return 0;
        }

        /// <summary>
        /// Wrap and angle and keep it within the range of 0 to 360.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The angle wrapped to be in the range of 0 to 360.</returns>
        public static float WrapAngle(float angle) {
            angle %= 360;
            if (angle > 180)
                return angle - 360;
            else if (angle <= -180)
                return angle + 360;
            else
                return angle;
        }

        /// <summary>
        /// Find the distance between a point and a rectangle.
        /// </summary>
        /// <param name="px">The X position of the point.</param>
        /// <param name="py">The Y position of the point.</param>
        /// <param name="rx">The X position of the rectangle.</param>
        /// <param name="ry">The Y position of the rectangle.</param>
        /// <param name="rw">The width of the rectangle.</param>
        /// <param name="rh">The height of the rectangle.</param>
        /// <returns>The distance.  Returns 0 if the point is within the rectangle.</returns>
        public static float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh) {
            if (px >= rx && px <= rx + rw) {
                if (py >= ry && py <= ry + rh) return 0;
                if (py > ry) return py - (ry + rh);
                return ry - py;
            }
            if (py >= ry && py <= ry + rh) {
                if (px > rx) return px - (rx + rw);
                return rx - px;
            }
            if (px > rx) {
                if (py > ry) return Distance(px, py, rx + rw, ry + rh);
                return Distance(px, py, rx + rw, ry);
            }
            if (py > ry) return Distance(px, py, rx, ry + rh);
            return Distance(px, py, rx, ry);
        }

        /// <summary>
        /// Find the distance between a point and a rectangle.
        /// </summary>
        /// <param name="px">The X position of the point.</param>
        /// <param name="py">The Y position of the point.</param>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The distance.  Returns 0 if the point is within the rectangle.</returns>
        public static float DistanceRectPoint(float px, float py, Rectangle rect) {
            return DistanceRectPoint(px, py, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Convert XML attributes to a dictionary.
        /// </summary>
        /// <param name="attributes">The attributes to convert.</param>
        /// <returns>A dictionary of string, string with the attribute names and values.</returns>
        public static Dictionary<string, string> XmlAttributesToDictionary(XmlAttributeCollection attributes) {
            var d = new Dictionary<string, string>();
            foreach (XmlAttribute attr in attributes) {
                d.Add(attr.Name, attr.Value);
            }
            return d;
        }

        /// <summary>
        /// Searches all known assemblies for a type and returns that type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>The type found.  Null if no match.</returns>
        [Obsolete("Use GetTypesFromAllAssemblies instead")]
        public static Type GetTypeFromAllAssemblies(string type, bool ignoreCase = false) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var t in types) {
                    if (t.Name == type) {
                        return t;
                    }
                    if (ignoreCase) {
                        if (t.Name.ToLower() == type) {
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Searches all known assemblies for a type and returns all types that match that type name.
        /// </summary>
        /// <param name="type">The name of the type to search for.</param>
        /// <param name="ignoreCase">If true, types will be matched regardless of case.</param>
        /// <returns>The list of types found.</returns>
        public static IEnumerable<Type> GetTypesFromAllAssemblies(string type, bool ignoreCase = false)
        {
            return GetTypesFromAllAssemblies<object>(type, ignoreCase);
        }

        /// <summary>
        /// Searches all known assemblies for a type and returns all types that match that type name.
        /// </summary>
        /// <typeparam name="TBaseType">The base class that the type must derrive from.</typeparam>
        /// <param name="type">The name of the type to search for.</param>
        /// <param name="ignoreCase">If true, types will be matched regardless of case.</param>
        /// <returns>The list of types found.</returns>
        public static IEnumerable<Type> GetTypesFromAllAssemblies<TBaseType>(string type, bool ignoreCase = false)
        {
            var comparisonType = ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly=>assembly.GetTypes())
                .Where(t=>t.Name.Equals(type, comparisonType))
                .Where(t=>typeof(TBaseType).IsAssignableFrom(t));
        }

        /// <summary>
        /// Get the list of all types in all known assemblies.
        /// </summary>
        /// <returns>The list of all types in all known assemblies.</returns>
        public static List<Type> GetTypesFromAllAssemblies() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allTypes = new List<Type>();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var t in types) {
                    allTypes.Add(t);
                }
            }
            return allTypes;
        }

        /// <summary>
        /// Get the basic type name of an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The basic type name of the object.</returns>
        public static string GetBasicTypeName(object obj) {
            var strarr = obj.GetType().ToString().Split('.');
            return strarr[strarr.Length - 1];
        }

        /// <summary>
        /// Compresses a string and base64 encodes it.  Use "DecompressString" to restore it.
        /// </summary>
        /// <param name="str">The string to compress.</param>
        /// <returns>The compressed string.</returns>
        public static string CompressString(string str) {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) {
                using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
                    CopyStream(msi, gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        /// <summary>
        /// Copies a stream from source to destination.
        /// </summary>
        /// <param name="src">The string to copy.</param>
        /// <param name="dest">The stream to copy the string to.</param>
        public static void CopyStream(Stream src, Stream dest) {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Decompresses a string compressed with "CompressString"
        /// </summary>
        /// <param name="base64str">The compressed string.</param>
        /// <returns>The uncompressed string.</returns>
        public static string DecompressString(string base64str) {
            byte[] bytes;
            try {
                bytes = Convert.FromBase64String(base64str);
            }
            catch {
                return null;
            }

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
                    CopyStream(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// Compresses a byte array using GZip
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <returns>The compressed byte array</returns>
        public static byte[] CompressBytes(byte[] data) {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionLevel.Optimal)) {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        /// <summary>
        /// Decompresses a byte array using GZip
        /// </summary>
        /// <param name="data">The data to decompress</param>
        /// <returns>The decompressed byte array</returns>
        public static byte[] DecompressBytes(byte[] data) {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream()) {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        /// <summary>
        /// Converts a dictionary of string, string into a string of data.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="keydelim">The string that separates keys.</param>
        /// <param name="valuedelim">The string that separates values.</param>
        /// <returns>A string with all of the dictionary's data.</returns>
        public static string DictionaryToString(Dictionary<string, string> dictionary, string keydelim, string valuedelim) {
            string str = "";

            foreach (var s in dictionary) {
                str += s.Key + keydelim + s.Value + valuedelim;
            }

            str = str.Substring(0, str.Length - valuedelim.Length);

            return str;
        }

        /// <summary>
        /// Convert a string into a dictionary of string, string.
        /// </summary>
        /// <param name="source">The string to convert into a dictionary.</param>
        /// <param name="keydelim">The string that separates keys.</param>
        /// <param name="valuedelim">The string that separates values.</param>
        /// <returns>A dictionary with all the string's data.</returns>
        public static Dictionary<string, string> StringToDictionary(string source, string keydelim, string valuedelim) {
            var d = new Dictionary<string, string>();

            string[] split = Regex.Split(source, valuedelim);

            foreach (var s in split) {
                string[] entry = Regex.Split(s, keydelim);
                d.Add(entry[0], entry[1]);
            }

            return d;
        }

        /// <summary>
        /// Calculate an MD5 hash of a string.
        /// </summary>
        /// <param name="input">The string to calculate the has for.</param>
        /// <returns>The MD5 hash of the string.</returns>
        public static string MD5Hash(string input) {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get all the values of an enum without a giant mess of code.
        /// </summary>
        /// <typeparam name="T">The type of the Enum.</typeparam>
        /// <returns>An enumerable containing all the enum values.</returns>
        public static IEnumerable<T> EnumValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Convert a string number to a float. If the string contains a % char it will be parsed as a percentage.
        /// For example "50%" => 0.5f, or 50 => 50f.
        /// </summary>
        /// <param name="percent">The string to parse.</param>
        /// <returns>If the string contained % a float of the percent on the scale of 0 to 1. Otherwise the float.</returns>
        public static float ParsePercent(string percent) {
            if (percent.Contains('%')) {
                percent = percent.TrimEnd('%');
                return float.Parse(percent) * 0.01f;
            }
            return float.Parse(percent);
        }

        /// <summary>
        /// Download data from a URL. This method will stall the thread. Probably
        /// should use something like a BackgroundWorker or something but I haven't
        /// figured that out yet.
        /// </summary>
        /// <param name="url">The url to download.</param>
        /// <returns>The string downloaded from the url.</returns>
        public static string GetURLString(string url) {
            string s = "";
            using (WebClient client = new WebClient()) {
                s = client.DownloadString(url);
            }
            return s;
        }

        /// <summary>
        /// Parse an enum value from a string.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="str">The string to parse.</param>
        /// <returns>The enum value.</returns>
        public static T ParseEnum<T>(string str) {
            return (T)Enum.Parse(typeof(T), str);
        }

        /// <summary>
        /// Convert a generic enum's value to a string.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The string of the enum value.</returns>
        public static string EnumValueToString(Enum value) {
            if (enumStringCache.ContainsKey(value)) return enumStringCache[value];
            var str = string.Format("{0}.{1}", value.GetType(), value);
            enumStringCache.Add(value, str);
            return enumStringCache[value];
        }
        static Dictionary<Enum, string> enumStringCache = new Dictionary<Enum, string>();

        /// <summary>
        /// Convert a generic enum's value into a string of just its value. (No type included!)
        /// </summary>
        /// <param name="e">The enum value.</param>
        /// <returns>The value of the enum following the final period.</returns>
        public static string EnumValueToBasicString(Enum e) {
            var split = Util.EnumValueToString(e).Split('.');
            return split[split.Length - 1];
        }

        /// <summary>
        /// Convert an array of generic enums to ints.
        /// </summary>
        /// <param name="enums">The enums to convert.</param>
        /// <returns>An array of ints.</returns>
        public static int[] EnumToIntArray(params Enum[] enums) {
            int[] ints = new int[enums.Length];
            for (var i = 0; i < enums.Length; i++) {
                ints[i] = Convert.ToInt32(enums[i]);
            }
            return ints;
        }

        public static int[] EnumToIntArray(List<Enum> enums) {
            int[] ints = new int[enums.Count];
            for (var i = 0; i < enums.Count; i++) {
                ints[i] = Convert.ToInt32(enums[i]);
            }
            return ints;
        }

        /// <summary>
        /// Distance between a line and a point.
        /// </summary>
        /// <param name="x">The X position of the point.</param>
        /// <param name="y">The Y position of the point.</param>
        /// <param name="line">The line.</param>
        /// <returns>The distance from the point to the line.</returns>
        public static float DistanceLinePoint(float x, float y, Line2 line) {
            return (DistanceLinePoint(x, y, line.X1, line.Y1, line.X2, line.Y2));
        }

        /// <summary>
        /// Distance between a line and a point.
        /// </summary>
        /// <param name="x">The X position of the point.</param>
        /// <param name="y">The Y position of the point.</param>
        /// <param name="x1">The first X position of the line.</param>
        /// <param name="y1">The first Y position of the line.</param>
        /// <param name="x2">The second X position of the line.</param>
        /// <param name="y2">The second Y position of the line.</param>
        /// <returns>The distance from the point to the line.</returns>
        public static float DistanceLinePoint(float x, float y, float x1, float y1, float x2, float y2) {
            if (x1 == x2 && y1 == y2) return Util.Distance(x, y, x1, y1);

            var px = x2 - x1;
            var py = y2 - y1;

            float something = px * px + py * py;

            var u = ((x - x1) * px + (y - y1) * py) / something;

            if (u > 1) u = 1;
            if (u < 0) u = 0;

            var xx = x1 + u * px;
            var yy = y1 + u * py;

            var dx = xx - x;
            var dy = yy - y;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Gets every possible combination of a List.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="list">A list of objects.</param>
        /// <returns>An IEnumerable of every possible combination in the List.</returns>
        public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list) {
            return from m in Enumerable.Range(0, 1 << list.Count)
                   select
                       from i in Enumerable.Range(0, list.Count)
                       where (m & (1 << i)) != 0
                       select list[i];
        }

        /// <summary>
        /// Shortcut to serialize an object to a file.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="path">The file to write to.</param>
        public static void SerializeToFile<T>(T obj, string path) {
            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();

            bformatter.Serialize(stream, obj);
            stream.Close();
        }

        /// <summary>
        /// Shortcut to deserialize an object from a file.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="path">The file to read from.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeFromFile<T>(string path) {
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            var obj = (T)bformatter.Deserialize(stream);
            stream.Close();

            return obj;
        }

        /// <summary>
        /// Calculates the point in a cubic Bezier curve at t (0 to 1) using 4 control points.
        /// </summary>
        /// <param name="t">Distance along the curve 0 to 1.</param>
        /// <param name="p0">The first control point.</param>
        /// <param name="p1">The second control point.</param>
        /// <param name="p2">The third control point.</param>
        /// <param name="p3">The fourth control point.</param>
        /// <returns>A Vector2 point along the curve at distance t.</returns>
        public static Vector2 BezierCurvePoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3) {
            // http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
            float u = 1 - t;
            float uu = u * u;
            float uuu = uu * u;

            float tt = t * t;
            float ttt = tt * t;

            Vector2 p = uuu * p0; // first term
            p += 3 * uu * t * p1; // second term
            p += 3 * u * tt * p2; // third term
            p += ttt * p3; // fourth term

            return p;
        }

        /// <summary>
        /// Find the point on a Bezier path at t.
        /// </summary>
        /// <param name="t">The progress along the path 0 to 1.</param>
        /// <param name="points">The points that make up the cubic Bezier path. (4 points per curve, 1 point of overlap.)</param>
        /// <returns>The position on the curve at t.</returns>
        static public Vector2 BezierPathPoint(float t, params Vector2[] points) {
            var length = points.Length - (points.Length - 1) % 3;

            var steps = length / 3;
            var step = (int)Math.Floor(ScaleClamp(t, 0, 1, 0, steps));

            int i = step * 3;

            var p0 = points[i];
            var p1 = points[i + 1];
            var p2 = points[i + 2];
            var p3 = points[i + 3];

            var segment = 1 / (float)(steps);

            var st = ScaleClamp(t - segment * step, 0, segment, 0, 1);

            return BezierCurvePoint(st, p0, p1, p2, p3);
        }

        static float CatmullRom(float a, float b, float c, float d, float t) {
            // Hermite Spline... I think?
            return 0.5f * (2 * b + (c - a) * t + (2 * a - 5 * b + 4 * c - d) * (t * t) + (3 * b - a - 3 * c + d) * (t * t * t));
        }

        /// <summary>
        /// Gets the spline path point.
        /// </summary>
        /// <param name="t">The progress on the path from 0 to 1.</param>
        /// <param name="path">The set of points making the path.</param>
        /// <returns>The position on the spline path for the progress t.</returns>
        public static Vector2 SplinePathPoint(float t, params Vector2[] path) {
            // Shoutouts to Chevy Ray
            int b = (int)((path.Length - 1) * t);
            int a = b - 1;
            int c = b + 1;
            int d = c + 1;
            a = (int)Util.Clamp(a, 0, path.Length - 1);
            b = (int)Util.Clamp(b, 0, path.Length - 1);
            c = (int)Util.Clamp(c, 0, path.Length - 1);
            d = (int)Util.Clamp(d, 0, path.Length - 1);
            float i = 1f / (path.Length - 1);
            t = (t - b * i) / i;
            return new Vector2(
                    CatmullRom(path[a].X, path[b].X, path[c].X, path[d].X, t),
                    CatmullRom(path[a].Y, path[b].Y, path[c].Y, path[d].Y, t)
                    );
        }

        /// <summary>
        /// The Width of the desktop.  Note that this is for the display that the game was initialized in.
        /// </summary>
        public static int DesktopWidth {
            get {
                return (int)SFML.Window.VideoMode.DesktopMode.Width;
            }
        }

        /// <summary>
        /// The height of the desktop.  Note that this is for the display that the game was initialized in.
        /// </summary>
        public static int DesktopHeight {
            get {
                return (int)SFML.Window.VideoMode.DesktopMode.Height;
            }
        }

        /// <summary>
        /// Combines Lists of the same type into one list. Does not remove duplicates.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="lists">The Lists to combine.</param>
        /// <returns>One List to rule them all.</returns>
        public static List<T> ListCombine<T>(params List<T>[] lists) {
            if (lists.Length == 0) return null;
            if (lists.Length == 1) return lists[0];

            var list = lists[0];
            for (int i = 1; i < lists.Length; i++) {
                list.AddRange(lists[i]);
            }
            return list;
        }

        /// <summary>
        /// Get all types with a specific Attribute.
        /// </summary>
        /// <typeparam name="T">The type of Attribute.</typeparam>
        /// <param name="inherit">Check inherited classes.</param>
        /// <returns>All types with the Attribute T.</returns>
        public static IEnumerable<Type> GetTypesWithAttribute<T>(bool inherit = true) where T : Attribute {
            return from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            where t.IsDefined(typeof(T), inherit)
            select t;
        }

        /// <summary>
        /// Download the json feed of a Google Spreadsheet from the key and sheet id.
        /// Only works on publicly shared sheets.
        /// </summary>
        /// <param name="docKey">The long id of the sheet.  Usually a big set of numbers and letters in the url.</param>
        /// <param name="sheetId">The sheet id.  Usually a shorter set of letters and numbers.  Default is "od6".</param>
        /// <returns>A json feed of the spreadsheet.</returns>
        public static string GetSpreadsheetJsonById(string docKey, string sheetId = "od6") {
            var url = string.Format("https://spreadsheets.google.com/feeds/list/{0}/{1}/public/values?alt=json", docKey, sheetId);
            return DownloadData(url);
        }

        /// <summary>
        /// Download the json feed of a Google Spreadsheet from the key and the sheet name.
        /// Only works on publicly shared sheets.
        /// </summary>
        /// <param name="docKey">The long id of the sheet.  Usually a big set of numbers and letters in the url.</param>
        /// <param name="sheetName">The name of the sheet as it appears on the bottom left of the spreadsheet window.</param>
        /// <returns>A json feed of the spreadsheet.</returns>
        public static string GetSpreadsheetJsonByName(string docKey, string sheetName) {
            return GetSpreadsheetJsonById(docKey, GetSpreadsheetId(docKey, sheetName));
        }

        /// <summary>
        /// Get the internal id of a Google Spreadsheet sheet.
        /// Only works on publicly shared sheets.
        /// </summary>
        /// <param name="docKey">The long id of the sheet.  Usually a big set of numbers and letters in the url.</param>
        /// <param name="sheetName">The name of the sheet as it appears on the bottom left of the spreadsheet window.</param>
        /// <returns>The internal id of the sheet.</returns>
        public static string GetSpreadsheetId(string docKey, string sheetName) {
            var url = string.Format("https://spreadsheets.google.com/feeds/worksheets/{0}/public/full", docKey);
            var xml = DownloadData(url);
            XElement xDoc = XElement.Parse(xml);
            var ns = "{" + xDoc.GetDefaultNamespace() + "}";
            var id = "od6";

            xDoc.Elements(ns + "entry")
                .Where(x => x.Element(ns + "title").Value == sheetName)
                .Each(x => x.Elements(ns + "id")
                    .Each(xx => {
                        id = xx.Value.Split('/').Last();
                    }));

            return id;
        }

        /// <summary>
        /// Get the internal ids of all sheets in a Google Spreadsheet.
        /// Only works on publicly shared sheets.
        /// </summary>
        /// <param name="docKey">The long id of the sheet.  Usually a big set of numbers and letters in the url.</param>
        /// <returns>A dictionary with sheet names as the keys, and ids as the values.</returns>
        public static Dictionary<string, string> GetSpreadsheetIds(string docKey) {
            var url = string.Format("https://spreadsheets.google.com/feeds/worksheets/{0}/public/full", docKey);
            var xml = DownloadData(url);
            XElement xDoc = XElement.Parse(xml);
            var ns = "{" + xDoc.GetDefaultNamespace() + "}";
            var dict = new Dictionary<string, string>();
            var name = "";
            var id = "";

            xDoc.Elements(ns + "entry")
                .Each(e => {
                    name = e.Element(ns + "title").Value;
                    id = e.Element(ns + "id").Value.Split('/').Last();
                    dict.Add(name, id);
                });

            return dict;
        }

        // Download url cache stuff
        static string downloadData;
        static string downloadUrl = "";

        static string DownloadData(string url) {
            if (downloadUrl != url) {
                downloadData = new WebClient().DownloadString(url);
                downloadUrl = url;
            }
            return downloadData;
        }

        #endregion

    }
}
