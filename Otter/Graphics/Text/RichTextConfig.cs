namespace Otter.Graphics.Text
{
    /// <summary>
    /// A utility class used for storing default values for a RichText object.
    /// Set the values by using "var config = new RichTextConfig() { Font = "MyFont.ttf", FontSize = 16, ... };"
    /// </summary>
    public class RichTextConfig
    {
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
