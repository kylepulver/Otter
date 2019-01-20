using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace Otter {
    /// <summary>
    /// Various extensions for classes are in here.
    /// </summary>
    public static class Extensions {

        #region XML

        /// <summary>
        /// Parse an attribute from an XmlElement as an int.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>An value as an int.</returns>
        public static int AttributeInt(this XmlNode xml, string name) {
            return int.Parse(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse an attribute from an XmlElement as an int.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="returnOnNull">The default value to return if that attribute doesn't exist.</param>
        /// <returns>The value as an int.</returns>
        public static int AttributeInt(this XmlNode xml, string name, int returnOnNull) {
            if (xml == null) return returnOnNull;
            if (xml.Attributes[name] == null) return returnOnNull;
            return int.Parse(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse an attibute from an XmlElement as a string.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value as a string.</returns>
        public static string AttributeString(this XmlNode xml, string name) {
            return xml.Attributes[name].Value;
        }

        /// <summary>
        /// Parse an attibute from an XmlElement as a string.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="returnOnNull">The default value to return if that attribute doesn't exist.</param>
        /// <returns>The value as a string.</returns>
        public static string AttributeString(this XmlNode xml, string name, string returnOnNull) {
            if (xml == null) return returnOnNull;
            if (xml.Attributes[name] == null) return returnOnNull;
            return xml.Attributes[name].Value;
        }

        /// <summary>
        /// Parse an attribute from an XmlElement as a float.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value as a float.</returns>
        public static float AttributeFloat(this XmlNode xml, string name) {
            return float.Parse(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse an attribute from an XmlElement as a float.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="returnOnNull">The default value to return if that attribute doesn't exist.</param>
        /// <returns>The value as a float.</returns>
        public static float AttributeFloat(this XmlNode xml, string name, float returnOnNull) {
            if (xml == null) return returnOnNull;
            if (xml.Attributes[name] == null) return returnOnNull;
            return float.Parse(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse an attribute from an XmlElement as a bool.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value as a bool.</returns>
        public static bool AttributeBool(this XmlNode xml, string name) {
            return bool.Parse(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse an attribute from an XmlElement as a bool.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="returnOnNull">The default value to return if that attribute doesn't exist.</param>
        /// <returns>The value as a bool.</returns>
        public static bool AttributeBool(this XmlNode xml, string name, bool returnOnNull) {
            if (xml == null) return returnOnNull;
            if (xml.Attributes[name] == null) return returnOnNull;
            return bool.Parse(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse an attribute from an XmlElement as a Color.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The value as a Color.</returns>
        public static Color AttributeColor(this XmlNode xml, string name) {
            return new Color(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse an attribute from an XmlElement as a Color.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="returnOnNull">The default value to return if that attribute doesn't exist.</param>
        /// <returns>The value as a Color.</returns>
        public static Color AttributeColor(this XmlNode xml, string name, Color returnOnNull) {
            if (xml == null) return returnOnNull;
            if (xml.Attributes[name] == null) return returnOnNull;
            return new Color(xml.Attributes[name].Value);
        }

        /// <summary>
        /// Parse the inner text of an XmlElement as an int.
        /// </summary>
        /// <param name="xml">The XmlElement to parse.</param>
        /// <returns>The value as an int.</returns>
        public static int InnerInt(this XmlNode xml) {
            return int.Parse(xml.InnerText);
        }

        public static int InnerInt(this XmlNode xml, int returnOnNull) {
            if (xml != null) return xml.InnerInt();
            return returnOnNull;
        }

        /// <summary>
        /// Parse the inner text of an XmlElement as a float.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>The value as a float.</returns>
        public static float InnerFloat(this XmlNode xml) {
            return float.Parse(xml.InnerText);
        }

        public static float InnerFloat(this XmlNode xml, float returnOnNull) {
            if (xml != null) return xml.InnerFloat();
            return returnOnNull;
        }

        public static string InnerText(this XmlNode xml, string returnOnNull) {
            if (xml != null) return xml.InnerText;
            return returnOnNull;
        }

        /// <summary>
        /// Parse the inner text of an XmlElement as a bool.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>The value as a bool.</returns>
        public static bool InnerBool(this XmlNode xml) {
            return bool.Parse(xml.InnerText);
        }

        /// <summary>
        /// Parse the inner text of an XmlElement as a Color.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>The value as a Color.</returns>
        public static Color InnerColor(this XmlNode xml) {
            return new Color(xml.InnerText);
        }

        /// <summary>
        /// Parse an int from an attribute collection.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="name">The attribute to parse.</param>
        /// <param name="returnOnNull">The value to return if null.</param>
        /// <returns>The attribute as an int.</returns>
        public static int Int(this XmlAttributeCollection a, string name, int returnOnNull) {
            if (a[name] == null) return returnOnNull;
            return int.Parse(a[name].Value);
        }

        #endregion

        #region String
        // String Extensions

        static public string ClearWhitespace(this string str) {
            return Regex.Replace(str, @"\s+", "");
        }

        #endregion

        #region Dictionary
        /// <summary>
        /// Get a value out of a Dictionary of strings as an int, and return a default value if the key
        /// is not present in the Dictionary.
        /// </summary>
        /// <param name="d">The Dictionary.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="onNull">The value to return if that key is not found.</param>
        /// <returns>The value from the Dictionary as an int.</returns>
        static public int ValueAsInt(this Dictionary<string, string> d, string key, int onNull = 0) {
            if (d.ContainsKey(key)) {
                return int.Parse(d[key]);
            }
            return onNull;
        }

        /// <summary>
        /// Get a value out of a Dictionary of strings as a a float, and return a default value if the key
        /// is not present in the Dictionary.
        /// </summary>
        /// <param name="d">The Dictionary.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="onNull">The value to return if that key is not found.</param>
        /// <returns>The value from the Dictionary as an float.</returns>
        static public float ValueAsFloat(this Dictionary<string, string> d, string key, float onNull = 0) {
            if (d.ContainsKey(key)) {
                return float.Parse(d[key]);
            }
            return onNull;
        }

        /// <summary>
        /// Get a value out of a Dictionary of strings as a a bool, and return a default value if the key
        /// is not present in the Dictionary.
        /// </summary>
        /// <param name="d">The Dictionary.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="onNull">The value to return if that key is not found.</param>
        /// <returns>The value from the Dictionary as a bool.</returns>
        static public bool ValueAsBool(this Dictionary<string, string> d, string key, bool onNull = false) {
            if (d.ContainsKey(key)) {
                return bool.Parse(d[key]);
            }
            return onNull;
        }

        /// <summary>
        /// Get the value out of a Dictionary of strings as a Color, and return a default value if the key
        /// is not present in the Dictionary.
        /// </summary>
        /// <param name="d">The Dictionary.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="onNull">The value to return if that key is not found.</param>
        /// <returns>The value fro the Dictionary as a Color.</returns>
        static public Color ValueAsColor(this Dictionary<string, string> d, string key, Color onNull = null) {
            if (d.ContainsKey(key)) {
                return new Color(d[key]);
            }
            return onNull;
        }

        static public void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value) {
            if (d.ContainsKey(key)) d[key] = value;
            else d.Add(key, value);
        }

        static public TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value = default(TValue)) {
            if (d.ContainsKey(key)) return d[key];
            d.Add(key, value);
            return value;
        }

        #endregion

        #region List

        /// <summary>
        /// Removes an item from a list only if the list contains that item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <param name="element">The element to remove.</param>
        /// <returns>True if the element was removed successfully.</returns>
        static public bool RemoveIfContains<T>(this List<T> l, T element) {
            if (l.Contains(element)) {
                l.Remove(element);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add multiple items to a list.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="l">The list.</param>
        /// <param name="elements">The elements to add.</param>
        static public void Add<T>(this List<T> l, params T[] elements) {
            foreach (var i in elements) {
                l.Add(i);
            }
        }

        static public void MoveForward<T>(this List<T> list, T item) {
            var oldIndex = list.IndexOf(item);
            list.Remove(item);
            list.InsertOrAdd(oldIndex - 1, item);
        }

        public static void MoveBackward<T>(this List<T> list, T item) {
            var oldIndex = list.IndexOf(item);
            list.Remove(item);
            list.InsertOrAdd(oldIndex + 1, item);
        }

        public static void MoveToFront<T>(this List<T> list, T item) {
            list.Remove(item);
            list.InsertOrAdd(0, item);
        }

        public static void MoveToBack<T>(this List<T> list, T item) {
            list.Remove(item);
            list.Add(item);
        }

        #endregion

        #region SFML

        public static void Append(this VertexArray vertices, float x, float y, Color color, float tx, float ty) {
            vertices.Append(new Vertex(new Vector2f(x, y), color.SFMLColor, new Vector2f(tx, ty)));
        }

        public static void Append(this VertexArray vertices, float x, float y, Color color = null) {
            if (color == null) color = Color.White;
            vertices.Append(new Vertex(new Vector2f(x, y), color.SFMLColor));
        }

        public static void Append(this VertexArray vertices, double x, double y, Color color) {
            vertices.Append(new Vertex(new Vector2f((float)x, (float)y), color.SFMLColor));
        }

        public static void Append(this VertexArray vertices, Vert vert) {
            vertices.Append(vert.X, vert.Y, vert.Color, vert.U, vert.V);
        }

        #endregion

        #region Other

        /// <summary>
        /// Converts a RGB or RGBA uint value to a Color.
        /// </summary>
        /// <param name="i">The uint.</param>
        /// <returns>A new Color from the uint.</returns>
        public static Color ToColor(this uint i) {
            return new Color(i);
        }

        /// <summary>
        /// Converts a RGB or RGBA int value to a Color.
        /// </summary>
        /// <param name="i">The int.</param>
        /// <returns>A new Color from the int.</returns>
        public static Color ToColor(this int i) {
            return new Color((uint)i);
        }

        /// <summary>
        /// Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="variable">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns>True if the Enum contains the flag.</returns>
        public static bool HasFlag(this Enum variable, Enum value) {
            // http://stackoverflow.com/questions/4108828/generic-extension-method-to-see-if-an-enum-contains-a-flag
            if (variable == null)
                return false;

            if (value == null)
                throw new ArgumentNullException("value");

            // Not as good as the .NET 4 version of this function, but should be good enough
            if (!Enum.IsDefined(variable.GetType(), value)) {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            ulong num = Convert.ToUInt64(value);
            return ((Convert.ToUInt64(variable) & num) == num);

        }

        #endregion

    }
}
