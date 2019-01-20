using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Otter {
    /// <summary>
    /// Graphic that renders text with some more options than normal Text.
    /// RichText can be very slow to render with large strings of text so be careful!
    /// For large blocks of text use the normal Text graphic.
    /// </summary>
    /// <example>
    /// richText.String = "Hello, {color:f00}this text is red!{clear} {shake:4}Shaking text!";
    /// <code>
    /// Commands:
    ///     {clear} - Clear all styles and reset back to normal, white text.
    ///     {style:name} - Apply the style 'name' to text.  Create styles with AddStyle().
    ///     {color:fff} - Colors text. Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color0:fff} - Colors the top left corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color1:fff} - Colors the top right corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color2:fff} - Colors the bottom right corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {color3:fff} - Colors the bottom left corner of characters.  Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {colorShadow:fff} - Colors text shadow. Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {colorOutline:fff} - Colors text outline. Strings of 3, 4, 6, or 8 hex digits allowed.
    ///     {shadowX:0} - Set the drop shadow of the text on the X axis.
    ///     {shadowY:0} - Set the drop shadow of the text on the Y axis.
    ///     {shadow:0} - Set the drop shadow of the text on the X and Y axes.
    ///     {outline:0} - Set the outline thickness on text.
    ///     {shakeX:0} - Shake the text on the X axis with a float range.
    ///     {shakeY:0} - Shake the text on the Y axis with a float range.
    ///     {shake:0} - Shake the text on the X and Y axes with a float range.
    ///     {waveAmpX:0} - Wave the text on the X axis with a float range.
    ///     {waveAmpY:0} - Wave the text on the Y axis with a float range.
    ///     {waveAmp:0} - Wave the text on the X and Y axes with a float range.
    ///     {waveRateX:0} - Set the wave speed for the X axis.
    ///     {waveRateY:0} - Set the wave speed for the Y axis.
    ///     {waveRate:0} - Set the wave speed for the X and Y axes.
    ///     {waveOffsetX:0} - Set the wave offset for the X axis.
    ///     {waveOffsetY:0} - Set the wave offset for the Y axis.
    ///     {waveOffset:0} - Set the wave offset for the X and Y axes.
    ///     {offset:0} - Set the offset rate for characters.
    ///     {charOffsetX:0} - Set the character offset X for the BitmapFont.
    ///     {charOffsetY:0} - Set the character offset Y for the BitmapFont.
    /// </code>
    /// </example>
    public class RichText : Graphic {

        #region Static Fields

        static Dictionary<string, string> styles = new Dictionary<string, string>();

        #endregion

        #region Static Methods

        /// <summary>
        /// Add a global style to RichText objects.  The style will not be updated unless Refresh() is
        /// called on the objects.
        /// </summary>
        /// <example>
        /// RichText.AddStyle("important","color:f00,waveAmpY:2,waveRate:2");
        /// </example>
        /// <param name="name">The name of the style.</param>
        /// <param name="content">The properties to set using commas as a delim character.</param>
        static public void AddStyle(string name, string content) {
            if (styles.ContainsKey(name)) {
                styles[name] = content;
                return;
            }
            styles.Add(name, content);
        }

        /// <summary>
        /// Removes a style from all RichText objects.
        /// </summary>
        /// <param name="name">The name of the style to remove.</param>
        static public void RemoveStyle(string name) {
            styles.Remove(name);
        }

        /// <summary>
        /// Remove all styles from RichText objects.
        /// </summary>
        static public void ClearStyles() {
            styles.Clear();
        }

        #endregion

        #region Private Fields

        List<RichTextCharacter> chars = new List<RichTextCharacter>();
        List<uint> glyphs = new List<uint>();

        bool currentVisible = true;

        Color currentCharColor = Color.White;
        Color currentCharColor0 = Color.White;
        Color currentCharColor1 = Color.White;
        Color currentCharColor2 = Color.White;
        Color currentCharColor3 = Color.White;

        Color currentShadowColor = Color.Black;
        Color currentOutlineColor = Color.White;

        float currentSineAmpX = 0;
        float currentSineAmpY = 0;
        float currentSineRateX = 1;
        float currentSineRateY = 1;
        float currentSineOffsetX = 0;
        float currentSineOffsetY = 0;
        float currentOffsetAmount = 10;
        float currentShadowX = 0;
        float currentShadowY = 0;
        float currentOutlineThickness = 0;
        int currentCharOffsetX = 0;
        int currentCharOffsetY = 0;
        float currentScaleX = 1;
        float currentScaleY = 1;
        float currentAngle = 0;
        bool currentBold = false;

        float currentShakeX = 0;
        float currentShakeY = 0;

        int totalHeight = 0;

        float timer = 0;

        int textWidth = -1;
        int textHeight = -1;

        bool wordWrap = false;

        float advanceSpace;

        string cachedCleanString = "";

        string textString;
        string parsedString;

        List<float> cachedLineWidths = new List<float>();

        BaseFont font;

        int charSize = 16;

        float boundsLeft;
        float boundsTop;

        #endregion

        #region Public Fields

        /// <summary>
        /// The alignment of the text.  Left, Right, or Center.
        /// </summary>
        public TextAlign TextAlign = TextAlign.Left;

        /// <summary>
        /// The character used to mark an opening of a command.
        /// </summary>
        public char CommandOpen = '{';

        /// <summary>
        /// The character used to mark the closing of a command.
        /// </summary>
        public char CommandClose = '}';

        /// <summary>
        /// The character used to separate the command with the command value.
        /// </summary>
        public char CommandDelim = ':';

        /// <summary>
        /// Controls the spacing between each character. If set above 0 the text will use a monospacing.
        /// </summary>
        public int MonospaceWidth = -1;

        /// <summary>
        /// The default horizontal amplitude of the sine wave.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultSineAmpX;

        /// <summary>
        /// The default vertical amplitude of the sine wave.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultSineAmpY;

        /// <summary>
        /// The default horizontal rate of the sine wave.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultSineRateX = 1;

        /// <summary>
        /// The default vertical rate of the sine wave.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultSineRateY = 1;

        /// <summary>
        /// The default horizontal offset of the sine wave.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultSineOffsetX;

        /// <summary>
        /// The default vertical offset of the sine wave.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultSineOffsetY;

        /// <summary>
        /// The default amount to offset each character for sine wave related transformations.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultOffsetAmount = 10;

        /// <summary>
        /// The default X position of the text shadow.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultShadowX;

        /// <summary>
        /// The default Y position of the text shadow.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultShadowY;

        /// <summary>
        /// The default outline thickness.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultOutlineThickness;

        /// <summary>
        /// The default horizontal shaking effect.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultShakeX;

        /// <summary>
        /// The default vertical shaking effect.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultShakeY;

        /// <summary>
        /// The default character visibility.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public bool DefaultVisible = true;

        /// <summary>
        /// The default character color.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public Color DefaultCharColor = Color.White;

        /// <summary>
        /// The default color of the top left corner of each character.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public Color DefaultCharColor0 = Color.White;

        /// <summary>
        /// The default color of the top right corner of each character.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public Color DefaultCharColor1 = Color.White;

        /// <summary>
        /// The default color of the bottom right corner of each character.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public Color DefaultCharColor2 = Color.White;

        /// <summary>
        /// The default color of the bottom left corner of each character.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public Color DefaultCharColor3 = Color.White;

        /// <summary>
        /// The default shadow color.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public Color DefaultShadowColor = Color.Black;

        /// <summary>
        /// The default outline color.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public Color DefaultOutlineColor = Color.White;

        /// <summary>
        /// The default x scale of the characters.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultScaleX = 1;

        /// <summary>
        /// The default y scale of the characters.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultScaleY = 1;

        /// <summary>
        /// The default angle of the characters.
        /// Will not take effect until the string changes, or Refresh() is called.
        /// </summary>
        public float DefaultAngle = 0;

        /// <summary>
        /// The line height. 1 is 100% of the normal line height for the font.
        /// </summary>
        public float LineHeight = 1;

        /// <summary>
        /// The letter spacing. 1 is 100% of the normal letter spacing.
        /// </summary>
        public float LetterSpacing = 1;

        /// <summary>
        /// How far to offset the text rendering horizontally from the origin.
        /// </summary>
        public float OffsetX;

        /// <summary>
        /// How far to offset the text rendering vertically from the origin.
        /// </summary>
        public float OffsetY;

        /// <summary>
        /// The default config.
        /// </summary>
        public static RichTextConfig Default = new RichTextConfig();

        /// <summary>
        /// The angle interval for drawing an outline.  The smaller the number the more
        /// times the text will draw itself to make an outline.
        /// </summary>
        public int OutlineQuality = 45;

        #endregion

        #region Public Properties

        /// <summary>
        /// True if the text is using MonospaceWidth.
        /// </summary>
        public bool Monospaced {
            get { return MonospaceWidth > 0; }
        }

        /// <summary>
        /// The width of the text box.  If not set it will be automatically set.
        /// </summary>
        public int TextWidth {
            get { return textWidth; }
            set {
                textWidth = value;
                UpdateCharacterData();
            }
        }

        /// <summary>
        /// The height of the text box.  If not set it will be automatically set.
        /// </summary>
        public int TextHeight {
            get { return textHeight; }
            set {
                textHeight = value;
                UpdateCharacterData();
            }
        }

        /// <summary>
        /// The line spacing between each vertical line.
        /// </summary>
        public float LineSpacing {
            get { return font.GetLineSpacing(charSize); }
        }

        /// <summary>
        /// Determines if the text will automatically wrap.  This will not work unless TextWidth is set.
        /// </summary>
        public bool WordWrap {
            get { return wordWrap; }
            set {
                wordWrap = value;
                UpdateCharacterData();
            }
        }

        /// <summary>
        /// The font size of the text.
        /// </summary>
        public int FontSize {
            get {
                return charSize;
            }
            set {
                charSize = value;
                glyphs.Clear();

                // Force update here
                UpdateDrawable();
            }
        }

        /// <summary>
        /// True of the width was not manually set.
        /// </summary>
        public bool AutoWidth {
            get { return TextWidth < 0; }
        }

        /// <summary>
        /// True if the height was not manually set.
        /// </summary>
        public bool AutoHeight {
            get { return TextHeight < 0; }
        }

        /// <summary>
        /// The string to display stripped of all commands.
        /// </summary>
        public string CleanString {
            get {
                if (cachedCleanString != "") return cachedCleanString;
                var str = "";
                foreach (var c in chars) {
                    str += c.Character.ToString();
                }
                cachedCleanString = str;
                return str;
            }
        }

        /// <summary>
        /// The pixel width of the longest line in the displayed string.
        /// </summary>
        public float LongestLine {
            get {
                var lines = CleanString.Split('\n');
                float longest = 0;
                for (int i = 0; i < NumLines; i++) {
                    var width = GetLineWidth(i);
                    longest = Math.Max(longest, width);
                }

                return longest;
            }
        }

        /// <summary>
        /// The displayed string broken up into an array by lines.
        /// </summary>
        public string[] Lines { get; private set; }

        /// <summary>
        /// The total number of lines in the displayed string.
        /// </summary>
        public int NumLines {
            get { return Lines.Length; }
        }

        /// <summary>
        /// The string to display.  This string can contain commands to alter the text dynamically.
        /// </summary>
        public string String {
            get {
                return textString;
            }
            set {
                textString = value;
                UpdateCharacterData();

                // Force update here to set Width and Height and other stuff
                UpdateDrawable();
            }
        }

        /// <summary>
        /// The character count of the string without formatting commands.
        /// </summary>
        public int CharacterCount {
            get { return chars.Count; }
        }

        /// <summary>
        /// The top bounds of the RichText.
        /// </summary>
        public float BoundsTop {
            get {
                return boundsTop;
            }
        }

        /// <summary>
        /// The top bounds of the RichText.
        /// </summary>
        public float BoundsLeft {
            get {
                return boundsLeft;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new RichText object.
        /// </summary>
        /// <param name="str">The string to display. This can include commands to alter text.</param>
        /// <param name="font">The file path to the font to use.</param>
        /// <param name="size">The font size to use.</param>
        /// <param name="textWidth">The width of the text box.</param>
        /// <param name="textHeight">The height of the text box.</param>
        public RichText(string str, string font = "", int size = 16, int textWidth = -1, int textHeight = -1)
            : base() {
            Initialize(str, (font == "" ? new Font() : new Font(font)), size, textWidth, textHeight);
        }

        /// <summary>
        /// Create a new RichText object.
        /// </summary>
        /// <param name="str">The string to display. This can include commands to alter text.</param>
        /// <param name="font">The stream of the font to use.</param>
        /// <param name="size">The font size to use.</param>
        /// <param name="textWidth">The width of the text box.</param>
        /// <param name="textHeight">The height of the text box.</param>
        public RichText(string str, Stream font, int size = 16, int textWidth = -1, int textHeight = -1)
            : base() {
            Initialize(str, new Font(font), size, textWidth, textHeight);
        }

        public RichText(string str, BaseFont font, int size = 16, int textWidth = -1, int textHeight = -1) : base() {
            Initialize(str, font, size, textWidth, textHeight);
        }

        /// <summary>
        /// Create a new RichText object using a RichTextConfig.
        /// </summary>
        /// <param name="str">The starting default text.</param>
        /// <param name="config">The config to set all the default style values.</param>
        /// <param name="textWidth">The width of the text box.</param>
        /// <param name="textHeight">The height of the text box.</param>
        public RichText(string str, RichTextConfig config, int textWidth = -1, int textHeight = -1) {
            // I probably should've used a dictionary of values or something.
            if (config == null) config = Default;

            DefaultSineAmpX = config.SineAmpX;
            DefaultSineAmpY = config.SineAmpY;
            DefaultSineRateX = config.SineRateX;
            DefaultSineRateY = config.SineRateY;
            DefaultSineOffsetX = config.SineOffsetX;
            DefaultSineOffsetY = config.SineOffsetY;
            DefaultOffsetAmount = config.OffsetAmount;
            DefaultShadowX = config.ShadowX;
            DefaultShadowY = config.ShadowY;
            DefaultOutlineThickness = config.OutlineThickness;
            DefaultShakeX = config.ShakeX;
            DefaultShakeY = config.ShakeY;
            DefaultCharColor = config.CharColor;
            DefaultCharColor0 = config.CharColor0;
            DefaultCharColor1 = config.CharColor1;
            DefaultCharColor2 = config.CharColor2;
            DefaultCharColor3 = config.CharColor3;
            DefaultShadowColor = config.ShadowColor;
            DefaultOutlineColor = config.OutlineColor;
            DefaultScaleX = config.ScaleX;
            DefaultScaleY = config.ScaleY;
            DefaultAngle = config.Angle;

            LineHeight = config.LineHeight;
            LetterSpacing = config.LetterSpacing;
            MonospaceWidth = config.MonospaceWidth;
            TextAlign = config.TextAlign;
            OffsetX = config.OffsetX;
            OffsetY = config.OffsetY;

            if (config.String != "") {
                str = config.String;
            }
            if (config.TextWidth != -1) {
                textWidth = config.TextWidth;
            }
            if (config.TextHeight != -1) {
                textHeight = config.TextHeight;
            }
            if (config.Font == null) {
                config.Font = new Font();
            }

            Initialize(str, config.Font, config.FontSize, textWidth, textHeight);
        }

        /// <summary>
        /// Create a new RichText object.
        /// </summary>
        /// <param name="str">The string to display.</param>
        /// <param name="size">The size of the font.</param>
        public RichText(string str, int size) : this(str, "", size) { }

        /// <summary>
        /// Create a new RichText object.
        /// </summary>
        /// <param name="size">The size of the font.</param>
        public RichText(int size = 16) : this("", "", size) { }

        /// <summary>
        /// Create a new RichText object
        /// </summary>
        /// <param name="config">The RichTextConfig to use.</param>
        public RichText(RichTextConfig config) : this("", config) { } 

        #endregion

        #region Private Methods

        void Initialize(string str, BaseFont font, int size, int textWidth, int textHeight) {
            Dynamic = true;

            this.font = font;

            charSize = size;
            TextWidth = textWidth;
            TextHeight = textHeight;
            String = str;
            roundRendering = false;

            if (font is BitmapFont) {
                charSize = 0; // Char size shouldn't matter for bitmap fonts?
            }
        }

        float Advance(Glyph glyph) {
            if (Monospaced) return MonospaceWidth;
            // Note: This is to fix an issue before SFML updates to 2.3!!
            var bytes = BitConverter.GetBytes(glyph.Advance);
            return BitConverter.ToSingle(bytes, 0);
        }

        Glyph Glyph(char charCode) {
            var g = font.GetGlyph(charCode, charSize, currentBold);

            //update otter texture because SFML font texture updates
            if (!glyphs.Contains(charCode)) {
                SetTexture(font.GetTexture(charSize));
                glyphs.Add(charCode);
            }

            return g;
        }

        void PrecalculateLineWidths() {
            if (cachedLineWidths.Count > 0) return; // Already calculated, will reset when string changes.
            //Console.WriteLine("Precalculate line width go");
            int lineNumber = 0;
            foreach (var line in Lines) {
                cachedLineWidths.Add(GetLineWidth(lineNumber));
                lineNumber++;
            }
            //Console.WriteLine("cached line width count {0}", cachedLineWidths.Count);
        }

        void ApplyCommand(string command, string args) {
            switch (command) {
                case "color":
                    currentCharColor = new Color(args);
                    break;
                case "color0":
                    currentCharColor0 = new Color(args);
                    break;
                case "color1":
                    currentCharColor1 = new Color(args);
                    break;
                case "color2":
                    currentCharColor2 = new Color(args);
                    break;
                case "color3":
                    currentCharColor3 = new Color(args);
                    break;
                case "colorShadow":
                    currentShadowColor = new Color(args);
                    break;
                case "colorOutline":
                    currentOutlineColor = new Color(args);
                    break;
                case "outline":
                    currentOutlineThickness = float.Parse(args);
                    break;
                case "shakeX":
                    currentShakeX = float.Parse(args);
                    break;
                case "shakeY":
                    currentShakeY = float.Parse(args);
                    break;
                case "shake":
                    currentShakeX = float.Parse(args);
                    currentShakeY = float.Parse(args);
                    break;
                case "waveAmpX":
                    currentSineAmpX = float.Parse(args);
                    break;
                case "waveAmpY":
                    currentSineAmpY = float.Parse(args);
                    break;
                case "waveAmp":
                    currentSineAmpX = float.Parse(args);
                    currentSineAmpY = float.Parse(args);
                    break;
                case "waveRateX":
                    currentSineRateX = float.Parse(args);
                    break;
                case "waveRateY":
                    currentSineRateY = float.Parse(args);
                    break;
                case "waveRate":
                    currentSineRateX = float.Parse(args);
                    currentSineRateY = float.Parse(args);
                    break;
                case "waveOffsetX":
                    currentSineOffsetX = float.Parse(args);
                    break;
                case "waveOffsetY":
                    currentSineOffsetY = float.Parse(args);
                    break;
                case "waveOffset":
                    currentSineOffsetX = float.Parse(args);
                    currentSineOffsetY = float.Parse(args);
                    break;
                case "shadowX":
                    currentShadowX = float.Parse(args);
                    break;
                case "shadowY":
                    currentShadowY = float.Parse(args);
                    break;
                case "shadow":
                    currentShadowX = float.Parse(args);
                    currentShadowY = float.Parse(args);
                    break;
                case "offset":
                    currentOffsetAmount = float.Parse(args);
                    break;
                case "bold":
                    currentBold = int.Parse(args) > 0;
                    break;
                case "charOffsetX":
                    currentCharOffsetX = int.Parse(args);
                    break;
                case "charOffsetY":
                    currentCharOffsetY = int.Parse(args);
                    break;
                case "scaleX":
                    currentScaleX = float.Parse(args);
                    break;
                case "scaleY":
                    currentScaleY = float.Parse(args);
                    break;
                case "angle":
                    currentAngle = float.Parse(args);
                    break;
            }
        }

        void UpdateCharacterData() {
            parsedString = textString;

            if (textString == "") {
                chars.Clear();
                return;
            }

            var oldStringLength = chars.Count;
            cachedCleanString = ""; // Clear clean string cache
            cachedLineWidths.Clear();
            Clear();

            if (string.IsNullOrEmpty(textString)) textString = "";

            var writingText = true;

            //auto word wrap string on input, before parsing?
            if (!AutoWidth && WordWrap) {
                textString = PreWrap(textString);
            }

            writingText = true;

            //create the set of chars with properties and parse commands
            var charIndex = 0;
            for (var i = 0; i < textString.Length; i++) {
                var c = textString[i];
                if (c == CommandOpen) {
                    //scan for commandclose
                    var cmdEnd = textString.IndexOf(CommandClose, i + 1);
                    if (cmdEnd >= 0) {
                        //only continue of command close character is found
                        writingText = false;
                        var cmd = textString.Substring(i + 1, cmdEnd - i - 1);
                        var cmdSplit = cmd.Split(CommandDelim);
                        var command = cmdSplit[0];
                        if (command == "clear") {
                            Clear();
                        }
                        else if (command == "style") {
                            var args = cmdSplit[1];
                            if (styles.ContainsKey(args)) {
                                var stylestring = styles[args];

                                var styleSplit = stylestring.Split(',');
                                foreach (var str in styleSplit) {
                                    var styleStrSplit = str.Split(CommandDelim);

                                    ApplyCommand(styleStrSplit[0], styleStrSplit[1]);
                                }
                            }
                        }
                        else {
                            ApplyCommand(command, cmdSplit[1]);
                        }

                        continue;
                    }
                }
                if (c == CommandClose) {
                    writingText = true;
                    continue;
                }
                if (writingText) {
                    RichTextCharacter rtchar;
                    if (chars.Count > charIndex) {
                        rtchar = chars[charIndex];
                        rtchar.Character = c;
                    }
                    else {
                        rtchar = new RichTextCharacter(c, charIndex);
                        chars.Add(rtchar);
                    }

                    rtchar.Timer = timer;

                    rtchar.sineAmpX = currentSineAmpX;
                    rtchar.sineAmpY = currentSineAmpY;
                    rtchar.sineRateX = currentSineRateX;
                    rtchar.sineRateY = currentSineRateY;
                    rtchar.sineOffsetX = currentSineOffsetX;
                    rtchar.sineOffsetY = currentSineOffsetY;
                    rtchar.offsetAmount = currentOffsetAmount;
                    rtchar.shadowX = currentShadowX;
                    rtchar.shadowY = currentShadowY;
                    rtchar.shadowColor = currentShadowColor;
                    rtchar.outlineThickness = currentOutlineThickness;
                    rtchar.outlineColor = currentOutlineColor;
                    rtchar.color = currentCharColor;
                    rtchar.color0 = currentCharColor0;
                    rtchar.color1 = currentCharColor1;
                    rtchar.color2 = currentCharColor2;
                    rtchar.color3 = currentCharColor3;
                    rtchar.shakeX = currentShakeX;
                    rtchar.shakeY = currentShakeY;
                    rtchar.textureOffsetX = currentCharOffsetX;
                    rtchar.textureOffsetY = currentCharOffsetY;
                    rtchar.scaleX = currentScaleX;
                    rtchar.scaleY = currentScaleY;
                    rtchar.angle = currentAngle;
                    
                    charIndex++;
                }
            }

            if (charIndex < oldStringLength) {
                chars.RemoveRange(charIndex, oldStringLength - charIndex);
            }

            Lines = CleanString.Split('\n');

            totalHeight = (int)Math.Ceiling(NumLines * font.GetLineSpacing(charSize) * LineHeight);
        }

        void Clear() {
            currentVisible = DefaultVisible;
            currentCharColor = DefaultCharColor;
            currentCharColor0 = DefaultCharColor0;
            currentCharColor1 = DefaultCharColor1;
            currentCharColor2 = DefaultCharColor2;
            currentCharColor3 = DefaultCharColor3;
            currentShadowColor = DefaultShadowColor;
            currentOutlineColor = DefaultOutlineColor;
            currentOutlineThickness = DefaultOutlineThickness;
            currentShadowX = DefaultShadowX;
            currentShadowY = DefaultShadowY;
            currentShakeX = DefaultShakeX;
            currentShakeY = DefaultShakeY;
            currentSineAmpX = DefaultSineAmpX;
            currentSineAmpY = DefaultSineAmpY;
            currentSineOffsetX = DefaultSineOffsetX;
            currentSineOffsetY = DefaultSineOffsetY;
            currentSineRateX = DefaultSineRateX;
            currentSineRateY = DefaultSineRateY;
            currentOffsetAmount = DefaultOffsetAmount;
            currentCharOffsetX = 0;
            currentCharOffsetY = 0;
            currentScaleX = 1;
            currentScaleY = 1;
            currentAngle = 0;
        }

        protected override void UpdateDrawable() {
            base.UpdateDrawable();

            SFMLVertices.Clear();

            // No precalculate for now, it just doubles the loops I think.
            //PrecalculateLineWidths();

            advanceSpace = Advance(Glyph(' ')); //Figure out space ahead of time.

            float x = 0;
            float y = 0;

            int currentLine = 0;

            x = LineStartPosition(currentLine) + OffsetX;
            y = charSize + OffsetY;

            float lineLength = 0;

            float maxX = 0;
            float maxY = 0;
            float minY = charSize;
            float minX = charSize;

            var quadCount = 0;

            var buildLineCache = false;

            if (cachedLineWidths.Count == 0) {
                //build cached line widths this draw.
                //cache will be cleared on string change
                buildLineCache = true;
            }

            char prevChar = (char)0;

            for (var i = 0; i < chars.Count; i++) {

                var c = chars[i].Character;

                if (c == ' ' || c == '\t' || c == '\n') {

                    minX = Util.Min(minX, x - LineStartPosition(currentLine));
                    minY = Util.Min(minY, y);

                    switch (c) {
                        case ' ':
                            x += advanceSpace * LetterSpacing;
                            lineLength += advanceSpace * LetterSpacing;
                            break;
                        case '\t':
                            x += advanceSpace * 4 * LetterSpacing;
                            lineLength += advanceSpace * 4 * LetterSpacing;
                            break;
                        case '\n':
                            if (buildLineCache) cachedLineWidths.Add(lineLength);
                            lineLength = 0;

                            y += LineSpacing * LineHeight;

                            currentLine++;
                            x = LineStartPosition(currentLine);
                            break;
                    }

                    maxX = Util.Max(maxX, x - LineStartPosition(currentLine));
                    maxY = Util.Max(maxY, y);

                }
                else {
                    var glyph = Glyph(c);
                    var rect = glyph.TextureRect;
                    var bounds = glyph.Bounds;

                    // Char offset only works with default formatted bitmap fonts!!
                    rect.Top += chars[i].TextureOffsetY;
                    rect.Left += chars[i].TextureOffsetX;

                    // This is how you do kerning I guess
                    if (!Monospaced)
                        x += font.GetKerning(prevChar, chars[i].Character, FontSize);

                    var cx = chars[i].OffsetX;
                    var cy = chars[i].OffsetY;

                    var left = bounds.Left;
                    var right = bounds.Left + bounds.Width;
                    var top = bounds.Top;
                    var bottom = bounds.Top + bounds.Height;

                    var x1y1 = new Vector2(cx + x + left, cy + y + top);
                    var x2y1 = new Vector2(cx + x + right, cy + y + top);
                    var x2y2 = new Vector2(cx + x + right, cy + y + bottom);
                    var x1y2 = new Vector2(cx + x + left, cy + y + bottom);

                    var u1 = rect.Left;
                    var v1 = rect.Top;
                    var u2 = rect.Left + rect.Width;
                    var v2 = rect.Top + rect.Height;

                    var charCenterX = cx + x + bounds.Left + bounds.Width / 2;
                    var charCenterY = cy + y + bounds.Top + bounds.Height / 2;

                    var charCenter = new Vector2(charCenterX, charCenterY);

                    var scaleX = chars[i].ScaleX;
                    var scaleY = chars[i].ScaleY;
                    var angle = chars[i].Angle;

                    // Scale the character verticies
                    x1y1 = Util.ScaleAround(x1y1, charCenter, scaleX, scaleY);
                    x1y2 = Util.ScaleAround(x1y2, charCenter, scaleX, scaleY);
                    x2y2 = Util.ScaleAround(x2y2, charCenter, scaleX, scaleY);
                    x2y1 = Util.ScaleAround(x2y1, charCenter, scaleX, scaleY);

                    // Rotate the character verticies
                    x1y1 = Util.RotateAround(x1y1, charCenter, angle);
                    x1y2 = Util.RotateAround(x1y2, charCenter, angle);
                    x2y2 = Util.RotateAround(x2y2, charCenter, angle);
                    x2y1 = Util.RotateAround(x2y1, charCenter, angle);

                    // Draw shadow
                    Color nextColor;

                    if (chars[i].ShadowX != 0 || chars[i].ShadowY != 0) {
                        var shadowx = chars[i].ShadowX;
                        var shadowy = chars[i].ShadowY;

                        nextColor = chars[i].ShadowColor * Color;

                        Append(shadowx + x1y1.X, shadowy + x1y1.Y, nextColor, u1, v1);
                        Append(shadowx + x2y1.X, shadowy + x2y1.Y, nextColor, u2, v1);
                        Append(shadowx + x2y2.X, shadowy + x2y2.Y, nextColor, u2, v2);
                        Append(shadowx + x1y2.X, shadowy + x1y2.Y, nextColor, u1, v2);

                        quadCount++;
                    }

                    // Draw outline
                    if (chars[i].Visible) {
                        if (chars[i].OutlineThickness > 0) {
                            var outline = chars[i].OutlineThickness;
                            nextColor = chars[i].OutlineColor * Color;

                            for (float o = outline * 0.5f; o < outline; o += outline * 0.5f) {
                                for (float r = 0; r < 360; r += OutlineQuality) {
                                    var outlinex = Util.PolarX(r, o);
                                    var outliney = Util.PolarY(r, o);

                                    Append(outlinex + x1y1.X, outliney + x1y1.Y, nextColor, u1, v1);
                                    Append(outlinex + x2y1.X, outliney + x2y1.Y, nextColor, u2, v1);
                                    Append(outlinex + x2y2.X, outliney + x2y2.Y, nextColor, u2, v2);
                                    Append(outlinex + x1y2.X, outliney + x1y2.Y, nextColor, u1, v2);

                                    quadCount++;
                                }
                            }
                        }

                        // Draw character
                        nextColor = chars[i].Color.Copy() * Color;
                        nextColor *= chars[i].Color0;

                        Append(x1y1.X, x1y1.Y, nextColor, u1, v1);

                        nextColor = chars[i].Color.Copy() * Color;
                        nextColor *= chars[i].Color1;

                        Append(x2y1.X, x2y1.Y, nextColor, u2, v1);

                        nextColor = chars[i].Color.Copy() * Color;
                        nextColor *= chars[i].Color2;

                        Append(x2y2.X, x2y2.Y, nextColor, u2, v2);

                        nextColor = chars[i].Color.Copy() * Color;
                        nextColor *= chars[i].Color3;

                        Append(x1y2.X, x1y2.Y, nextColor, u1, v2);

                        // Keep track of how many quads for debugging purposes
                        quadCount++;
                    }

                    // Update bounds.
                    minX = Util.Min(minX, x + left - LineStartPosition(currentLine));
                    minY = Util.Min(minY, y + top);

                    maxX = Util.Max(maxX, x + right - LineStartPosition(currentLine));
                    maxY = Util.Max(maxY, y + bottom);

                    if (Monospaced) {
                        // Note: messy fix for wiggling monospaced text. Whatever I'm done with text forever.
                        maxX = Util.Max(maxX, x + Advance(glyph) - LineStartPosition(currentLine));
                        minX = 0;
                    }

                    // Advance position
                    x += Advance(glyph) * LetterSpacing;

                    // Keep track of line length separately
                    lineLength += Advance(glyph) * LetterSpacing;

                    // Keep track of prev char for kernin'
                    prevChar = chars[i].Character;
                }
            }

            // Handle Length of final line
            if (buildLineCache) cachedLineWidths.Add(lineLength);

            // Figure out dimensions
            if (AutoWidth) {
                Width = (int)(maxX - minX);
            }
            else {
                Width = TextWidth;
            }

            if (AutoHeight) {
                Height = (int)(maxY - minY);
                Height = (int)Util.Max(Height, charSize); // Temp fix for negative height?
            }
            else {
                Height = TextHeight;
            }

            boundsLeft = minX;
            boundsTop = minY;
        }

        float LineStartPosition(int lineNumber) {
            if (TextAlign == TextAlign.Left) return 0;

            float lineStart = 0;
            var lineLength = GetLineWidth(lineNumber);
            switch (TextAlign) {
                case TextAlign.Center:
                    lineStart = (Width - lineLength) / 2;
                    break;
                case TextAlign.Right:
                    lineStart = Width - lineLength;
                    break;
            }
            return lineStart;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Center the RichText's origin. This factors in the RichText's local bounds.
        /// </summary>
        public void CenterTextOrigin() {
            CenterTextOriginX();
            CenterTextOriginY();
        }

        /// <summary>
        /// Center the RichText's Y origin.  This factors in the RichText's top bounds.
        /// </summary>
        public void CenterTextOriginY() {
            OriginY = Util.Round(HalfHeight + BoundsTop);
        }

        /// <summary>
        /// Center the RichText's X origin.  This factors in the RichText's left bounds.
        /// </summary>
        public void CenterTextOriginX() {
            OriginX = Util.Round(HalfWidth + BoundsLeft);
        }

        public override void CenterOrigin() {
            // Rounding to an int for this because origins of 0.5f cause bad text blurring.
            OriginX = Util.Round(HalfWidth);
            OriginY = Util.Round(HalfHeight);
        }

        /// <summary>
        /// Insert new lines into a string to prepare it for word wrapping with this object's width.
        /// This function will not wrap text if AutoWidth is true!
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <returns>The wrapped string.</returns>
        public string PreWrap(string str) {
            if (AutoWidth) return str; //Auto width cannot auto word wrap.

            var finalStr = str;

            var writingText = true;

            float pixels = 0;
            int lastSpaceIndex = 0;

            for (var i = 0; i < str.Length; i++) {
                var c = str[i];
                var glyph = Glyph(c);

                if (c == CommandOpen) {
                    var cmdEnd = str.IndexOf(CommandClose, i + 1);
                    if (cmdEnd >= 0) {
                        writingText = false;
                    }
                }
                if (!writingText) {
                    if (c == CommandClose) {
                        writingText = true;
                    }
                }
                else if (writingText) {
                    if (c == '\t') {
                        pixels += Advance(glyph) * 4;
                    }
                    else if (c == '\n') {
                        pixels = 0;
                    }
                    else {
                        pixels += Advance(glyph);

                        if (c == ' ') {
                            // Keep track of the last space.
                            lastSpaceIndex = i;
                        }
                        if (pixels > TextWidth) {
                            StringBuilder sb = new StringBuilder(finalStr);

                            // Turn last space into new line if pixels exceeds allowed width
                            if (lastSpaceIndex < sb.Length) {
                                sb[lastSpaceIndex] = '\n';
                            }

                            finalStr = sb.ToString();

                            // Return the loop to the new line.
                            i = lastSpaceIndex;

                            // Reset the current pixel width.
                            pixels = 0;
                        }
                    }
                }
            }

            return finalStr;
        }

        /// <summary>
        /// The line width in pixels of a specific line.
        /// </summary>
        /// <param name="lineNumber">The line number to check.</param>
        /// <returns>The length of the line in pixels.</returns>
        public float GetLineWidth(int lineNumber) {
            if (lineNumber < 0 || lineNumber >= NumLines) throw new ArgumentOutOfRangeException("Line doesn't exist in string!");
            if (lineNumber < cachedLineWidths.Count) return cachedLineWidths[lineNumber];
            
            //This is very slow on large text objects, but I'm not sure how to get around that!

            var line = Lines[lineNumber];
            float width = 0;
            foreach (var c in line) {
                var glyph = Glyph(c);
                if (c == '\t') {
                    width += (Advance(glyph) * 3 * LetterSpacing);
                }
                width += (Advance(glyph) * LetterSpacing);
            }
            return width;
        }

        /// <summary>
        /// Refresh the text.  This will reapply all commands and update the text image.
        /// </summary>
        public void Refresh() {
            UpdateCharacterData();
        }

        /// <summary>
        /// Update the RichText.
        /// </summary>
        public override void Update() {
            timer += Game.Instance.DeltaTime;

            foreach (var c in chars) {
                c.Update();
            }

            base.Update();
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <typeparam name="T">The specific type of Font.</typeparam>
        /// <returns>The font as type font type T.</returns>
        public T GetFont<T>() where T : BaseFont {
            return (T)font;
        }

        #endregion

        /// <summary>
        /// Retrieve the RichTextCharacter from the string.
        /// </summary>
        /// <param name="index">The index of the character.</param>
        /// <returns>The RichTextCharacter at that index in the RichText string.</returns>
        public RichTextCharacter this[int index] {
            get { return chars[index]; }
            set { chars[index] = value; }
        }

        /// <summary>
        /// Get a list of RichTextCharacter
        /// </summary>
        public List<RichTextCharacter> Characters {
            get { return chars; }
        }

    }

    #region Enum

    public enum TextAlign {
        Left,
        Right,
        Center
    }

    #endregion

    /// <summary>
    /// Internal class for managing characters in RichText.
    /// </summary>
    public class RichTextCharacter {

        #region Private Fields

        float finalShakeX;
        float finalShakeY;
        float finalSinX;
        float finalSinY;
        
        float activeScaleX = 1;
        float activeScaleY = 1;
        float activeAngle;
        float activeX;
        float activeY;
        float activeSineAmpX;
        float activeSineAmpY;
        float activeSineRateX;
        float activeSineRateY;
        float activeSineOffsetX;
        float activeSineOffsetY;
        float activeShakeX;
        float activeShakeY;
        float activeShadowX;
        float activeShadowY;
        float activeOutlineThickness;
        int activeTextureOffsetX;
        int activeTextureOffsetY;
        float activeOffsetAmount;
        bool activeVisible = true;
        Color activeColor = Color.White;
        Color activeColor0 = Color.White;
        Color activeColor1 = Color.White;
        Color activeColor2 = Color.White;
        Color activeColor3 = Color.White;
        Color activeShadowColor = Color.White;
        Color activeOutlineColor = Color.White;

        #endregion

        #region Public Fields

        /// <summary>
        /// The character.
        /// </summary>
        public char Character;

        /// <summary>
        /// Timer used for animation.
        /// </summary>
        public float Timer;

        /// <summary>
        /// The sine wave offset for this specific character.
        /// </summary>
        public float CharOffset;

        /// <summary>
        /// Determines if the character is bold.  Not supported yet.
        /// </summary>
        public bool Bold = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// If the character is visible.
        /// </summary>
        public bool Visible {
            get { return visible && activeVisible; }
            set { activeVisible = value; }
        }

        /// <summary>
        /// The Color of the character.
        /// </summary>
        public Color Color {
            get { return color * activeColor; }
            set { activeColor = value; }
        }

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color Color0 {
            get { return color0 * activeColor0; }
            set { activeColor0 = value; }
        }

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color Color1 {
            get { return color1 * activeColor1; }
            set { activeColor1 = value; }
        }

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color Color2 {
            get { return color2 * activeColor2; }
            set { activeColor2 = value; }
        }

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color Color3 {
            get { return color3 * activeColor3; }
            set { activeColor3 = value; }
        }

        /// <summary>
        /// The Color of the shadow.
        /// </summary>
        public Color ShadowColor {
            get { return shadowColor * activeShadowColor; }
            set { activeShadowColor = value; }
        }

        /// <summary>
        /// The Color of the outline.
        /// </summary>
        public Color OutlineColor {
            get { return outlineColor * activeShadowColor; }
            set { activeShadowColor = value; }
        }

        /// <summary>
        /// The offset amount for each character.
        /// </summary>
        public float OffsetAmount {
            get { return offsetAmount + activeOffsetAmount; }
            set { activeOffsetAmount = value; }
        }

        /// <summary>
        /// The horizontal texture offset of the character.  BitmapFont only.
        /// </summary>
        public int TextureOffsetX {
            get { return textureOffsetX + activeTextureOffsetX; }
            set { activeTextureOffsetX = value; }
        }

        /// <summary>
        /// The vertical texture offset of the character.  BitmapFont only.
        /// </summary>
        public int TextureOffsetY {
            get { return textureOffsetY + activeTextureOffsetY; }
            set { activeTextureOffsetY = value; }
        }

        /// <summary>
        /// The outline thickness.
        /// </summary>
        public float OutlineThickness {
            get { return outlineThickness + activeOutlineThickness; }
            set { activeOutlineThickness = value; }
        }

        /// <summary>
        /// The horizontal sine wave offset.
        /// </summary>
        public float SineOffsetX {
            get { return sineOffsetX + activeSineOffsetX; }
            set { activeSineOffsetX = value; }
        }

        /// <summary>
        /// The vertical sine wave offset.
        /// </summary>
        public float SineOffsetY {
            get { return sineOffsetY + activeSineOffsetY; }
            set { activeSineOffsetY = value; }
        }

        /// <summary>
        /// The X position of the shadow.
        /// </summary>
        public float ShadowX {
            get { return shadowX + activeShadowX; }
            set { activeShadowX = value; }
        }

        /// <summary>
        /// The Y position of the shadow.
        /// </summary>
        public float ShadowY {
            get { return shadowY + activeShadowY; }
            set { activeShadowY = value; }
        }

        /// <summary>
        /// The horizontal sine wave rate.
        /// </summary>
        public float SineRateX {
            get { return sineRateX + activeSineRateX; }
            set { activeSineRateX = value; }
        }

        /// <summary>
        /// The vertical sine wave rate.
        /// </summary>
        public float SineRateY {
            get { return sineRateY + activeSineRateY; }
            set { activeSineRateY = value; }
        }

        /// <summary>
        /// The amount of horizontal shake.
        /// </summary>
        public float ShakeX {
            get { return shakeX + activeShakeX; }
            set { activeShakeX = value; }
        }

        /// <summary>
        /// The amount of vertical shake.
        /// </summary>
        public float ShakeY {
            get { return shakeY + activeShakeY; }
            set { activeShakeY = value; }
        }

        /// <summary>
        /// The horizontal sine wave amplitude.
        /// </summary>
        public float SineAmpX {
            get { return sineAmpX + activeSineAmpX; }
            set { activeSineAmpX = value; }
        }

        /// <summary>
        /// The vertical sine wave amplitude.
        /// </summary>
        public float SineAmpY {
            get { return sineAmpY + activeSineAmpY; }
            set { activeSineAmpY = value; }
        }

        /// <summary>
        /// The X scale of the character.
        /// </summary>
        public float ScaleX {
            get { return scaleX * activeScaleX; }
            set { activeScaleX = value; }
        }

        /// <summary>
        /// The Y scale of the character.
        /// </summary>
        public float ScaleY {
            get { return scaleY * activeScaleY; }
            set { activeScaleY = value; }
        }

        /// <summary>
        /// The angle of the character.
        /// </summary>
        public float Angle {
            get { return angle + activeAngle; }
            set { activeAngle = value; }
        }

        /// <summary>
        /// The X position offset of the character.
        /// </summary>
        public float X {
            get { return x + activeX; }
            set { activeX = value; }
        }

        /// <summary>
        /// The Y position offset of the character.
        /// </summary>
        public float Y {
            get { return y + activeY; }
            set { activeY = value; }
        }

        /// <summary>
        /// The final horizontal offset position of the character when rendered.
        /// </summary>
        public float OffsetX {
            get {
                return finalShakeX + finalSinX + X;
            }
        }

        /// <summary>
        /// The final vertical offset position of the character when rendered.
        /// </summary>
        public float OffsetY {
            get {
                return finalShakeY + finalSinY + Y;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new RichTextCharacter.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="charOffset">The character offset for animation.</param>
        public RichTextCharacter(char character, int charOffset = 0) {
            Character = character;
            CharOffset = charOffset;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the character.
        /// </summary>
        public void Update() {
            Timer += Game.Instance.DeltaTime;

            finalShakeX = Rand.Float(-ShakeX, ShakeX);
            finalShakeY = Rand.Float(-ShakeY, ShakeY);
            finalSinX = Util.SinScale((Timer + SineOffsetX - CharOffset * OffsetAmount) * SineRateX, -SineAmpX, SineAmpX);
            finalSinY = Util.SinScale((Timer + SineOffsetY - CharOffset * OffsetAmount) * SineRateY, -SineAmpY, SineAmpY);
        }

        #endregion

        #region Internal

        internal void Append(VertexArray vertices, float x, float y) {
            var col = new Color(Color0);
            col.R *= Color.R;
            col.G *= Color.G;
            col.B *= Color.B;
            col.A *= Color.A;
        }

        internal float scaleX = 1;
        internal float scaleY = 1;
        internal float angle;
        internal float x;
        internal float y;
        internal float sineAmpX;
        internal float sineAmpY;
        internal float sineRateX = 1;
        internal float sineRateY = 1;
        internal float sineOffsetX;
        internal float sineOffsetY;
        internal float shakeX;
        internal float shakeY;
        internal float shadowX;
        internal float shadowY;
        internal float outlineThickness;
        internal int textureOffsetX;
        internal int textureOffsetY;
        internal float offsetAmount = 10;
        internal bool visible = true;
        internal Color color = Color.White;
        internal Color color0 = Color.White;
        internal Color color1 = Color.White;
        internal Color color2 = Color.White;
        internal Color color3 = Color.White;
        internal Color shadowColor = Color.Black;
        internal Color outlineColor = Color.White;

        #endregion
        
    }

    /// <summary>
    /// A utility class used for storing default values for a RichText object.
    /// Set the values by using "var config = new RichTextConfig() { Font = "MyFont.ttf", FontSize = 16, ... };"
    /// </summary>
    public class RichTextConfig {

        #region Public Fields

        /// <summary>
        /// The horizontal sine wave amplitude.
        /// </summary>
        public float SineAmpX = 0;

        /// <summary>
        /// The vertical sine wave amplitude.
        /// </summary>
        public float SineAmpY = 0;

        /// <summary>
        /// The horizontal sine wave rate.
        /// </summary>
        public float SineRateX = 1;

        /// <summary>
        /// The vertical sine wave rate.
        /// </summary>
        public float SineRateY = 1;

        /// <summary>
        /// The horizontal sine wave offset.
        /// </summary>
        public float SineOffsetX = 0;

        /// <summary>
        /// The vertical sine wave offset.
        /// </summary>
        public float SineOffsetY = 0;

        /// <summary>
        /// The offset amount for each character for sine wave related transformations.
        /// </summary>
        public float OffsetAmount = 10;

        /// <summary>
        /// The X position of the shadow.
        /// </summary>
        public float ShadowX = 0;

        /// <summary>
        /// The Y position of the shadow.
        /// </summary>
        public float ShadowY = 0;

        /// <summary>
        /// The outline thickness.
        /// </summary>
        public float OutlineThickness = 0;

        /// <summary>
        /// The amount of horizontal shake.
        /// </summary>
        public float ShakeX = 0;

        /// <summary>
        /// The amount of vertical shake.
        /// </summary>
        public float ShakeY = 0;

        /// <summary>
        /// If the character is visible.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// The Color of the character.
        /// </summary>
        public Color CharColor = Color.White;

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color CharColor0 = Color.White;

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color CharColor1 = Color.White;

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color CharColor2 = Color.White;

        /// <summary>
        /// The Color of the top left corner.
        /// </summary>
        public Color CharColor3 = Color.White;

        /// <summary>
        /// The Color of the shadow.
        /// </summary>
        public Color ShadowColor = Color.Black;

        /// <summary>
        /// The Color of the outline.
        /// </summary>
        public Color OutlineColor = Color.White;

        /// <summary>
        /// The X offset of the character.  BitmapFont only.
        /// </summary>
        public int CharOffsetX = 0;

        /// <summary>
        /// The Y offset of the character.  BitmapFont only.
        /// </summary>
        public int CharOffsetY = 0;

        /// <summary>
        /// The X scale of the character.
        /// </summary>
        public float ScaleX = 1;

        /// <summary>
        /// The Y scale of the character.
        /// </summary>
        public float ScaleY = 1;

        /// <summary>
        /// The angle of the character.
        /// </summary>
        public float Angle = 0;

        /// <summary>
        /// The spacing between each character.
        /// </summary>
        public float LetterSpacing = 1.0f;

        /// <summary>
        /// The line height between each line. Default is 1.
        /// </summary>
        public float LineHeight = 1.0f;

        /// <summary>
        /// Controls the spacing between each character. If set above 0 the text will use a monospacing.
        /// </summary>
        public int MonospaceWidth = -1;

        /// <summary>
        /// The alignment of the text.  Left, Right, or Center.
        /// </summary>
        public TextAlign TextAlign = TextAlign.Left;

        /// <summary>
        /// The font to use.
        /// </summary>
        public BaseFont Font;

        /// <summary>
        /// The font size.
        /// </summary>
        public int FontSize = 16;

        /// <summary>
        /// The string to display.
        /// </summary>
        public string String = "";

        /// <summary>
        /// The width of the text block.
        /// </summary>
        public int TextWidth = -1;

        /// <summary>
        /// The height of the text block.
        /// </summary>
        public int TextHeight = -1;

        /// <summary>
        /// How far to offset the text rendering horizontally from the origin.
        /// </summary>
        public float OffsetX;

        /// <summary>
        /// How far to offset the text rendering vertically from the origin.
        /// </summary>
        public float OffsetY;

        #endregion
        
    }
}
