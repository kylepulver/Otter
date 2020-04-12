using SFML.Graphics;
using SFML.System;

using Otter.Utility;

namespace Otter.Graphics.Drawables
{
    /// <summary>
    /// A class containing all the info to describe a specific tile.
    /// </summary>
    public class TileInfo
    {
        #region Public Fields

        /// <summary>
        /// The X position of the tile.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y position of the tile.
        /// </summary>
        public int Y;

        /// <summary>
        /// The X position of the source texture to render the tile from.
        /// </summary>
        public int TX;

        /// <summary>
        /// The Y position of the source texture to render the tile from.
        /// </summary>
        public int TY;

        /// <summary>
        /// The width of the tile.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the tile.
        /// </summary>
        public int Height;

        /// <summary>
        /// Flipped tile options.
        /// </summary>
        public bool FlipX;
        public bool FlipY;

        /// <summary>
        /// Flips the tile anti-diagonally, equivalent to a 90 degree rotation and a horizontal flip.
        /// Combined with FlipX and FlipY you can rotate the tile any direction.
        /// </summary>
        public bool FlipD;

        /// <summary>
        /// The color of the tile, or the color to tint the texture.
        /// </summary>
        public Color Color;

        /// <summary>
        /// The alpha of the tile.
        /// </summary>
        public float Alpha
        {
            get { return Color.A; }
            set { Color.A = value; }
        }

        #endregion

        #region Constructors

        public TileInfo(int x, int y, int tx, int ty, int width, int height, Color color = null, float alpha = 1)
        {
            X = x;
            Y = y;
            TX = tx;
            TY = ty;
            Width = width;
            Height = height;
            if (color == null)
            {
                Color = Color.White;
            }
            else
            {
                Color = color;
            }
            Alpha = alpha;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the index of the tile on the source Texture of a Tilemap.
        /// </summary>
        /// <param name="tilemap">The Tilemap that uses the Texture to be tested against.</param>
        /// <returns>The index of the tile on the Tilemap's Texture.</returns>
        public int GetIndex(Tilemap tilemap)
        {
            return Util.OneDee(tilemap.Texture.Width / Width, TX / Width, TY / Height);
        }

        #endregion

        #region Internal

        internal Color tilemapColor = new Color();

        internal Vector2f SFMLPosition
        {
            get { return new Vector2f(X, Y); }
        }

        internal Vector2f SFMLTextureCoord
        {
            get { return new Vector2f(TX, TY); }
        }

        internal Vertex CreateVertex(int x = 0, int y = 0, int tx = 0, int ty = 0)
        {
            var tileColor = new Color(Color);
            tileColor *= tilemapColor;
            if (TX == -1 || TY == -1)
            {
                return new Vertex(new Vector2f(X + x, Y + y), tileColor.SFMLColor);
            }
            return new Vertex(new Vector2f(X + x, Y + y), tileColor.SFMLColor, new Vector2f(TX + tx, TY + ty));
        }

        internal void AppendVertices(VertexArray array)
        {
            if (!FlipD)
            {
                if (!FlipX && !FlipY)
                {
                    array.Append(CreateVertex(0, 0, 0, 0)); //upper-left
                    array.Append(CreateVertex(Width, 0, Width, 0)); //upper-right
                    array.Append(CreateVertex(Width, Height, Width, Height)); //lower-right
                    array.Append(CreateVertex(0, Height, 0, Height)); //lower-left
                }
                if (FlipX && FlipY)
                {
                    array.Append(CreateVertex(0, 0, Width, Height));
                    array.Append(CreateVertex(Width, 0, 0, Height));
                    array.Append(CreateVertex(Width, Height, 0, 0));
                    array.Append(CreateVertex(0, Height, Width, 0));
                }
                if (FlipX & !FlipY)
                {
                    array.Append(CreateVertex(0, 0, Width, 0));
                    array.Append(CreateVertex(Width, 0, 0, 0));
                    array.Append(CreateVertex(Width, Height, 0, Height));
                    array.Append(CreateVertex(0, Height, Width, Height));
                }
                if (!FlipX & FlipY)
                {
                    array.Append(CreateVertex(0, 0, 0, Height));
                    array.Append(CreateVertex(Width, 0, Width, Height));
                    array.Append(CreateVertex(Width, Height, Width, 0));
                    array.Append(CreateVertex(0, Height, 0, 0));
                }
            }
            else
            { //swaps lower-left corner with upper-right on all the cases
                if (!FlipX && !FlipY)
                {
                    array.Append(CreateVertex(0, 0, 0, 0)); //upper-left
                    array.Append(CreateVertex(0, Height, Width, 0)); //upper-right
                    array.Append(CreateVertex(Width, Height, Width, Height)); //lower-right
                    array.Append(CreateVertex(Width, 0, 0, Height)); //lower-left
                }
                if (FlipX && FlipY)
                {
                    array.Append(CreateVertex(0, 0, Width, Height));
                    array.Append(CreateVertex(0, Height, 0, Height));
                    array.Append(CreateVertex(Width, Height, 0, 0));
                    array.Append(CreateVertex(Width, 0, Width, 0));
                }
                if (!FlipX & FlipY)
                {
                    array.Append(CreateVertex(0, 0, Width, 0));
                    array.Append(CreateVertex(0, Height, 0, 0));
                    array.Append(CreateVertex(Width, Height, 0, Height));
                    array.Append(CreateVertex(Width, 0, Width, Height));
                }
                if (FlipX & !FlipY)
                {
                    array.Append(CreateVertex(0, 0, 0, Height));
                    array.Append(CreateVertex(0, Height, Width, Height));
                    array.Append(CreateVertex(Width, Height, Width, 0));
                    array.Append(CreateVertex(Width, 0, 0, 0));
                }
            }
        }
        #endregion
    }
}
