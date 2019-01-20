using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Graphic that represents a render target.  By default the game uses a master surface to
    /// render the game to the window.  Be aware of graphics card limiations of render textures when
    /// creating surfaces.
    /// </summary>
    public class Surface : Image {

        #region Private Fields

        RenderStates states;

        float cameraX, cameraY, cameraAngle, cameraZoom = 1f;

        RectangleShape fill;

        List<Shader> shaders = new List<Shader>();

        RenderTexture
            postProcessA,
            postProcessB;

        bool saveNextFrame;
        string saveNameFramePath;

        #endregion

        #region Public Fields

        /// <summary>
        /// The color that the surface will fill with at the start of each render.
        /// </summary>
        public Color FillColor;

        /// <summary>
        /// Determines if the Surface will automatically clear at the start of the next render cycle.
        /// </summary>
        public bool AutoClear = true;

        /// <summary>
        /// Determines if the Surface will automatically set its camera to the Scene's camera.
        /// </summary>
        public bool UseSceneCamera;

        #endregion

        #region Public Properties

        /// <summary>
        /// The reference to the Game using this Surface (if it is the main Surface the game is rendering to!)
        /// </summary>
        public Game Game { get; internal set; }

        /// <summary>
        /// The camera X for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active Scene.
        /// </summary>
        public float CameraX {
            set {
                cameraX = value;
                RefreshView();
            }
            get {
                return cameraX;
            }
        }

        /// <summary>
        /// The camera Y for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active Scene.
        /// </summary>
        public float CameraY {
            set {
                cameraY = value;
                RefreshView();
            }
            get {
                return cameraY;
            }
        }

        /// <summary>
        /// The camera angle for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active Scene.
        /// </summary>
        public float CameraAngle {
            set {
                cameraAngle = value;
                RefreshView();
            }
            get {
                return cameraAngle;
            }
        }

        /// <summary>
        /// The camera zoom for the view of the surface.
        /// Note: For the game's main surface, this is controlled by the active Scene.
        /// </summary>
        public float CameraZoom {
            set {
                cameraZoom = value;
                if (cameraZoom <= 0) { cameraZoom = 0.0001f; } //dont be divin' by zero ya hear?
                RefreshView();
            }
            get {
                return cameraZoom;
            }
        }

        public float CameraWidth {
            get {
                return Width / CameraZoom;
            }
        }

        public float CameraHeight {
            get {
                return Height / CameraZoom;
            }
        }

        /// <summary>
        /// The Texture the Surface has rendered to.
        /// </summary>
        public override Texture Texture {
            get {
                return new Texture(renderTexture.Texture);
            }
        }

        /// <summary>
        /// Convert an X position into the same position but on the Surface.
        /// TODO: Make this work with scale and rotation.
        /// </summary>
        /// <param name="x">The X position in the Scene.</param>
        /// <returns>The X position on the Surface.</returns>
        public float SurfaceX(float x) {
            return x - X + cameraX;
        }

        /// <summary>
        /// Convert a Y position into the same position but on the Surface.
        /// TODO: Make this work with scale and rotation.
        /// </summary>
        /// <param name="y">The Y position in the Scene.</param>
        /// <returns>The Y position on the Surface.</returns>
        public float SurfaceY(float y) {
            return y - Y + cameraY;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a Surface with a specified size.
        /// </summary>
        /// <param name="width">The width of the Surface to create.</param>
        /// <param name="height">The height of the Surface to create.</param>
        /// <param name="color">The default fill color of the Surface.</param>
        public Surface(int width, int height, Color color = null) {
            if (width < 0) throw new ArgumentException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentException("Height must be greater than 0.");

            if (color == null) color = Color.None;

            FillColor = color;
            Width = width;
            Height = height;
            renderTexture = new RenderTexture((uint)Width, (uint)Height);
            TextureRegion = new Rectangle(0, 0, Width, Height);
            ClippingRegion = TextureRegion;
            renderTexture.Smooth = Texture.DefaultSmooth;

            fill = new RectangleShape(new Vector2f(Width, Height)); // Using this for weird clearing bugs on some video cards

            Clear();
        }

        /// <summary>
        /// Creates a Surface of a specified size.
        /// </summary>
        /// <param name="width">The width of the Surface to create.</param>
        /// <param name="height">The height of the Surface to create.</param>
        public Surface(int width, int height) : this(width, height, Color.None) { }

        /// <summary>
        /// Creates a Surface of a specified size.
        /// </summary>
        /// <param name="size">The width and height of the Surface to create.</param>
        public Surface(int size) : this(size, size) { }

        /// <summary>
        /// Creates a Surface of a specified size.
        /// </summary>
        /// <param name="size">The width and height of the Surface to create.</param>
        /// <param name="color">The default fill color of the Surface.</param>
        public Surface(int size, Color color) : this(size, size, color) { }

        #endregion

        #region Private Methods

        void UpdateShader() {
            if (shaders.Count < 2) {
                if (postProcessA != null) {
                    postProcessA.Dispose();
                    postProcessA = null;
                }
                if (postProcessB != null) {
                    postProcessB.Dispose();
                    postProcessB = null;
                }
            }
            else if (shaders.Count == 2) {
                if (postProcessA == null) {
                    postProcessA = new RenderTexture((uint)Width, (uint)Height);
                }
                if (postProcessB != null) {
                    postProcessB.Dispose();
                    postProcessB = null;
                }
            }
            else if (shaders.Count > 2) {
                if (postProcessA == null) {
                    postProcessA = new RenderTexture((uint)Width, (uint)Height);
                }
                if (postProcessB == null) {
                    postProcessB = new RenderTexture((uint)Width, (uint)Height);
                }
            }
        }
       
        void RefreshView() {
            View v = new View(new FloatRect(cameraX, cameraY, Width, Height));
            
            v.Rotation = -cameraAngle;
            v.Zoom(1 / cameraZoom);
            RenderTarget.SetView(v);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a shader to be drawn on the surface.  If "Shader" is set, the shader list is ignored.
        /// </summary>
        /// <param name="shader">The Shader to add.</param>
        public void AddShader(Shader shader) {
            shaders.Add(shader);
            UpdateShader();
        }

        /// <summary>
        /// Remove a shader from the surface.
        /// </summary>
        /// <param name="shader">The Shader to remove.</param>
        public void RemoveShader(Shader shader) {
            shaders.Remove(shader);
            UpdateShader();
        }

        /// <summary>
        /// Remove the top most shader on the list of shaders.
        /// </summary>
        /// <returns>The removed Shader.</returns>
        public Shader PopShader() {
            if (shaders.Count == 0) return null;

            var shader = shaders[shaders.Count - 1];
            RemoveShader(shader);
            return shader;
        }

        /// <summary>
        /// Calls the SFML Display function on the internal render texture.  Should be used before
        /// any sort of rendering, otherwise the texture will be upside down!
        /// </summary>
        public void Display() {
            renderTexture.Display();
            SetTexture(new Texture(renderTexture.Texture));
            Update();
            UpdateDrawable();
        }

        /// <summary>
        /// Remove all shaders from the surface.
        /// </summary>
        public void ClearShaders() {
            shaders.Clear();
            UpdateShader();
        }

        /// <summary>
        /// Replace all shaders with a single shader.  This will be ignored if "Shader" is set.
        /// </summary>
        /// <param name="shader">The Shader to use.</param>
        public void SetShader(Shader shader) {
            shaders.Clear();
            shaders.Add(shader);
            UpdateShader();
        }

        /// <summary>
        /// Draws a graphic to this surface.
        /// </summary>
        /// <param name="graphic">The Graphic to draw.</param>
        /// <param name="x">The X position of the Graphic.</param>
        /// <param name="y">The Y position of the Graphic.</param>
        public void Draw(Graphic graphic, float x = 0, float y = 0) {
            Surface tempSurface = Otter.Draw.Target;
            Otter.Draw.SetTarget(this);
            graphic.Render(x, y);
            Otter.Draw.SetTarget(tempSurface);
        }

        /// <summary>
        /// Fills the surface with the specified color.
        /// </summary>
        /// <param name="color">The Color to fill the Surface with.</param>
        public void Fill(Color color) {
            fill.Size = new Vector2f(Width, Height);
            fill.FillColor = color.SFMLColor;
            fill.Position = new Vector2f(CameraX, CameraY);
            renderTexture.Draw(fill); // Sometimes after 20-30 frames, game will freeze here?
        }

        /// <summary>
        /// Clears the surface with the fill color.
        /// </summary>
        public void Clear() {
            renderTexture.Clear(FillColor.SFMLColor);
        }

        /// <summary>
        /// Clears the surface with a specified color.
        /// </summary>
        /// <param name="color">The Color to clear the Surface with.</param>
        public void Clear(Color color) {
            renderTexture.Clear(color.SFMLColor);
        }

        /// <summary>
        /// Determines the pixel smoothing for the surface.
        /// </summary>
        public override bool Smooth {
            get { return renderTexture.Smooth; }
            set { renderTexture.Smooth = value; }
        }

        /// <summary>
        /// Resizes the surface to a new width and height.
        /// </summary>
        /// <param name="width">The new width of the surface.</param>
        /// <param name="height">The new height of the surface.</param>
        public void Resize(int width, int height) {
            if (width < 0) throw new ArgumentException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentException("Height must be greater than 0.");

            if (Width == width && Height == height) return;

            Width = width;
            Height = height;

            renderTexture.Dispose(); // not sure if needed?
            renderTexture = new RenderTexture((uint)Width, (uint)Height);
            TextureRegion = new Rectangle(0, 0, Width, Height);
            ClippingRegion = TextureRegion;
            renderTexture.Smooth = Texture.DefaultSmooth;

            fill = new RectangleShape(new Vector2f(Width, Height)); // Using this for weird clearing bugs on some video cards

            Clear();
        }

        /// <summary>
        /// Draw the Surface.
        /// </summary>
        /// <param name="x">The X position offset.</param>
        /// <param name="y">The Y position offset.</param>
        public override void Render(float x = 0, float y = 0) {
            Display();

            SFMLDrawable = RenderShaders();

            base.Render(x, y);

            if (saveNextFrame) {
                saveNextFrame = false;
                var saveTarget = new RenderTexture((uint)Width, (uint)Height);
                saveTarget.Draw(SFMLDrawable, states);
                saveTarget.Display();
                saveTarget.Texture.CopyToImage().SaveToFile(saveNameFramePath);
                saveTarget.Dispose();
            }

            if (AutoClear) Clear();
        }

        /// <summary>
        /// Draw the surface directly to the game window.  This will refresh the view,
        /// and Display the surface, as well as clear it if AutoClear is true.
        /// </summary>
        /// <param name="game">The Game to render to.</param>
        public void DrawToWindow(Game game) {
            RefreshView();

            Display();

            Drawable drawable = RenderShaders();

            game.Window.Draw(drawable, states);

            if (saveNextFrame) {
                saveNextFrame = false;
                game.Window.Capture().SaveToFile(saveNameFramePath);
            }

            if (AutoClear) Clear(FillColor);
        }

        /// <summary>
        /// Draw the Surface to the Game window.
        /// </summary>
        public void DrawToWindow() {
            DrawToWindow(Game);
        }

        /// <summary>
        /// Set view of the Surface.
        /// </summary>
        /// <param name="x">The X position of the view.</param>
        /// <param name="y">The Y position of the view.</param>
        /// <param name="angle">The angle of the view.</param>
        /// <param name="zoom">The zoom of the view.</param>
        public void SetView(float x, float y, float angle = 0, float zoom = 1f) {
            cameraX = x;
            cameraY = y;
            cameraAngle = angle;
            cameraZoom = zoom;
            RefreshView();
        }

        /// <summary>
        /// Returns a texture by getting the current render texture. I don't know if this works right yet.
        /// </summary>
        /// <returns></returns>
        public Texture GetTexture() {
            return new Texture(renderTexture.Texture);
        }

        /// <summary>
        /// Saves the next completed render to a file. The supported image formats are bmp, png, tga and jpg.
        /// Note that this waits until the end of the game's Render() to actually export, otherwise it will be blank!
        /// </summary>
        /// <param name="path">
        /// The file path to save to. The type of image is deduced from the extension. If left unspecified the
        /// path will be a png file of the current time in the same folder as the executable.
        /// </param>
        public void SaveToFile(string path = "") {
            saveNextFrame = true;
            if (path == "") {
                path = string.Format("{0:yyyyMMddHHmmssff}.png", DateTime.Now);
            }
            saveNameFramePath = path;
        }

        /// <summary>
        /// Matches the view of the surface to the same view of a Scene.
        /// </summary>
        /// <param name="scene">The Scene to match the camera with.</param>
        public void CameraScene(Scene scene) {
            SetView(scene.CameraX + X, scene.CameraY + Y, scene.CameraAngle, scene.CameraZoom);
        }

        /// <summary>
        /// Centers the camera of the surface.
        /// </summary>
        /// <param name="x">The X position to be the center of the scene.</param>
        /// <param name="y">The Y position to be the center of the scene.</param>
        public void CenterCamera(float x, float y) {
            CameraX = x - HalfWidth;
            CameraY = y - HalfHeight;
        }

        #endregion

        #region Internal

        internal RenderTexture renderTexture;

        internal void Draw(Drawable drawable) {
            RenderTarget.Draw(drawable);
        }

        internal void Draw(Vertex[] vertices, RenderStates states) {
            RenderTarget.Draw(vertices, PrimitiveType.Quads, states);
        }

        internal void Draw(Vertex[] vertices, PrimitiveType primitiveType, RenderStates states) {
            RenderTarget.Draw(vertices, primitiveType, states);
        }

        internal void Draw(List<Vertex> vertices, PrimitiveType primitiveType, RenderStates states) {
            Draw(vertices.ToArray(), primitiveType, states);
        }

        internal void Draw(List<Vertex> vertices, RenderStates states) {
            Draw(vertices.ToArray(), states);
        }

        internal void Draw(Texture texture, float x, float y, float originX, float originY, int width, int height, float scaleX, float scaleY, float angle, Color color = null, BlendMode blend = BlendMode.Alpha, Shader shader = null) {
            states = new RenderStates(Texture.SFMLTexture);

            //states.BlendMode = (SFML.Graphics.BlendMode)Blend;
            states.BlendMode = SFMLBlendMode(blend);

            if (Shader != null) {
                states.Shader = Shader.SFMLShader;
            }

            states.Transform.Translate(x - OriginX, y - OriginY);
            states.Transform.Rotate(-Angle, OriginX, OriginY);
            states.Transform.Scale(ScaleX, ScaleY, OriginX, OriginY);

            var v = new VertexArray(PrimitiveType.Quads);

            if (color == null) color = Color.White;

            v.Append(x, y, color, 0, 0);
            v.Append(x + width, y, color, width, 0);
            v.Append(x + width, y + height, color, width, height);
            v.Append(x, y + height, color, 0, height);

            Draw(v, states);
        }

        internal void Draw(Drawable drawable, RenderStates states) {
            // Sometimes this hangs in the first 30ish frames?
            // I have no clue what causes this.
            RenderTarget.Draw(drawable, states);
        }

        internal RenderTarget RenderTarget {
            get { return renderTexture; }
        }

        /// <summary>
        /// This goes through all the shaders and applies them between two buffers, and
        /// eventually spits out the final drawable object.
        /// </summary>
        /// <returns></returns>
        Drawable RenderShaders() {
            Drawable drawable = SFMLVertices;

            states = new RenderStates(renderTexture.Texture);
            SetTexture(new Texture(renderTexture.Texture));
            states.Transform.Translate(X - OriginX, Y - OriginY);
            states.Transform.Rotate(Angle, OriginX, OriginY);
            states.Transform.Scale(ScaleX, ScaleY, OriginX, OriginY);
            //states.BlendMode = (SFML.Graphics.BlendMode)Blend;
            states.BlendMode = SFMLBlendMode(Blend);

            if (Shader != null) {
                states.Shader = Shader.SFMLShader;
            }
            else {
                if (shaders.Count == 1) {
                    states.Shader = shaders[0].SFMLShader;
                }
                else if (shaders.Count == 2) {
                    states = new RenderStates(renderTexture.Texture);
                    states.Shader = shaders[0].SFMLShader;

                    Game.Instance.RenderCount++;
                    postProcessA.Draw(SFMLVertices, states);
                    postProcessA.Display();

                    states.Shader = shaders[1].SFMLShader;

                    drawable = new Sprite(postProcessA.Texture);
                    states.Transform.Rotate(Angle, OriginX, OriginY);
                    states.Transform.Translate(new Vector2f(X - OriginX, Y - OriginY));
                    states.Transform.Scale(ScaleX, ScaleY, OriginX, OriginY);
                }
                else if (shaders.Count > 2) {
                    states = new RenderStates(renderTexture.Texture);
                    RenderTexture nextRt, currentRt;
                    nextRt = postProcessB;
                    currentRt = postProcessA;

                    states.Shader = shaders[0].SFMLShader;

                    Game.Instance.RenderCount++;
                    postProcessA.Draw(SFMLVertices, states);
                    postProcessA.Display();

                    for (int i = 1; i < shaders.Count - 1; i++) {
                        states = RenderStates.Default;
                        states.Shader = shaders[i].SFMLShader;

                        Game.Instance.RenderCount++;
                        nextRt.Draw(new Sprite(currentRt.Texture), states);
                        nextRt.Display();

                        nextRt = nextRt == postProcessA ? postProcessB : postProcessA;
                        currentRt = currentRt == postProcessA ? postProcessB : postProcessA;
                    }

                    drawable = new Sprite(currentRt.Texture);
                    currentRt.Display();
                    states.Shader = shaders[shaders.Count - 1].SFMLShader;
                    states.Transform.Rotate(Angle, OriginX, OriginY);
                    states.Transform.Translate(new Vector2f(X - OriginX, Y - OriginY));
                    states.Transform.Scale(ScaleX, ScaleY, OriginX, OriginY);
                }
            }

            return drawable;
        }

        #endregion

    }
}
