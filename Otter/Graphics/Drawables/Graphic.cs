using SFML.Graphics;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Base abstract class used for anything that can be rendered.
    /// </summary>
    public abstract class Graphic {

        #region Private Fields

        Color color;

        protected VertexArray SFMLVertices = new VertexArray(PrimitiveType.Quads);
        protected Drawable SFMLDrawable;

        float shakeX;
        float shakeY;

        public List<Transformation> Transforms = new List<Transformation>() { new Transformation() };
        Dictionary<string, Transformation> transformByString = new Dictionary<string, Transformation>();

        protected float
            RepeatSizeX,
            RepeatSizeY;

        protected bool roundRendering = true;

        #endregion

        #region Public Fields

        public Transformation Transform {
            get {
                return Transforms[0];
            }
        }

        public Transformation AddTransform(Transformation transform) {
            Transforms.Add(transform);
            return transform;
        }

        public Transformation AddTransform(Vector2 translation, Vector2 scale, Vector2 origin, float rotation) {
            return AddTransform(new Transformation(translation, scale, origin, rotation));
        }

        public Transformation AddTransform() {
            return AddTransform(Vector2.Zero, Vector2.One, Vector2.Zero, 0);
        }

        public Transformation RemoveTransformation(Transformation transform) {
            Transforms.Remove(transform);
            return transform;
        }

        public Transformation PopTransformation() {
            if (Transforms.Count > 1) {
                var t = Transforms[Transforms.Count - 1];
                Transforms.Remove(t);
                return t;
            }
            return null;
        }

        /// <summary>
        /// Determines if the Graphic is rendered relative to its Entity.
        /// </summary>
        public bool Relative = true;

        /// <summary>
        /// The X position of the Graphic.
        /// </summary>
        public float X {
            get {
                return Transform.Translation.X;
            }
            set {
                Transform.Translation.X = value;
            }
        }

        /// <summary>
        /// The Y position of the Graphic.
        /// </summary>
        public float Y {
            get {
                return Transform.Translation.Y;
            }
            set {
                Transform.Translation.Y = value;
            }
        }

        /// <summary>
        /// Determines if the Graphic will render.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// The scroll factor for the x position. Used for parallax like effects. Values lower than 1
        /// will scroll slower than the camera (appear to be further away) and values higher than 1
        /// will scroll faster than the camera (appear to be closer.)
        /// </summary>
        public float ScrollX = 1;

        /// <summary>
        /// The scroll factor for the y position. Used for parallax like effects. Values lower than 1
        /// will scroll slower than the camera (appear to be further away) and values higher than 1
        /// will scroll faster than the camera (appear to be closer.)
        /// </summary>
        public float ScrollY = 1;

        /// <summary>
        /// The horizontal scale of the graphic.  Used in the final transformation.
        /// </summary>
        public float ScaleX {
            get {
                return Transform.Scale.X;
            }
            set {
                Transform.Scale.X = value;
            }
        }

        /// <summary>
        /// The vertical scale of the graphic.  Used in the final transformation.
        /// </summary>
        public float ScaleY {
            get {
                return Transform.Scale.Y;
            }
            set {
                Transform.Scale.Y = value;
            }
        }

        /// <summary>
        /// The angle of rotation of the graphic.  Used in the final transformation.
        /// </summary>
        public float Angle {
            get {
                return Transform.Rotation;
            }
            set {
                Transform.Rotation = value;
            }
        }

        /// <summary>
        /// The X origin point to scale and rotate the graphic with.  Used in the final transformation.
        /// </summary>
        public float OriginX {
            get {
                return Transform.Origin.X;
            }
            set {
                Transform.Origin.X = value;
            }
        }

        /// <summary>
        /// The Y origin point to scale and rotate the graphic with.  Used in the final transformation.
        /// </summary>
        public float OriginY {
            get {
                return Transform.Origin.Y;
            }
            set {
                Transform.Origin.Y = value;
            }
        }

        /// <summary>
        /// The horizontal amount to randomly offset the graphic by each frame.
        /// </summary>
        public float ShakeX;

        /// <summary>
        /// The vertial amount to randomly offset the graphic by each frame.
        /// </summary>
        public float ShakeY;

        /// <summary>
        /// If true the graphic will always update its drawable.
        /// </summary>
        protected bool Dynamic;

        /// <summary>
        /// The region to render the Texture with.
        /// </summary>
        public Rectangle TextureRegion;

        /// <summary>
        /// The Rectangle to render an AtlasTexture with.
        /// </summary>
        public Rectangle AtlasRegion;

        /// <summary>
        /// The shader to be applied to this graphic.
        /// </summary>
        public Shader Shader;

        /// <summary>
        /// The name of the graphic.
        /// </summary>
        public string Name = "Graphic";

        /// <summary>
        /// The blend mode to be applied to this graphic.
        /// </summary>
        public BlendMode Blend = BlendMode.Alpha;

        /// <summary>
        /// Determines if the image should be rendered multiple times horizontally.
        /// </summary>
        public bool RepeatX;

        /// <summary>
        /// Determines if the image should be rendered multiple times vertically.
        /// </summary>
        public bool RepeatY;

        #endregion

        #region Public Properties

        /// <summary>
        /// The base color of the Graphic.  Multiplies the vertices of the graphic by this color.
        /// </summary>
        public Color Color {
            get {
                if (color == null) color = Color.White;
                return color;
            }
            set {
                // Get rid of graphic reference in old color
                if (color != null) {
                    color.Graphic = null;
                }
                color = new Color(value); // 2015/2/12: testing copying the color because of a bug
                // Set reference so that NeedsUpdate will be activated on changes
                color.Graphic = this;
                NeedsUpdate = true;
            }
        }

        /// <summary>
        /// The texture that the graphic is using.
        /// </summary>
        public virtual Texture Texture { get; private set; }

        /// <summary>
        /// The base transparency of the graphic.  A shortcut to access the base color's Alpha.
        /// </summary>
        public float Alpha {
            get {
                return Color.A;
            }
            set {
                Color.A = value;
                NeedsUpdate = true;
            }
        }

        /// <summary>
        /// The width of the Graphic.
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// The height of the Graphic.
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// The width in pixels of the image after applying the X scale.
        /// </summary>
        public float ScaledWidth {
            get {
                return Width * ScaleX;
            }
            set {
                ScaleX = value / Width;
            }
        }

        /// <summary>
        /// The height in pixels of the image after applying the Y scale.
        /// </summary>
        public float ScaledHeight {
            get {
                return Height * ScaleY;
            }
            set {
                ScaleY = value / Height;
            }
        }

        /// <summary>
        /// Smooth the texture of a sprite image while scaling it.
        /// </summary>
        public virtual bool Smooth {
            get {
                if (Texture != null) return Texture.Smooth;
                return false;
            }
            set {
                if (Texture != null) Texture.Smooth = value;
            }
        }

        /// <summary>
        /// Set both ScrollX and ScrollY.
        /// </summary>
        public float Scroll {
            set { ScrollX = value; ScrollY = value; }
            get { return (ScrollX + ScrollY) / 2f; }
        }

        /// <summary>
        /// Half of the width.
        /// </summary>
        public float HalfWidth { get { return Width / 2f; } }

        /// <summary>
        /// Half of the height.
        /// </summary>
        public float HalfHeight { get { return Height / 2f; } }

        /// <summary>
        /// Sets both the ScaleX and ScaleY at the same time.
        /// </summary>
        public float Scale {
            set { ScaleX = value; ScaleY = value; }
        }

        /// <summary>
        /// Sets both RepeatX and RepeatY at the same time.
        /// </summary>
        public bool Repeat {
            set { RepeatX = value; RepeatY = value; }
        }

        /// <summary>
        /// A shortcut to set both ShakeX and ShakeY.
        /// </summary>
        public float Shake {
            set { ShakeX = value; ShakeY = value; }
        }

        /// <summary>
        /// The X position of the left side of the Graphic.
        /// </summary>
        public float Left {
            get { return X - OriginX; }
        }

        /// <summary>
        /// The Y position of the top of the Graphic.
        /// </summary>
        public float Top {
            get { return Y - OriginY; }
        }

        /// <summary>
        /// The X position of the right side of the Graphic.
        /// </summary>
        public float Right {
            get { return Left + Width; }
        }

        /// <summary>
        /// The Y position of the bottom of the Graphic.
        /// </summary>
        public float Bottom {
            get { return Top + Height; }
        }

        /// <summary>
        /// The X position of the left of the Texture.
        /// </summary>
        public int TextureLeft {
            get {
                return AtlasRegion.Left + TextureRegion.Left;
            }
        }

        /// <summary>
        /// The X position of the right of the Texture.
        /// </summary>
        public int TextureRight {
            get {
                return TextureLeft + TextureRegion.Width;
            }
        }

        /// <summary>
        /// The Y position of the top of the Texture.
        /// </summary>
        public int TextureTop {
            get {
                return AtlasRegion.Top + TextureRegion.Top;
            }
        }

        /// <summary>
        /// The Y position of the bottom of the Texture.
        /// </summary>
        public int TextureBottom {
            get {
                return TextureTop + TextureRegion.Height;
            }
        }

        #endregion

        #region Private Methods

        protected void Append(float x, float y, Color color, float u, float v) {
            SFMLVertices.Append(x, y, color, u, v);
        }

        protected void Append(float x, float y, Color color = null) {
            SFMLVertices.Append(x, y, color);
        }

        /// <summary>
        /// Updates the internal SFML data for rendering.
        /// </summary>
        protected virtual void UpdateDrawable() {
            if (!Dynamic) {
                NeedsUpdate = false;
            }
        }

        protected virtual void TextureChanged() {

        }

        protected virtual void SFMLRender(Drawable drawable, float x = 0, float y = 0) {
            RenderStates renderStates;
            if (Texture != null) {
                renderStates = new RenderStates(Texture.SFMLTexture);
            }
            else {
                renderStates = RenderStates.Default;
            }
            renderStates.BlendMode = SFMLBlendMode(Blend);
            if (Shader != null) {
                renderStates.Shader = Shader.SFMLShader;
            }

            // This is really bad x_x lol
            renderStates.Transform.Translate(x - OriginX, y - OriginY);
            foreach (var t in Transforms) {
                if (t != Transform) {
                    renderStates.Transform.Translate(t.X, t.Y);
                }
                renderStates.Transform.Rotate(-t.Angle, t.OriginX, t.OriginY);
                renderStates.Transform.Scale(t.ScaleX, t.ScaleY, t.OriginX, t.OriginY);
            }

            if (Batchable) {
                Draw.Batchable((VertexArray)drawable, renderStates);
                return;
            }
            Draw.Drawable(drawable, renderStates);
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Removes the shader from the graphic.
        /// </summary>
        public virtual void ClearShader() {
            Shader = null;
        }

        /// <summary>
        /// Set the position of the Graphic.
        /// </summary>
        /// <param name="x">The X Position.</param>
        /// <param name="y">The Y Position.</param>
        public void SetPosition(float x, float y) {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Set the position of the Graphic.
        /// </summary>
        /// <param name="g">The Graphic to get the position from.</param>
        public void SetPosition(Graphic g, float offsetX = 0, float offsetY = 0) {
            SetPosition(g.X + offsetX, g.Y + offsetY);
        }

        /// <summary>
        /// Set the position of the Graphic.
        /// </summary>
        /// <param name="xy">The Vector2 to get the position from.</param>
        public void SetPosition(Vector2 xy) {
            SetPosition(xy.X, xy.Y);
        }

        /// <summary>
        /// Set the origin of the Graphic.
        /// </summary>
        /// <param name="x">The X origin.</param>
        /// <param name="y">The Y origin.</param>
        public void SetOrigin(float x, float y) {
            OriginX = x;
            OriginY = y;
        }

        /// <summary>
        /// Set the origin of the Graphic.
        /// </summary>
        /// <param name="xy">The X,Y position of the origin.</param>
        public void SetOrigin(Vector2 xy) {
            SetOrigin(xy.X, xy.Y);
        }

        /// <summary>
        /// Set the Texture that the Graphic is using (if it is using one.)
        /// </summary>
        /// <param name="path">The path to the Texture to use.</param>
        public void SetTexture(string path) {
            SetTexture(new Texture(path));
        }

        /// <summary>
        /// Set the Texture that the Graphic is using (if it is using one.)
        /// </summary>
        /// <param name="texture">The Texture to use.</param>
        public void SetTexture(Texture texture) {
            Texture = texture;
            TextureRegion = texture.Region;
            TextureChanged();
            NeedsUpdate = true;
        }

        /// <summary>
        /// Set the Texture that the Graphic is using (if it is using one.)
        /// </summary>
        /// <param name="atlasTexture">The AtlasTexture to use.</param>
        public void SetTexture(AtlasTexture atlasTexture) {
            Texture = atlasTexture.Texture;
            AtlasRegion = atlasTexture.Region;
            TextureRegion.Width = atlasTexture.Width;
            TextureRegion.Height = atlasTexture.Height;
            NeedsUpdate = true;
        }

        /// <summary>
        /// Update the graphic.
        /// </summary>
        public virtual void Update() {
            shakeX = Rand.Float(-ShakeX * 0.5f, ShakeX * 0.5f);
            shakeY = Rand.Float(-ShakeY * 0.5f, ShakeY * 0.5f);
        }

        /// <summary>
        /// Centers the graphic origin.
        /// </summary>
        public virtual void CenterOrigin() {
            OriginX = HalfWidth;
            OriginY = HalfHeight;
        }

        /// <summary>
        /// Centers the graphic origin while retaining its relative position.
        /// </summary>
        public virtual void CenterOriginZero() {
            float ox, oy;
            ox = OriginX;
            oy = OriginY;
            CenterOrigin();
            ox = OriginX - ox;
            oy = OriginY - oy;
            X += ox;
            Y += oy;
        }

        /// <summary>
        /// Draws the graphic.
        /// </summary>
        /// <param name="x">the x offset to draw the image from</param>
        /// <param name="y">the y offset to draw the image from</param>
        public virtual void Render(float x = 0, float y = 0) {
            if (!Visible) return;

            UpdateDrawableIfNeeded();

            float renderX = X + x + shakeX;
            float renderY = Y + y + shakeY;

            // Rounding here to fix 1 pixel offset problems (textures wrap around?)
            if (roundRendering) {
                renderX = Util.Round(renderX);
                renderY = Util.Round(renderY);
            }

            if (ScrollX != 1) {
                renderX = X + Draw.Target.CameraX * (1 - ScrollX) + x;
            }
            if (ScrollY != 1) {
                renderY = Y + Draw.Target.CameraY * (1 - ScrollY) + y;
            }

            float
                screenX = renderX - Draw.Target.CameraX,
                screenY = renderY - Draw.Target.CameraY;

            float
                zoom = Draw.Target.CameraZoom,
                w = Draw.Target.Width + OriginX,
                h = Draw.Target.Height + OriginY;

            float
                repeatLeft = Draw.Target.CameraX - (w / zoom - w) * 0.5f,
                repeatTop = Draw.Target.CameraY - (h / zoom - h) * 0.5f,
                repeatRight = Draw.Target.CameraX + (w / zoom + w) * 0.5f,
                repeatBottom = Draw.Target.CameraY + (h / zoom + h) * 0.5f;

            RepeatSizeX = repeatRight - repeatLeft + OriginX;
            RepeatSizeY = repeatBottom - repeatTop + OriginY;

            Drawable drawable;
            if (SFMLDrawable == null) {
                drawable = SFMLVertices;
            }
            else {
                drawable = SFMLDrawable;
            }


            if (!RepeatX && !RepeatY) {
                SFMLRender(drawable, renderX, renderY);
            }

            else if (RepeatX && !RepeatY) {
                while (renderX > repeatLeft) {
                    renderX -= ScaledWidth;
                }

                while (renderX < repeatRight) {
                    SFMLRender(drawable, renderX, renderY);
                    renderX += ScaledWidth;
                }
            }

            else if (!RepeatX && RepeatY) {
                while (renderY > repeatTop) {
                    renderY -= ScaledHeight;
                }

                while (renderY < repeatBottom) {
                    SFMLRender(drawable, renderX, renderY);
                    renderY += ScaledHeight;
                }
            }

            else if (RepeatX && RepeatY) {
                float startX = renderX;
                while (renderY > repeatTop) {
                    renderY -= ScaledHeight;
                }

                while (renderY < repeatBottom) {
                    while (renderX > repeatLeft) {
                        renderX -= ScaledWidth;
                    }

                    while (renderX < repeatRight) {
                        SFMLRender(drawable, renderX, renderY);
                        renderX += ScaledWidth;
                    }
                    renderY += ScaledHeight;
                }
            }
            
        }

        #endregion

        #region Internal

        /// <summary>
        /// Determines if the graphic's core drawable will have to be updated before it's rendered.
        /// </summary>
        internal bool NeedsUpdate = true; // Needs update on first update

        internal bool Batchable = false;

        internal void UpdateDrawableIfNeeded() {
            if (NeedsUpdate) {
                UpdateDrawable();
            }
        }

        internal SFML.Graphics.BlendMode SFMLBlendMode(BlendMode blend) {
            switch (blend) {
                case BlendMode.Alpha: return SFML.Graphics.BlendMode.Alpha;
                case BlendMode.Add: return SFML.Graphics.BlendMode.Add;
                case BlendMode.Multiply: return SFML.Graphics.BlendMode.Multiply;
                case BlendMode.None: return SFML.Graphics.BlendMode.None;
            }
            return SFML.Graphics.BlendMode.None;
        }

        #endregion

    }

    #region Enum

    /// <summary>
    /// The blendmodes that can be used for graphic rendering.
    /// </summary>
    public enum BlendMode {
        Alpha,
        Add,
        Multiply,
        None,
        Null
    }

    #endregion

}
