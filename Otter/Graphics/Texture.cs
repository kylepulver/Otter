using System;
using System.IO;

namespace Otter {
    /// <summary>
    /// Class representing a texture. Can perform pixel operations on the CPU, but those will be
    /// pretty slow and shouldn't be used that much.
    /// </summary>
    public class Texture : IDisposable {

        #region Public Fields

        /// <summary>
        /// The default setting to use for smoothing textures.
        /// Much easier to set this at the start of a program rather than
        /// adjust the settings for every single texture you use.
        /// </summary>
        public static bool DefaultSmooth = false;

        #endregion Public Fields

        #region Internal Fields

        internal bool needsUpdate = false;

        internal SFML.Graphics.Texture texture;

        #endregion Internal Fields

        #region Private Fields

        SFML.Graphics.Image image;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Load a texture from a file path.
        /// </summary>
        /// <param name="source">The file path to load from.</param>
        /// <param name="useCache">Determines if the cache should be checked first.</param>
        public Texture(string source, bool useCache = true) {
            if (useCache) {
                texture = Textures.Load(source);
            }
            else {
                texture = new SFML.Graphics.Texture(source);
            }
            Source = source;

            texture.Smooth = DefaultSmooth;
        }

        /// <summary>
        /// Create a texture from a stream of bytes.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        /// <param name="useCache">Determines if the cache should be checked first.</param>
        public Texture(Stream stream, bool useCache = true) {
            if (useCache) {
                texture = Textures.Load(stream);
            }
            else {
                texture = new SFML.Graphics.Texture(stream);
            }
            Source = "stream";

            texture.Smooth = DefaultSmooth;
        }

        /// <summary>
        /// Creates a new texture from a copy of another texture.  No cache option for this.
        /// </summary>
        /// <param name="copy">The texture to copy from.</param>
        public Texture(Texture copy) {
            texture = new SFML.Graphics.Texture(copy.SFMLTexture);

            Source = copy.Source;

            texture.Smooth = DefaultSmooth;
        }

        /// <summary>
        /// Create a texture from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to load from.</param>
        /// <param name="useCache">Determines if the cache should be checked first.</param>
        public Texture(byte[] bytes, bool useCache = true) {
            if (useCache) {
                using (MemoryStream ms = new MemoryStream(bytes)) {
                    texture = Textures.Load(ms);
                }
            }
            else {
                using (MemoryStream ms = new MemoryStream(bytes)) {
                    texture = new SFML.Graphics.Texture(ms);
                }
            }
            Source = "byte array";

            texture.Smooth = DefaultSmooth;
        }

        /// <summary>
        /// Creates an empty texture of width and height.  This does not use the cache.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        public Texture(int width, int height) {
            if (width < 0) throw new ArgumentException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentException("Height must be greater than 0.");

            texture = new SFML.Graphics.Texture((uint)width, (uint)height);

            Source = width + " x " + height + " texture";

            texture.Smooth = DefaultSmooth;
        }

        #endregion Public Constructors

        #region Internal Constructors

        /// <summary>
        /// Load a texture from an SFML texture.
        /// </summary>
        /// <param name="texture"></param>
        internal Texture(SFML.Graphics.Texture texture) {
            this.texture = texture;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// The height of the texture.
        /// </summary>
        public int Height {
            get { return (int)texture.Size.Y; }
        }

        /// <summary>
        /// The array of pixels in the texture in bytes.
        /// </summary>
        public byte[] Pixels {
            get {
                CreateImage();
                return image.Pixels;
            }
            set {
                image = new SFML.Graphics.Image((uint)Width, (uint)Height, value);
                Update();
            }
        }

        /// <summary>
        /// The rectangle created by the Texture's width and height.
        /// </summary>
        public Rectangle Region {
            get { return new Rectangle(0, 0, Width, Height); }
        }

        /// <summary>
        /// Determines if the source texture is smoothed when transformed.
        /// </summary>
        public bool Smooth { get { return texture.Smooth; } set { texture.Smooth = value; } }

        /// <summary>
        /// The file path source if the texture was loaded from a file.
        /// </summary>
        public string Source { get; private set; }
        /// <summary>
        /// The width of the Texture.
        /// </summary>
        public int Width {
            get { return (int)texture.Size.X; }
        }

        #endregion Public Properties

        #region Internal Properties

        internal SFML.Graphics.Texture SFMLTexture {
            get { return texture; }
        }

        #endregion Internal Properties

        #region Public Methods

        /// <summary>
        /// Copy pixels from one texture to another using blitting.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromX"></param>
        /// <param name="fromY"></param>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        public void CopyPixels(Texture from, int fromX, int fromY, int toX, int toY) {
            CreateImage();
            from.CreateImage();

            image.Copy(from.image, (uint)toX, (uint)toY, new SFML.Graphics.IntRect(fromX, fromY, from.Width, from.Height));

            texture = new SFML.Graphics.Texture(image);
            needsUpdate = true;
        }

        public void CopyPixels(Texture from, int fromX, int fromY, int fromWidth, int fromHeight, int toX, int toY) {
            CreateImage();
            from.CreateImage();

            image.Copy(from.image, (uint)toX, (uint)toY, new SFML.Graphics.IntRect(fromX, fromY, fromWidth, fromHeight));

            texture = new SFML.Graphics.Texture(image);
            needsUpdate = true;
        }

        /// <summary>
        /// Loads the image internally in the texture for image manipulation.  This is
        /// handled automatically, but it's exposed so that it can be manually controlled.
        /// </summary>
        /// <param name="forceNewImage">If set to true a new image will always be created instead of only when there is no image.</param>
        public void CreateImage(bool forceNewImage = false) {
            if (image == null || forceNewImage) {
                image = texture.CopyToImage();
            }
        }

        /// <summary>
        /// Get the Color from a specific pixel on the texture.
        /// Warning: This is slow!
        /// </summary>
        /// <param name="x">The x coordinate of the pixel to get.</param>
        /// <param name="y">The y coordinate of the pixel to get.</param>
        /// <returns>The Color of the pixel.</returns>
        public Color GetPixel(int x, int y) {
            if (x < 0) throw new ArgumentException("X must be greater or equal to than 0.");
            if (y < 0) throw new ArgumentException("Y must be greater or equal to than 0.");

            CreateImage();

            return new Color(image.GetPixel((uint)x, (uint)y));
        }

        /// <summary>
        /// Save the texture to a file. The supported image formats are bmp, png, tga and jpg.
        /// </summary>
        /// <param name="path">The file path to save to. The type of image is deduced from the extension.</param>
        public void SaveToFile(string path) {
            CreateImage();

            image.SaveToFile(path);
        }

        /// <summary>
        /// Sets the color of a specific pixel on the texture.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <param name="color">The Color to set the pixel to.</param>
        public void SetPixel(int x, int y, Color color) {
            if (x < 0) throw new ArgumentException("X must be greater than 0.");
            if (y < 0) throw new ArgumentException("Y must be greater than 0.");
            if (x > Width) throw new ArgumentException("X must be within the texture width.");
            if (y > Height) throw new ArgumentException("Y must be within the texture width.");
           
            CreateImage();
            
            image.SetPixel((uint)x, (uint)y, color.SFMLColor);
            //texture = new SFML.Graphics.Texture(image);
            texture.Update(image);

            needsUpdate = true;
        }

        /// <summary>
        /// Sets the color of a rectangle of pixels on the texture.
        /// </summary>
        /// <param name="x">The x coordinate of the rectangle.</param>
        /// <param name="y">The y coordinate of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        public void SetRect(int x, int y, int width, int height, Color color) {
            if (x < 0) throw new ArgumentException("X must be greater than 0.");
            if (y < 0) throw new ArgumentException("Y must be greater than 0.");
            if (width < 0) throw new ArgumentException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentException("Height must be greater than 0.");

            for (var xx = x; xx < x + width; xx++) {
                for (var yy = y; yy < y + height; yy++) {
                    SetPixel(xx, yy, color);
                }
            }
        }
        /// <summary>
        /// Updates the texture to reflect changes made from SetPixel.
        /// </summary>
        public void Update() {
            if (needsUpdate) {
                texture.Update(image);
                needsUpdate = false;
            }
        }

        /// <summary>
        /// Updates the texture with a byte array.
        /// Note: Updates immediately. Probably not the fastest.
        /// </summary>
        /// <param name="bytes">The byte array containing our pixels.</param>
        public void SetBytes(byte[] bytes) {
            texture.Update(bytes);
        }

        /// <summary>
        /// Updates the texture with a byte array, at the given position and size.
        /// Note: Updates immediately. Probably not the fastest.
        /// </summary>
        /// <param name="bytes">The byte array containing our pixels.</param>
        /// <param name="width">The width of the section we are updating.</param>
        /// <param name="height">The height of the section we are updating.</param>
        /// <param name="x">The X coordinate of the section we are updating.</param>
        /// <param name="y">The Y coordinate of the section we are updating.</param>
        public void SetBytes(byte[] bytes, int width, int height, int x = 0, int y = 0) {
            texture.Update(bytes, (uint)width, (uint)height, (uint)x, (uint)y);
        }

        /// <summary>
        /// Dispose the SFML texture to clear up memory probably.
        /// Warning: might not want to do this since other Textures might be using the same cached texture!
        /// </summary>
        public void Dispose() {
            SFMLTexture.Dispose();
        }

        #endregion Public Methods

        
    }
}
