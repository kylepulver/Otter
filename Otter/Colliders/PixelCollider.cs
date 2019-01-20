using System;
namespace Otter {
    /// <summary>
    /// Collider that can use an image as a mask.  This is not recommended to use for most cases as it can
    /// be pretty expensive to process.
    /// </summary>
    public class PixelCollider : Collider {

        #region Public Fields

        /// <summary>
        /// The amount of Alpha a pixel needs to exceed to register as a collision.
        /// If 0, any pixel with an alpha above 0 will register as collidable.
        /// </summary>
        public float Threshold = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// The byte array of pixels.
        /// </summary>
        public byte[] Pixels {
            get { return collideImage.Pixels; }
        }

        #endregion

        #region Private Fields

        SFML.Graphics.Image collideImage;
        SFML.Graphics.Texture texture;
        Texture visibleTexture;
        Graphic visibleImage;

        bool rendered;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a pixel collider.
        /// </summary>
        /// <param name="source">The source image to create the collider from.</param>
        /// <param name="tags">The tags to register the collider with.</param>
        public PixelCollider(string source, params int[] tags) {
            texture = Textures.Load(source);
            collideImage = texture.CopyToImage();
            
            Width = texture.Size.X;
            Height = texture.Size.Y;

            AddTag(tags);
        }

        public PixelCollider(Texture texture, params int[] tags) {
            this.texture = texture.SFMLTexture;
            collideImage = this.texture.CopyToImage();

            Width = this.texture.Size.X;
            Height = this.texture.Size.Y;

            AddTag(tags);
        }

        public PixelCollider(string source, Enum tag, params Enum[] tags) : this(source) {
            AddTag(tag);
            AddTag(tags);
        }

        public PixelCollider(Texture texture, Enum tag, params Enum[] tags) : this(texture) {
            AddTag(tag);
            AddTag(tags);
        }

        #endregion

        #region Private Methods

        void InitializeTexture() {
            visibleTexture = new Texture((int)Width, (int)Height);

            for (var x = 0; x < Width; x++) {
                for (var y = 0; y < Height; y++) {
                    if (PixelAt(x, y)) {
                        visibleTexture.SetPixel(x, y, Color.Red);
                    }
                    else {
                        visibleTexture.SetPixel(x, y, Color.None);
                    }
                }
            }

            visibleImage = new Image(visibleTexture);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if a pixel is collidable at x, y.
        /// </summary>
        /// <param name="x">The X position of the pixel to check.</param>
        /// <param name="y">The Y position of the pixel to check.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelAt(int x, int y) {
            if (x < 0 || y < 0 || x > Width || y > Height) return false;

            if (collideImage.GetPixel((uint)x, (uint)y).A > Threshold) return true;
            return false;
        }

        /// <summary>
        /// Check if a pixel is collidable at X, Y.
        /// </summary>
        /// <param name="x">The X position of the pixel to check.</param>
        /// <param name="y">The Y position of the pixel to check.</param>
        /// <param name="threshold">The alpha threshold that should register a collision.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelAt(int x, int y, float threshold) {
            if (x < 0 || y < 0 || x > Width || y > Height) return false;

            if (collideImage.GetPixel((uint)x, (uint)y).A > threshold) return true;
            return false;
        }

        /// <summary>
        /// Check if a pixel is collideable at X - Left, Y - Top.
        /// </summary>
        /// <param name="x">The X position of the pixel to check.</param>
        /// <param name="y">The Y position of the pixel to check.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelAtRelative(int x, int y) {
            x -= (int)Left;
            y -= (int)Top;

            return PixelAt(x, y);
        }

        /// <summary>
        /// Check if a pixel is collideable at X - Left, Y - Top.
        /// </summary>
        /// <param name="x">The X position of the pixel to check.</param>
        /// <param name="y">The Y position of the pixel to check.</param>
        /// <param name="threshold">The alpha threshold that should register a collision.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelAtRelative(int x, int y, float threshold) {
            x -= (int)Left;
            y -= (int)Top;

            return PixelAt(x, y, threshold);
        }

        /// <summary>
        /// Check if any pixels in the area defined by X, Y, X2, Y2 are collideable.
        /// </summary>
        /// <param name="x">The left of the area to check.</param>
        /// <param name="y">The top of the area to check.</param>
        /// <param name="x2">The right of the area to check.</param>
        /// <param name="y2">The bottom of the area to check.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelArea(int x, int y, int x2, int y2) {
            for (var i = x; i < x2; i++) {
                for (var j = y; j < y2; j++) {
                    if (PixelAt(i, j)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if any pixels in the area defined by X, Y, X2, Y2 are collideable.
        /// </summary>
        /// <param name="x">The left of the area to check.</param>
        /// <param name="y">The top of the area to check.</param>
        /// <param name="x2">The right of the area to check.</param>
        /// <param name="y2">The bottom of the area to check.</param>
        /// <param name="threshold">The alpha threshold that should register a collision.</param>
        /// <returns>True if the pixel collides.</returns>
        public bool PixelArea(int x, int y, int x2, int y2, float threshold) {
            for (var i = x; i < x2; i++) {
                for (var j = y; j < y2; j++) {
                    if (PixelAt(i, j, threshold)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draw the collider for debug purposes.
        /// </summary>
        public override void Render(Color color = null) {
            base.Render(color);
            if (color == null) color = Color.Red;

            if (Entity == null) return;

            if (!rendered) {
                rendered = true;
                InitializeTexture();
            }

            visibleImage.Color = color;
            Draw.Graphic(visibleImage, Left, Top);
        }

        #endregion

    }
}
