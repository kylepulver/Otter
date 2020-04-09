using Otter.Utility.MonoGame;

namespace Otter.Graphics
{
    /// <summary>
    /// Class used for representing a texture on a texture atlas.
    /// </summary>
    public class AtlasTexture
    {
        #region Public Fields

        /// <summary>
        /// The X position of the texture in the atlas.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y position of the texture in the atlas.
        /// </summary>
        public int Y;

        /// <summary>
        /// The width of the texture in the atlas.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the texture in the atlas.
        /// </summary>
        public int Height;

        /// <summary>
        /// The frame width of the texture in the atlas (used in Trim mode.)
        /// Trim is not supported yet.
        /// </summary>
        public int FrameWidth;

        /// <summary>
        /// The frame height of the texture in the atlas (used in Trim mode.)
        /// Trim is not supported yet.
        /// </summary>
        public int FrameHeight;

        /// <summary>
        /// The frame X position of the texture in the atlas (used in Trim mode.)
        /// Trim is not supported yet.
        /// </summary>
        public int FrameX;

        /// <summary>
        /// The frame Y position of the texture in the atlas (used in Trim mode.)
        /// Trim is not supported yet.
        /// </summary>
        public int FrameY;

        /// <summary>
        /// The name of the texture in the atlas.
        /// </summary>
        public string Name;

        /// <summary>
        /// The file path source of the texture.
        /// </summary>
        public string Source;

        /// <summary>
        /// The texture loaded for this atlas texture.
        /// </summary>
        public Texture Texture;

        #endregion

        #region Public Properties

        /// <summary>
        /// The rectangle region of the texture in the atlas.
        /// </summary>
        public Rectangle Region
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return Name + " " + Source + " X: " + X + " Y: " + Y + " Width: " + Width + " Height: " + Height + " FrameX: " + FrameX + " FrameY: " + FrameY + " FrameWidth: " + FrameWidth + " FrameHeight: " + FrameHeight;
        }

        #endregion
    }
}
