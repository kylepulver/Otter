using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Otter {
    /// <summary>
    /// Font used for loading premade textures of characters, usually arcade fonts and stuff like that.
    /// Currently supports fonts created with BMFont, Littera, and CBFG.
    /// </summary>
    public class BitmapFont : BaseFont {

        public Texture Texture { get; private set; }

        public int CharCodeOffset = 32; // most fonts start at Ascii code 32
        public int CharacterWidth = 8;
        public int CharacterHeight = 8;
        public int LineSpacing;
        public bool EnableKerning = true;

        public int CharacterOffsetX = 0;
        public int CharacterOffsetY = 0;

        public string FontData { get; private set; }
        public BitmapFontDataType DataType { get; private set; }

        public BitmapFont(Texture texture) {
            Texture = texture;
            DataType = BitmapFontDataType.None;
        }

        public BitmapFont(Texture texture, int charWidth, int charHeight, int charOffset = 32) : this(texture) {
            CharacterWidth = charWidth;
            CharacterHeight = charHeight;
            CharCodeOffset = charOffset;

            LineSpacing = CharacterHeight;
        }

        public BitmapFont(BitmapFontConfig config) : this(config.Texture) {
            CharacterWidth = config.CharacterWidth;
            CharacterHeight = config.CharacterHeight;
            CharCodeOffset = config.CharCodeOffset;
            CharacterOffsetX = config.CharacterOffsetX;
            CharacterOffsetY = config.CharacterOffsetY;

            LineSpacing = config.LineSpacing;
        }

        public BitmapFont(string source) : this(new Texture(source)) { }

        /// <summary>
        /// Loads the data to render the bitmap text with from a file.
        /// </summary>
        /// <param name="path">The path to the file that contains the data.</param>
        /// <param name="dataType">The type of data.</param>
        /// <returns>The BitmapFont</returns>
        public BitmapFont LoadDataFile(string path, BitmapFontDataType dataType) {
            return LoadData(File.ReadAllText(path), dataType);
        }

        /// <summary>
        /// Loads the data to render the bitmap text with.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The BitmapFont</returns>
        public BitmapFont LoadData(string data, BitmapFontDataType dataType) {
            FontData = data;
            DataType = dataType;

            switch (DataType) {
                case BitmapFontDataType.Littera:

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(data);

                    foreach (XmlNode n in xmlDoc.SelectNodes("//common")) {
                        LineSpacing = n.AttributeInt("lineHeight");
                    }

                    foreach (XmlNode n in xmlDoc.SelectNodes("//char")) {
                        var c = new CharData();
                        c.CharacterId = n.AttributeInt("id");
                        c.Width = n.AttributeInt("width");
                        c.Height = n.AttributeInt("height");
                        c.X = n.AttributeInt("x");
                        c.Y = n.AttributeInt("y");
                        c.OffsetX = n.AttributeInt("xoffset");
                        c.OffsetY = n.AttributeInt("yoffset");
                        c.Advance = n.AttributeInt("xadvance");
                        c.Character = (char)c.CharacterId;
                        if (!charData.ContainsKey(c.Character)) {
                            charData.Add(c.Character, c);
                        }
                    }

                    foreach (XmlNode n in xmlDoc.SelectNodes("//kerning")) {
                        var first = (char)n.AttributeInt("first");
                        var second = (char)n.AttributeInt("second");
                        var amount = n.AttributeInt("amount");

                        AddKerning(first, second, amount);
                    }

                    break;

                case BitmapFontDataType.BMFontXml:

                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(data);

                    foreach (XmlNode n in xmlDoc.SelectNodes("//common")) {
                        LineSpacing = n.AttributeInt("lineHeight");
                    }

                    foreach (XmlNode n in xmlDoc.SelectNodes("//char")) {
                        var c = new CharData();
                        c.CharacterId = n.AttributeInt("id");
                        c.Width = n.AttributeInt("width");
                        c.Height = n.AttributeInt("height");
                        c.X = n.AttributeInt("x");
                        c.Y = n.AttributeInt("y");
                        c.OffsetX = n.AttributeInt("xoffset");
                        c.OffsetY = n.AttributeInt("yoffset");
                        c.Advance = n.AttributeInt("xadvance");
                        c.Character = (char)c.CharacterId;
                        if (!charData.ContainsKey(c.Character)) {
                            charData.Add(c.Character, c);
                        }
                    }

                    foreach (XmlNode n in xmlDoc.SelectNodes("//kerning")) {
                        var first = (char)n.AttributeInt("first");
                        var second = (char)n.AttributeInt("second");
                        var amount = n.AttributeInt("amount");

                        AddKerning(first, second, amount);
                    }

                    break;

                case BitmapFontDataType.BMFontText:

                    var lines = data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines) {
                        var entries = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        var header = entries[0];

                        var textData = new Dictionary<string, string>();

                        switch (header) {
                            case "info":

                                // Dont need any of this for now.

                                break;

                            case "common":

                                foreach (var e in entries) {
                                    if (!e.Contains('=')) continue;

                                    textData.Add(GetKeyValueFromString(e)[0], GetKeyValueFromString(e)[1]);
                                }

                                LineSpacing = int.Parse(textData["lineHeight"]);

                                break;

                            case "char":

                                var c = new CharData();

                                foreach (var e in entries) {
                                    if (!e.Contains('=')) continue;

                                    textData.Add(GetKeyValueFromString(e)[0], GetKeyValueFromString(e)[1]);
                                }

                                c.CharacterId = int.Parse(textData["id"]);
                                c.Character = (char)int.Parse(textData["id"]);
                                c.Width = int.Parse(textData["width"]);
                                c.Height = int.Parse(textData["height"]);
                                c.X = int.Parse(textData["x"]);
                                c.Y = int.Parse(textData["y"]);
                                c.OffsetX = int.Parse(textData["xoffset"]);
                                c.OffsetY = int.Parse(textData["yoffset"]);
                                c.Advance = int.Parse(textData["xadvance"]);

                                if (!charData.ContainsKey(c.Character)) {
                                    charData.Add(c.Character, c);
                                }

                                break;

                            case "kerning":

                                foreach (var e in entries) {
                                    if (!e.Contains('=')) continue;

                                    textData.Add(GetKeyValueFromString(e)[0], GetKeyValueFromString(e)[1]);
                                }

                                AddKerning((char)int.Parse(textData["first"]), (char)int.Parse(textData["second"]), int.Parse(textData["amount"]));

                                break;
                        }
                    }


                    break;

                case BitmapFontDataType.CodeheadCSV:

                    lines = data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    var csv = new Dictionary<string, string>();

                    foreach(var line in lines) {
                        csv.Add(GetKeyValueFromString(line, ',')[0], GetKeyValueFromString(line, ',')[1]);
                    }

                    var imgWidth = int.Parse(csv["Image Width"]);
                    var startChar = int.Parse(csv["Start Char"]);
                    var imgCellWidth = int.Parse(csv["Cell Width"]);
                    var imgCellHeight = int.Parse(csv["Cell Height"]);

                    var charsPerRow = (int)Util.Floor(imgWidth / imgCellWidth);

                    for (var i = 0; i < 256; i++) {
                        var c = new CharData();
                       
                        c.Width = int.Parse(csv[string.Format("Char {0} Base Width", i)]);
                        c.OffsetX = int.Parse(csv[string.Format("Char {0} X Offset", i)]);
                        c.OffsetY = int.Parse(csv[string.Format("Char {0} Y Offset", i)]);
                        var charPos = i - startChar;
                        c.X = Util.TwoDeeX(charPos, charsPerRow) * imgCellWidth;
                        c.Y = Util.TwoDeeY(charPos, charsPerRow) * imgCellHeight;
                        var widthOffset = int.Parse(csv[string.Format("Char {0} Width Offset", i)]);
                        c.Height = int.Parse(csv["Font Height"]);
                        c.Advance = c.Width + widthOffset;
                        c.Character = (char)i;

                        LineSpacing = c.Height;

                        if (!charData.ContainsKey(c.Character)) {
                            charData.Add(c.Character, c);
                        }
                    }

                    break;

                case BitmapFontDataType.Shoebox:

                    lines = data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines) {
                        var entries = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        var header = entries[0];

                        var textData = new Dictionary<string, string>();

                        switch (header) {
                            case "info":

                                break;

                            case "common":

                                foreach (var e in entries) {
                                    if (!e.Contains('=')) continue;

                                    textData.Add(GetKeyValueFromString(e)[0], GetKeyValueFromString(e)[1]);
                                }

                                LineSpacing = int.Parse(textData["lineHeight"]);

                                break;

                            case "char":

                                var c = new CharData();

                                foreach (var e in entries) {
                                    if (!e.Contains('=')) continue;

                                    textData.Add(GetKeyValueFromString(e)[0], GetKeyValueFromString(e)[1]);
                                }

                                c.CharacterId = int.Parse(textData["id"]);
                                c.Character = (char)int.Parse(textData["id"]);
                                c.Width = int.Parse(textData["width"]);
                                c.Height = int.Parse(textData["height"]);
                                c.X = int.Parse(textData["x"]);
                                c.Y = int.Parse(textData["y"]);
                                c.OffsetX = int.Parse(textData["xoffset"]);
                                c.OffsetY = int.Parse(textData["yoffset"]);
                                c.Advance = int.Parse(textData["xadvance"]);

                                charData.Add(c.Character, c);

                                break;

                            case "kerning":

                                foreach (var e in entries) {
                                    if (!e.Contains('=')) continue;

                                    textData.Add(GetKeyValueFromString(e)[0], GetKeyValueFromString(e)[1]);
                                }

                                AddKerning((char)int.Parse(textData["first"]), (char)int.Parse(textData["second"]), int.Parse(textData["amount"]));

                                break;
                        }

                    }

                    break;
            }

            return this;
        }

        string[] GetKeyValueFromString(string str, char delim = '=') {
            string[] result = new string[2];

            result[0] = str.Substring(0, str.IndexOf(delim));
            result[1] = str.Substring(str.IndexOf(delim) + 1, str.Length - str.IndexOf(delim) - 1);

            return result;
        }

        Dictionary<char, Dictionary<char, int>> kerningPairs = new Dictionary<char, Dictionary<char, int>>();
        Dictionary<char, CharData> charData = new Dictionary<char, CharData>();

        internal override SFML.Graphics.Glyph GetGlyph(char c, int size, bool bold) {

            var rect = new SFML.Graphics.IntRect();
            var bounds = new SFML.Graphics.FloatRect();
            var advance = 0;

            if (DataType == BitmapFontDataType.None) {

                var width = Texture.Width;
                var height = Texture.Height;

                var offsetChar = (int)c - CharCodeOffset;
                rect.Left = Util.TwoDeeX(offsetChar * CharacterWidth, width) + CharacterOffsetX;
                rect.Top = Util.TwoDeeY(offsetChar * CharacterHeight, width) * CharacterHeight + CharacterOffsetY;

                rect.Width = CharacterWidth;
                rect.Height = CharacterHeight;

                bounds.Top = 0;
                bounds.Left = 0;
                bounds.Width = CharacterWidth;
                bounds.Height = CharacterHeight;

                advance = CharacterWidth;

            }
            else {

                var data = charData[c];

                rect.Left = data.X;
                rect.Top = data.Y;
                rect.Width = data.Width;
                rect.Height = data.Height;

                bounds.Left = data.OffsetX;
                bounds.Top = data.OffsetY;
                bounds.Width = data.Width;
                bounds.Height = data.Height;

                advance = data.Advance;
            }

            return new SFML.Graphics.Glyph() {
                Advance = advance,
                Bounds = bounds,
                TextureRect = rect
            };
        }

        internal override float GetLineSpacing(int size) {
            return LineSpacing;
        }

        public override float GetKerning(char first, char second, int characterSize) {
            if (!EnableKerning) return 0;

            if (!kerningPairs.ContainsKey(first)) {
                return 0;
            }
            if (!kerningPairs[first].ContainsKey(second)) {
                return 0;
            }
            return kerningPairs[first][second];
        }

        void AddKerning(char first, char second, int amount) {
            if (!kerningPairs.ContainsKey(first)) {
                kerningPairs.Add(first, new Dictionary<char, int>());
            }
            if (!kerningPairs[first].ContainsKey(second)) {
                kerningPairs[first].Add(second, amount);
            }
            kerningPairs[first][second] = amount;
        }

        internal override Texture GetTexture(int size) {
            return Texture;
        }
    }

    class CharData {

        public char Character;
        public int CharacterId;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int Advance;
        public int OffsetX;
        public int OffsetY;

    }

    public class BitmapFontConfig {
        public Texture Texture;
        public int CharacterWidth;
        public int CharacterHeight;
        public int CharCodeOffset = 32;
        public int CharacterOffsetX;
        public int CharacterOffsetY;
        public int LineSpacing;
    }

    public enum BitmapFontDataType {
        None, // Just plain old monospaced characters.
        BMFontText, // http://www.angelcode.com/products/bmfont/
        BMFontXml, // http://www.angelcode.com/products/bmfont/
        Littera, // http://kvazars.com/littera/
        CodeheadCSV, // http://www.codehead.co.uk/cbfg/
        Shoebox // http://renderhjs.net/shoebox/bitmapFont.htm
    }
}
