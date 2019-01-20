using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Graphic that can render a bunch of static images all at once.  Images must use the same
    /// texture as the Decals object in order to be baked together properly.
    /// </summary>
    public class Decals : Graphic {

        #region Private Fields

        List<Image> images = new List<Image>();

        #endregion

        #region Public Properties

        /// <summary>
        /// If the decals have been baked or not.
        /// </summary>
        public bool Solid { get; private set; }

        /// <summary>
        /// The number of images in the list.
        /// </summary>
        public int Count {
            get {
                return images.Count;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Decals object using a source file path for a texture.
        /// </summary>
        /// <param name="source">The file path to the texture.</param>
        public Decals(string source) {
            SetTexture(new Texture(source));
            Initialize();
        }

        /// <summary>
        /// Create a new Decals object using a Texture.
        /// </summary>
        /// <param name="texture">The Texture to use.</param>
        public Decals(Texture texture) {
            SetTexture(texture);
            Initialize();
        }

        /// <summary>
        /// Create a new Decals object using an AtlasTexture.
        /// </summary>
        /// <param name="texture"></param>
        public Decals(AtlasTexture texture) {
            SetTexture(texture);
            Initialize();
        }

        #endregion

        #region Private Methods

        void Initialize() {
            NeedsUpdate = false;
        }

        protected override void UpdateDrawable() {
            base.UpdateDrawable();

            SFMLVertices.Clear();

            float maxX = float.MinValue;
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            float minY = float.MaxValue;

            foreach (var img in images) {
                img.UpdateDrawableIfNeeded();

                for (uint i = 0; i < img.GetVertices().VertexCount; i++) {
                    var v = img.GetVertices()[i];

                    var transform = SFML.Graphics.Transform.Identity;
                    transform.Translate(img.X - img.OriginX, img.Y - img.OriginY);
                    transform.Rotate(img.Angle, img.OriginX, img.OriginY);
                    transform.Scale(img.ScaleX, img.ScaleY, img.OriginX, img.OriginY);

                    var p = transform.TransformPoint(v.Position.X, v.Position.Y);

                    maxX = Util.Max(maxX, p.X);
                    minX = Util.Min(minX, p.X);
                    maxY = Util.Max(maxY, p.Y);
                    minY = Util.Min(minY, p.Y);

                    SFMLVertices.Append(p.X, p.Y, img.Color, v.TexCoords.X, v.TexCoords.Y);
                }
            }

            Width = Math.Abs((int)Util.Ceil(maxX - minX));
            Height = Math.Abs((int)Util.Ceil(maxY - minY));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add an image to the list of decals.  Only works if Solid is false.
        /// </summary>
        /// <param name="image">The image to add.</param>
        public void Add(Image image) {
            if (Solid) return;
            if (!image.Batchable) {
                throw new ArgumentException("Non batchable images cannot be baked.");
            }
            if (image.Texture.SFMLTexture != Texture.SFMLTexture) {
                throw new ArgumentException("Images must use the same texture as the Decals object.");
            }
            images.Add(image);
        }

        /// <summary>
        /// Remove an image from the list of decals.  Only works if Solid is false.
        /// </summary>
        /// <param name="image"></param>
        public void Remove(Image image) {
            if (Solid) return;
            images.RemoveIfContains(image);
        }

        /// <summary>
        /// Erases all images from the list.  Only works if Solid is false.  Will not immediately show changes
        /// until Bake() is called.
        /// </summary>
        public void Clear() {
            if (Solid) return;
            images.Clear();
            NeedsUpdate = true;
            UpdateDrawableIfNeeded();
        }

        /// <summary>
        /// Bake all the images together for rendering.
        /// </summary>
        public void Bake() {
            if (Count == 0) return; // Don't bake if 0 images.
            if (!Solid) {
                NeedsUpdate = true;
                Solid = true;
                UpdateDrawableIfNeeded();
            }
        }

        /// <summary>
        /// Unbake the images back to an editable form.
        /// </summary>
        public void Unbake() {
            if (Solid) {
                Solid = false;
            }
        }

        #endregion
        
    }
}
