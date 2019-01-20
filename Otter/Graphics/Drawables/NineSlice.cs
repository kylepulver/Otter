using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Graphic type used to render a panel made up of 9 slices of an image. Handy for rendering panels
    /// with border graphics.
    /// </summary>
    public class NineSlice : Graphic {

        #region Static Methods

        /// <summary>
        /// Register a fill rectangle for a specific asset.  Useful for not having to set the same fill rect
        /// every time you use a NineSlice for a specific image.
        /// </summary>
        /// <param name="key">The asset path.</param>
        /// <param name="x1">Fill rect x1.</param>
        /// <param name="y1">Fill Rect y1</param>
        /// <param name="x2">Fill rect x2.</param>
        /// <param name="y2">Fill Rect y2</param>
        public static void SetFillRect(string key, int x1, int y1, int x2, int y2) {
            var rect = new Rectangle() {
                X = x1,
                Y = y1,
                Width = x2 - x1,
                Height = y2 - y1
            };

            if (!fillRects.ContainsKey(key)) {
                fillRects.Add(key, rect);
            }
            fillRects[key] = rect;
        }

        /// <summary>
        /// Sets the FillRect for a NineSlice globally.
        /// </summary>
        /// <param name="keys">The source Texture of the NineSlice. (File path or name on Atlas both work.)</param>
        /// <param name="x1">The left corner of the fill rectangle.</param>
        /// <param name="y1">The top corner of the fill rectangle.</param>
        /// <param name="x2">The right corner of the fill rectangle.</param>
        /// <param name="y2">The bottom corner of the fill rectangle.</param>
        public static void SetFillRect(string[] keys, int x1, int y1, int x2, int y2) {
            foreach (var k in keys) {
                SetFillRect(k, x1, y1, x2, y2);
            }
        }

        /// <summary>
        /// Set the FillRect of the NineSlice using padding values.
        /// </summary>
        /// <param name="top">How far from the top of the texture to begin the rectangle.</param>
        /// <param name="right">How far from the right of the texture to end the rectangle.</param>
        /// <param name="bottom">How far from the bottom of the texture to end the rectangle.</param>
        /// <param name="left">How far from the left of the texture to begin the rectangle.</param>
        /// <returns>The NineSlice object.</returns>
        public static void SetBorderPadding(string key, int top, int right, int bottom, int left) {
            var texture = new Texture(key); // Have to load a texture here but I think that's okay?

            var x1 = left;
            var y1 = top;
            var x2 = texture.Width - right;
            var y2 = texture.Height - bottom;
            SetFillRect(key, x1, y1, x2, y2);
        }

        /// <summary>
        /// Set the FillRect of the NineSlice using padding values.
        /// </summary>
        /// <param name="padding">How far from the border of the texture to make the rectangle.</param>
        /// <returns>The NineSlice object.</returns>
        public static void SetBorderPadding(string key, int padding) {
            SetBorderPadding(key, padding, padding, padding, padding);
        }

        /// <summary>
        /// Set the FillRect of the NineSlice using padding values.
        /// </summary>
        /// <param name="horizontal">How far horizontally from the border of the texture to make the rectangle.</param>
        /// <param name="vertical">How far horizontally from the border of the texture to make the rectangle.</param>
        /// <returns>The NineSlice object.</returns>
        public static void SetBorderPadding(string key, int horizontal, int vertical) {
            SetBorderPadding(key, horizontal, vertical, horizontal, vertical);
        }

        #endregion

        #region Private Fields

        int
            tWidth,
            tHeight;

        static Dictionary<string, Rectangle> fillRects = new Dictionary<string, Rectangle>();

        int sliceX1, sliceX2, sliceY1, sliceY2;

        PanelType paneltype;

        bool usePanelClip;
        Rectangle panelClip;

        float panelScaleX, panelScaleY;

        #endregion

        #region Public Fields

        /// <summary>
        /// Draw the panel from the top left corner of the middle slice.
        /// </summary>
        public bool UseInsideOrigin;

        /// <summary>
        /// When using PanelType.Tiled snap the width to increments of the tile width.
        /// </summary>
        public bool SnapWidth;

        /// <summary>
        /// When using PanelType.Tiled snap the height to increments of the tile height.
        /// </summary>
        public bool SnapHeight;

        /// <summary>
        /// Determines how the size of the panel will be adjusted when setting PanelWidth and PanelHeight.
        /// If set to All, the entire panel will be the width and height.
        /// If set to Inside, the inside of the panel will be the width and height.
        /// </summary>
        public PanelSizeMode PanelSizeMode = PanelSizeMode.All;

        #endregion

        #region Public Properties

        /// <summary>
        /// The type of panel to use for the NineSlice.
        /// </summary>
        public PanelType PanelType {
            get {
                return paneltype;
            }
            set {
                paneltype = value;
                NeedsUpdate = true;
            }
        }

        /// <summary>
        /// Render the NineSlice through a clipping rectangle.
        /// </summary>
        public Rectangle ClippingRegion {
            set {
                panelClip = value;
                usePanelClip = true;
                NeedsUpdate = true;
            }
            get {
                return panelClip;
            }
        }

        /// <summary>
        /// Determines if the ClippingRegion is used or not.
        /// </summary>
        public bool UsePanelClip {
            set { usePanelClip = value; }
            get { return usePanelClip; }
        }

        /// <summary>
        /// Set the panel width of the NineSlice.  This will update and rerender it.
        /// </summary>
        public int PanelWidth {
            set {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    value += sliceX1 + tWidth - sliceX2;
                }

                if (SnapWidth) {
                    Width = GetSnapWidth(value);
                }
                else {
                    Width = value;
                }
                if (Width < 0) Width = 0;
                NeedsUpdate = true;
            }
            get {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    return Width - sliceX1 + tWidth - sliceX2;
                }

                return Width;
            }
        }

        /// <summary>
        /// Set the panel height of the NineSlice.  This will update and rerender it.
        /// </summary>
        public int PanelHeight {
            set {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    value += sliceY1 + tHeight - sliceY2;
                }

                if (SnapHeight) {
                    Height = GetSnapHeight(value);
                }
                else {
                    Height = value;
                }
                if (Height < 0) Height = 0;
                NeedsUpdate = true;
            }
            get {
                if (PanelSizeMode == PanelSizeMode.Inside) {
                    return Height - sliceY1 + tHeight - sliceY2;
                }

                return Height;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new NineSlice with a file path to a Texture.
        /// </summary>
        /// <param name="source">The file path to the Texture.</param>
        /// <param name="width">The width of the NineSlice panel.</param>
        /// <param name="height">The height of the NineSlice panel.</param>
        /// <param name="fillRect">The rectangle to determine the stretched areas.</param>
        public NineSlice(string source, int width = 0, int height = 0, Rectangle? fillRect = null)
            : base() {
            SetTexture(new Texture(source));
            Initialize(source, width, height, fillRect);
        }

        /// <summary>
        /// Create a new NineSlice with a Texture.
        /// </summary>
        /// <param name="texture">The Texture to use.</param>
        /// <param name="width">The width of the NineSlice panel.</param>
        /// <param name="height">The height of the NineSlice panel.</param>
        /// <param name="fillRect">The rectangle to determine the stretched areas.</param>
        public NineSlice(Texture texture, int width, int height, Rectangle? fillRect = null)
            : base() {
            SetTexture(texture);
            Initialize(texture.Source, width, height, fillRect);
        }

        /// <summary>
        /// Create a new NineSlice with an AtlasTexture.
        /// </summary>
        /// <param name="texture">The AtlasTexture to use.</param>
        /// <param name="width">The width of the NineSlice panel.</param>
        /// <param name="height">The height of the NineSlice panel.</param>
        /// <param name="fillRect">The rectangle to determine the stretched areas.</param>
        public NineSlice(AtlasTexture texture, int width, int height, Rectangle? fillRect = null)
            : base() {
            SetTexture(texture);
            Initialize(texture.Name + ".png", width, height, fillRect);
        }

        #endregion

        #region Private Methods

        void Initialize(string source, int width, int height, Rectangle? fillRect) {
            tWidth = TextureRegion.Width;
            tHeight = TextureRegion.Height;

            if (width == 0 || height == 0) {
                Width = tWidth;
                Height = tHeight;
            }
            else {
                Width = width;
                Height = height;
            }

            sliceX1 = tWidth / 3;
            sliceX2 = tWidth / 3 * 2;
            sliceY1 = tHeight / 3;
            sliceY2 = tHeight / 3 * 2;

            if (fillRect == null) {
                if (fillRects.ContainsKey(source)) {
                    var rect = fillRects[source];
                    SetFillRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
                }
            }
            else {
                var rect = fillRect.Value;
                SetFillRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
        }

        void DrawQuad(VertexArray v, float x1, float y1, float x2, float y2, float u1, float v1, float u2, float v2) {
            float cx1 = x1, cx2 = x2, cy1 = y1, cy2 = y2;
            float cu1 = u1, cu2 = u2, cv1 = v1, cv2 = v2;

            if (usePanelClip) {
                cx1 = Util.Clamp(x1, ClippingRegion.Left, ClippingRegion.Right);
                cu1 = Util.ScaleClamp(cx1, x1, x2, u1, u2);

                cx2 = Util.Clamp(x2, ClippingRegion.Left, ClippingRegion.Right);
                cu2 = Util.ScaleClamp(cx2, x1, x2, u1, u2);

                cy1 = Util.Clamp(y1, ClippingRegion.Top, ClippingRegion.Bottom);
                cv1 = Util.ScaleClamp(cy1, y1, y2, v1, v2);

                cy2 = Util.Clamp(y2, ClippingRegion.Top, ClippingRegion.Bottom);
                cv2 = Util.ScaleClamp(cy2, y1, y2, v1, v2);
            }

            v.Append(cx1, cy1, Color, cu1, cv1);
            v.Append(cx2, cy1, Color, cu2, cv1);
            v.Append(cx2, cy2, Color, cu2, cv2);
            v.Append(cx1, cy2, Color, cu1, cv2);
        }

        protected override void UpdateDrawable() {
            var minWidth = sliceX1 + tWidth - sliceX2;
            panelScaleX = (float)Width / (float)minWidth;
            if (panelScaleX > 1) panelScaleX = 1;

            var minHeight = sliceY1 + tHeight - sliceY2;
            panelScaleY = (float)Height / (float)minHeight;
            if (panelScaleY > 1) panelScaleY = 1;

            var v = new VertexArray(PrimitiveType.Quads);

            int x0, x1, x2, x3, y0, y1, y2, y3;
            int u0, u1, u2, u3, v0, v1, v2, v3;
            x0 = 0;
            y0 = 0;
            x1 = sliceX1;
            y1 = sliceY1;
            x2 = Width - (tWidth - sliceX2);
            y2 = Height - (tHeight - sliceY2);
            x3 = Width;
            y3 = Height;

            u0 = TextureLeft;
            v0 = TextureTop;
            u1 = TextureLeft + sliceX1;
            v1 = TextureTop + sliceY1;
            u2 = TextureLeft + sliceX2;
            v2 = TextureTop + sliceY2;
            u3 = TextureLeft + tWidth;
            v3 = TextureTop + tHeight;

            if (panelScaleX < 1) {
                x1 = (int)Math.Round(x1 * panelScaleX);
                x2 = x1;
            }
            if (panelScaleY < 1) {
                y1 = (int)Math.Round(y1 * panelScaleY);
                y2 = y1;
            }

            int tileX = sliceX2 - sliceX1;
            int tileY = sliceY2 - sliceY1;

            if (PanelType == PanelType.Stretch) {
                //top
                DrawQuad(v, x1, y0, x2, y1, u1, v0, u2, v1);

                //left
                DrawQuad(v, x0, y1, x1, y2, u0, v1, u1, v2);

                //right
                DrawQuad(v, x2, y1, x3, y2, u2, v1, u3, v2);

                //bottom
                DrawQuad(v, x1, y2, x2, y3, u1, v2, u2, v3);

                //middle
                DrawQuad(v, x1, y1, x2, y2, u1, v1, u2, v2);
            }
            else {
                for (int xx = x1; xx < x2; xx += tileX) {
                    for (int yy = y1; yy < y2; yy += tileY) {
                        //middle
                        DrawQuad(v, xx, yy, xx + tileX, yy + tileY, u1, v1, u2, v2);
                    }
                }

                for (int yy = y1; yy < y2; yy += tileY) {
                    //left
                    DrawQuad(v, x0, yy, x1, yy + tileY, u0, v1, u1, v2);

                    //right
                    DrawQuad(v, x2, yy, x3, yy + tileY, u2, v1, u3, v2);
                }

                for (int xx = x1; xx < x2; xx += tileX) {
                    //top
                    DrawQuad(v, xx, y0, xx + tileX, y1, u1, v0, u2, v1);

                    //bottom
                    DrawQuad(v, xx, y2, xx + tileX, y3, u1, v2, u2, v3);
                }
            }

            //top left
            DrawQuad(v, x0, y0, x1, y1, u0, v0, u1, v1);

            //top right
            DrawQuad(v, x2, y0, x3, y1, u2, v0, u3, v1);

            //bottom left
            DrawQuad(v, x0, y2, x1, y3, u0, v2, u1, v3);

            //bottom right
            DrawQuad(v, x2, y2, x3, y3, u2, v2, u3, v3);

            SFMLVertices = v;
        }

        void Append(VertexArray v, float x, float y, float tx, float ty) {
            v.Append(new Vertex(new Vector2f(x, y), new Vector2f(tx, ty)));
        }

        int GetSnapWidth(int width) {
            var sliceWidth = sliceX2 - sliceX1;
            int snapWidth = Width - sliceWidth;

            while (snapWidth < width) {
                snapWidth += sliceWidth;
            }

            return snapWidth;
        }

        int GetSnapHeight(int height) {
            var sliceHeight = sliceY2 - sliceY1;
            int snapHeight = Width - sliceHeight;

            while (snapHeight < height) {
                snapHeight += sliceHeight;
            }

            return snapHeight;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the FillRect of the NineSlice.  This determines which areas are stretched or tiled when rendering the tiles.
        /// </summary>
        /// <param name="x1">The left corner of the rectangle.</param>
        /// <param name="y1">The top corner of the rectangle.</param>
        /// <param name="x2">The right corner of the rectangle.</param>
        /// <param name="y2">The bottom corner of the rectangle.</param>
        /// <returns>The NineSlice object.</returns>
        public NineSlice SetFillRect(int x1, int y1, int x2, int y2) {
            sliceX1 = x1;
            sliceX2 = x2;
            sliceY1 = y1;
            sliceY2 = y2;
            return this;
        }

        /// <summary>
        /// Get the FillRect of the NineSlice.  This determines which areas are stretched or tiled when rendering the tiles.
        /// </summary>
        /// <returns>The Rectangle of the FillRect.</returns>
        public Rectangle GetFillRect() {
            return new Rectangle(sliceX1, sliceY1, sliceX2 - sliceX1, sliceY2 - sliceY1);
        }

        /// <summary>
        /// Set the FillRect of the NineSlice using padding values.
        /// </summary>
        /// <param name="top">How far from the top of the texture to begin the rectangle.</param>
        /// <param name="right">How far from the right of the texture to end the rectangle.</param>
        /// <param name="bottom">How far from the bottom of the texture to end the rectangle.</param>
        /// <param name="left">How far from the left of the texture to begin the rectangle.</param>
        /// <returns>The NineSlice object.</returns>
        public NineSlice SetBorderPadding(int top, int right, int bottom, int left) {
            var x1 = left;
            var y1 = top;
            var x2 = Texture.Width - right;
            var y2 = Texture.Height - bottom;
            return SetFillRect(x1, y1, x2, y2);
        }

        /// <summary>
        /// Set the FillRect of the NineSlice using padding values.
        /// </summary>
        /// <param name="padding">How far from the border of the texture to make the rectangle.</param>
        /// <returns>The NineSlice object.</returns>
        public NineSlice SetBorderPadding(int padding) {
            return SetBorderPadding(padding, padding, padding, padding);
        }

        /// <summary>
        /// Set the FillRect of the NineSlice using padding values.
        /// </summary>
        /// <param name="horizontal">How far horizontally from the border of the texture to make the rectangle.</param>
        /// <param name="vertical">How far horizontally from the border of the texture to make the rectangle.</param>
        /// <returns>The NineSlice object.</returns>
        public NineSlice SetBorderPadding(int horizontal, int vertical) {
            return SetBorderPadding(horizontal, vertical, horizontal, vertical);
        }

        /// <summary>
        /// Draw the NineSlice.
        /// </summary>
        /// <param name="x">The X position offset.</param>
        /// <param name="y">The Y position offset.</param>
        public override void Render(float x = 0, float y = 0) {
            float ox = 0, oy = 0;

            if (UseInsideOrigin) {
                ox = -sliceX1;
                oy = -sliceY1;
            }

            base.Render(x + ox, y + oy);
        }

        #endregion
        
    }

    #region Enum

    public enum PanelType {
        Stretch,
        Tile
    }

    public enum PanelSizeMode {
        All,
        Inside
    }

    #endregion
}
