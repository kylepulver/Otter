using System;

namespace Otter {
    /// <summary>
    /// Graphic that renders part of a sprite sheet, but does not automatically animate it at all.
    /// </summary>
    public class ImageSet : Image {

        #region Private Fields

        int frame = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of rows on the image sheet.
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// The number of columns on the image sheet.
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// The number of frames on the image sheet.
        /// </summary>
        public int Frames { get; private set; }

        /// <summary>
        /// The frame to render from the image set.
        /// </summary>
        public int Frame {
            get { return frame; }
            set {
                frame = (int)Util.Clamp(value, 0, Frames - 1);
                UpdateTextureRegion(frame);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new ImageSet from a file path for a texture.
        /// </summary>
        /// <param name="source">The file path to the texture to use for the image sheet.</param>
        /// <param name="width">The width of each cell on the image sheet.</param>
        /// <param name="height">The height of each cell on the image sheet.</param>
        public ImageSet(string source, int width, int height) : base(source) {
            Initialize(width, height);
        }

        /// <summary>
        /// Create a new ImageSet from a Texture.
        /// </summary>
        /// <param name="texture">The Texture to use for the image sheet.</param>
        /// <param name="width">The width of each cell on the image sheet.</param>
        /// <param name="height">The height of each cell on the image sheet.</param>
        public ImageSet(Texture texture, int width, int height) : base(texture) {
            Initialize(width, height);
        }

        /// <summary>
        /// Create a new ImageSet from an AtlasTexture.
        /// </summary>
        /// <param name="texture">The AtlasTexture to use for the image sheet.</param>
        /// <param name="width">The width of each cell on the image sheet.</param>
        /// <param name="height">The height of each cell on the image sheet.</param>
        public ImageSet(AtlasTexture texture, int width, int height) : base(texture) {
            Initialize(width, height);
        }

        #endregion

        #region Private Methods

        void Initialize(int width, int height) {
            Width = width;
            Height = height;

            ClippingRegion = new Rectangle(0, 0, Width, Height);

            // Proper sprite batching coming soon.
            //Batchable = true;

            Columns = (int)Math.Ceiling((float)TextureRegion.Width / width);
            Rows = (int)Math.Ceiling((float)TextureRegion.Height / height);

            Frames = Columns * Rows;

            UpdateTextureRegion(0);
        }

        /// <summary>
        /// Updates the internal source for the texture.
        /// </summary>
        /// <param name="frame">The frame in terms of the sprite sheet.</param>
        void UpdateTextureRegion(int frame) {
            var top = (int)(Math.Floor((float)frame / Columns) * Height);
            var left = (int)((frame % Columns) * Width);

            if (TextureRegion != new Rectangle(left, top, Width, Height)) {
                NeedsUpdate = true;
            }

            TextureRegion = new Rectangle(left, top, Width, Height);
        }

        #endregion
        
    }
}
