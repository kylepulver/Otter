using System;
using System.Collections.Generic;
using System.Xml;

namespace Otter {
    /// <summary>
    /// Class that represents a color with red, green, blue, and alpha channels.
    /// </summary>
    public class Color {

        #region Static Methods

        /// <summary>
        /// Interpolate from one Color to another.
        /// </summary>
        /// <param name="from">The start Color.</param>
        /// <param name="to">The end Color.</param>
        /// <param name="amount">The amount of completion on the lerp. (0 - 1)</param>
        /// <returns>The interpolated Color.</returns>
        public static Color Lerp(Color from, Color to, float amount) {
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
        public static Color Lerp(float amount, params Color[] colors) {
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

            return Lerp(colors[fromIndex], colors[toIndex], lerp);
        }

        /// <summary>
        /// Return a new color made by mixing multiple colors.
        /// Mixes the colors evenly.
        /// </summary>
        /// <param name="colors">The colors to be mixed.</param>
        /// <returns>A new color of all the colors mixed together.</returns>
        public static Color Mix(params Color[] colors) {
            float r = 0, g = 0, b = 0, a = 0;

            foreach (var c in colors) {
                r += c.R;
                g += c.G;
                b += c.B;
                a += c.A;
            }

            r /= colors.Length;
            g /= colors.Length;
            b /= colors.Length;
            a /= colors.Length;

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Create a new color from HSV values.
        /// </summary>
        /// <param name="h">Hue, 0 to 360.</param>
        /// <param name="s">Saturation, 0 to 1.</param>
        /// <param name="v">Value, 0 to 1.</param>
        /// <param name="a">Alpha, 0 to 1.</param>
        /// <returns>A new RGBA color.</returns>
        public static Color FromHSV(float h, float s, float v, float a) {
            h = h < 0 ? 0 : (h > 1 ? 1 : h);
            s = s < 0 ? 0 : (s > 1 ? 1 : s);
            v = v < 0 ? 0 : (v > 1 ? 1 : v);
            h *= 360;

            int hi = (int)(h / 60) % 6;
            float f = (h / 60) - (int)(h / 60);
            float p = (v * (1 - s));
            float q = (v * (1 - f * s));
            float t = (v * (1 - (1 - f) * s));
            float r, g, b;

            switch (hi) {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                case 5: r = v; g = p; b = q; break;
                default: r = g = b = 0; break;
            }

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Store a custom Color.  Actually stores a new copy of that Color.
        /// </summary>
        /// <param name="color">The Color to store.</param>
        /// <param name="name">The name of the Color.</param>
        public static void AddCustom(Color color, Enum name) {
            customColors.Add(Util.EnumValueToString(name), new Color(color));
        }

        /// <summary>
        /// Store a custom Color.  Actually stores a new copy of that Color.
        /// </summary>
        /// <param name="color">The Color to store.</param>
        /// <param name="name">The name of the Color.</param>
        public static void AddCustom(string color, Enum name) {
            AddCustom(new Color(color), name);
        }

        /// <summary>
        /// Store a custom Color.  Actually stores a new copy of that Color.
        /// </summary>
        /// <param name="color">The Color to store.</param>
        /// <param name="name">The name of the Color.</param>
        public static void AddCustom(UInt32 color, Enum name) {
            AddCustom(new Color(color), name);
        }

        /// <summary>
        /// Store a custom Color.  Actually stores a new copy of that Color.
        /// </summary>
        /// <param name="color">The Color to store.</param>
        /// <param name="name">The name of the Color.</param>
        public static void AddCustom(Color color, string name) {
            customColors.Add(name, new Color(color));
        }

        /// <summary>
        /// Store a custom Color.  Actually stores a new copy of that Color.
        /// </summary>
        /// <param name="color">The Color to store.</param>
        /// <param name="name">The name of the Color.</param>
        public static void AddCustom(string color, string name) {
            AddCustom(new Color(color), name);
        }

        /// <summary>
        /// Store a custom Color.  Actually stores a new copy of that Color.
        /// </summary>
        /// <param name="color">The Color to store.</param>
        /// <param name="name">The name of the Color.</param>
        public static void AddCustom(UInt32 color, string name) {
            AddCustom(new Color(color), name);
        }

        /// <summary>
        /// Get a stored custom Color.  Returns a new copy of it.
        /// </summary>
        /// <param name="name">The name of the Color stored.</param>
        /// <returns>A new copy of the custom Color.</returns>
        public static Color Custom(Enum name) {
            return customColors[Util.EnumValueToString(name)].Copy();
        }

        public static Color Custom(string name) {
            return customColors[name].Copy();
        }

        /// <summary>
        /// Create a shade of gray based on a value 0 to 1.
        /// </summary>
        /// <param name="rgb">The level of gray. 0 is black, 1 is white.</param>
        /// <returns>A color of RGB equal to the value input for rgb.</returns>
        public static Color Shade(float rgb) {
            return new Color(rgb, rgb, rgb);
        }

        /// <summary>
        /// Create a shade of gray based on a value 0 to 1.
        /// </summary>
        /// <param name="rgb">The level of gray. 0 is black, 1 is white.</param>
        /// <param name="a">The alpha of the returned Color.</param>
        /// <returns>A color of RGB equal to the value input for rgb with alpha a.</returns>
        public static Color Shade(float rgb, float a) {
            return new Color(rgb, rgb, rgb, a);
        }

        /// <summary>
        /// Create a Color using byte values 0 - 255.
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <param name="a">Alpha</param>
        /// <returns>A new Color.</returns>
        public static Color FromBytes(byte r, byte g, byte b, byte a = 255) {
            var color = new Color();
            color.ByteR = r;
            color.ByteG = g;
            color.ByteB = b;
            color.ByteA = a;
            return color;
        }

        #endregion

        #region Static Properties

        public static Color White {
            get { return new Color(1f, 1f, 1f); }
        }

        public static Color Black {
            get { return new Color(0f, 0f, 0f); }
        }

        public static Color Red {
            get { return new Color(1f, 0f, 0f); }
        }

        public static Color Green {
            get { return new Color(0f, 1f, 0f); }
        }

        public static Color Blue {
            get { return new Color(0f, 0f, 1f); }
        }

        public static Color Cyan {
            get { return new Color(0f, 1f, 1f); }
        }

        public static Color Magenta {
            get { return new Color(1f, 0f, 1f); }
        }

        public static Color Yellow {
            get { return new Color(1f, 1f, 0f); }
        }

        public static Color Orange {
            get { return new Color(1f, 0.5f, 0); }
        }

        public static Color Gold {
            get { return new Color("FFCC00"); }
        }

        public static Color None {
            get { return new Color(0f, 0f, 0f, 0f); }
        }

        public static Color Grey {
            get { return new Color("999999"); }
        }

        public static Color Gray {
            get { return Color.Grey; }
        }

        public static Color Random {
            get { return Rand.Color; }
        }

        public static Color RandomAlpha {
            get { return Rand.ColorAlpha; }
        }

        #endregion

        #region Static Fields

        static Dictionary<string, Color> customColors = new Dictionary<string, Color>();

        #endregion

        #region Private Fields

        float r, g, b, a;

        #endregion

        #region Public Properties

        /// <summary>
        /// Red
        /// </summary>
        public float R {
            get { return r; }
            set {
                r = Util.Clamp(value, 0, 1);
                if (Graphic != null) { Graphic.NeedsUpdate = true; }
            }
        }

        /// <summary>
        /// Green
        /// </summary>
        public float G {
            get { return g; }
            set {
                g = Util.Clamp(value, 0, 1);
                if (Graphic != null) { Graphic.NeedsUpdate = true; }
            }
        }

        /// <summary>
        /// Blue
        /// </summary>
        public float B {
            get { return b; }
            set {
                b = Util.Clamp(value, 0, 1);
                if (Graphic != null) { Graphic.NeedsUpdate = true; }
            }
        }

        /// <summary>
        /// Alpha
        /// </summary>
        public float A {
            get { return a; }
            set {
                a = Util.Clamp(value, 0, 1);
                if (Graphic != null) { Graphic.NeedsUpdate = true; }
            }
        }

        /// <summary>
        /// The bytes for Red.
        /// </summary>
        public byte ByteR {
            set {
                R = Convert.ToSingle(value) / 255f;
            }
            get {
                return (byte)(R * 255);
            }
        }

        /// <summary>
        /// The bytes for Green.
        /// </summary>
        public byte ByteG {
            set {
                G = Convert.ToSingle(value) / 255f;
            }
            get {
                return (byte)(G * 255);
            }
        }

        /// <summary>
        /// The bytes for Blue.
        /// </summary>
        public byte ByteB {
            set {
                B = Convert.ToSingle(value) / 255f;
            }
            get {
                return (byte)(B * 255);
            }
        }

        /// <summary>
        /// The bytes for Alpha.
        /// </summary>
        public byte ByteA {
            set {
                A = Convert.ToSingle(value) / 255f;
            }
            get {
                return (byte)(A * 255);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new color.
        /// </summary>
        /// <param name="r">Red, 0 to 1.</param>
        /// <param name="g">Green, 0 to 1.</param>
        /// <param name="b">Blue, 0 to 1.</param>
        /// <param name="a">Alpha, 0 to 1.</param>
        public Color(float r = 1f, float g = 1f, float b = 1f, float a = 1f) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Create a color by copying the RGBA from another color.
        /// </summary>
        /// <param name="copy">The color to copy.</param>
        public Color(Color copy) : this(copy.R, copy.G, copy.B, copy.A) { }

        /// <summary>
        /// Create a new color from an XML element.
        /// </summary>
        /// <param name="e">An XmlElement that contains attributes R, G, B, and A, from 0 to 255.</param>
        public Color(XmlElement e) {
            A = float.Parse(e.Attributes["A"].Value) / 255;
            R = float.Parse(e.Attributes["R"].Value) / 255;
            G = float.Parse(e.Attributes["G"].Value) / 255;
            B = float.Parse(e.Attributes["B"].Value) / 255;
        }

        /// <summary>
        /// Return a new color containing the channels from this color.
        /// </summary>
        /// <returns></returns>
        public Color Copy() {
            return new Color(R, G, B, A);
        }

        /// <summary>
        /// Create a new color from a string.  Formats are "RGB", "RGBA", "RRGGBB", and "RRGGBBAA".
        /// </summary>
        /// <param name="hex">A string with a hex representation of each channel.</param>
        public Color(string hex) {
            int red = 255, green = 255, blue = 255, alpha = 255;

            if (hex.Length == 6) {
                red = Convert.ToInt32(hex.Substring(0, 2), 16);
                green = Convert.ToInt32(hex.Substring(2, 2), 16);
                blue = Convert.ToInt32(hex.Substring(4, 2), 16);
            }
            else if (hex.Length == 3) {
                red = Convert.ToInt32(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
                green = Convert.ToInt32(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
                blue = Convert.ToInt32(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
            }
            else if (hex.Length == 8) {
                red = Convert.ToInt32(hex.Substring(0, 2), 16);
                green = Convert.ToInt32(hex.Substring(2, 2), 16);
                blue = Convert.ToInt32(hex.Substring(4, 2), 16);
                alpha = Convert.ToInt32(hex.Substring(6, 2), 16);
            }
            else if (hex.Length == 4) {
                red = Convert.ToInt32(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
                green = Convert.ToInt32(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
                blue = Convert.ToInt32(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
                alpha = Convert.ToInt32(hex.Substring(3, 1) + hex.Substring(2, 1), 16);
            }

            R = Util.ScaleClamp(red, 0, 255, 0, 1);
            G = Util.ScaleClamp(green, 0, 255, 0, 1);
            B = Util.ScaleClamp(blue, 0, 255, 0, 1);
            A = Util.ScaleClamp(alpha, 0, 255, 0, 1);
        }

        /// <summary>
        /// Create a new color from a hex number.  Formats are 0xRGB, 0xRRGGBB, 0xRGBA, 0xRRGGBBAA.
        /// </summary>
        /// <param name="hex">A hex number representing a color.</param>
        public Color(UInt32 hex) : this(hex.ToString("X6")) { }
        #endregion

        #region Public Methods

        public override string ToString() {
            return "Color [R: " + R.ToString("0.00") + " G: " + G.ToString("0.00") + " B: " + B.ToString("0.00") + " A: " + A.ToString("0.00") + "]";
        }

        /// <summary>
        /// Get a hex string of the Color.
        /// </summary>
        public string ColorString {
            get {
                return ByteR.ToString("X2") + ByteG.ToString("X2") + ByteB.ToString("X2") + ByteA.ToString("X2");
            }
        }

        public void SetColor(float r, float g, float b, float a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public void SetColor(float r, float g, float b) {
            SetColor(r, g, b, A);
        }

        public void SetColor(Color color) {
            SetColor(color.R, color.G, color.B, color.A);
        }

        #endregion

        #region Operators

        public static Color operator *(Color value1, Color value2) {
            value1 = new Color(value1); // Make new color otherwise this doesn't work properly.
            value1.R *= value2.R;
            value1.G *= value2.G;
            value1.B *= value2.B;
            value1.A *= value2.A;
            return value1;
        }

        public static Color operator *(Color value1, float value2) {
            value1 = new Color(value1); 
            value1.R *= value2;
            value1.G *= value2;
            value1.B *= value2;
            return value1;
        }

        public static Color operator +(Color value1, Color value2) {
            value1 = new Color(value1); 
            value1.R += value2.R;
            value1.G += value2.G;
            value1.B += value2.B;
            value1.A += value2.A;
            return value1;
        }

        public static Color operator -(Color value1, Color value2) {
            value1 = new Color(value1); 
            value1.R -= value2.R;
            value1.G -= value2.G;
            value1.B -= value2.B;
            value1.A -= value2.A;
            return value1;
        }

        public static Color operator /(Color value1, Color value2) {
            value1 = new Color(value1); 
            value1.R /= value2.R;
            value1.G /= value2.G;
            value1.B /= value2.B;
            value1.A /= value2.A;
            return value1;
        }

        #endregion

        #region Internal

        internal Color(SFML.Graphics.Color copy) {
            ByteR = copy.R;
            ByteG = copy.G;
            ByteB = copy.B;
            ByteA = copy.A;
        }

        internal SFML.Graphics.Color SFMLColor {
            get { return new SFML.Graphics.Color(ByteR, ByteG, ByteB, ByteA); }
        }

        internal Graphic Graphic; // Keep track of the graphic so it can be flagged for an update when the color changes

        #endregion

    }
}
