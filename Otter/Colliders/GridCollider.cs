using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Grid Collider.  Can be mainly used to create collision to correspond to a Tilemap.
    /// </summary>
    public class GridCollider : Collider {

        #region Private Fields

        List<bool> collisions = new List<bool>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The width of the tiles.
        /// </summary>
        public int TileWidth { get; private set; }

        /// <summary>
        /// The height of the tiles.
        /// </summary>
        public int TileHeight { get; private set; }

        /// <summary>
        /// The total number of rows on the grid.
        /// </summary>
        public int TileRows { get; private set; }

        /// <summary>
        /// The total number of columns on the grid.
        /// </summary>
        public int TileColumns { get; private set; }

        /// <summary>
        /// The width of the grid. (TileColumns * TileWidth)
        /// </summary>
        public override float Width {
            get { return TileColumns * TileWidth; }
        }

        /// <summary>
        /// The height of the grid (TileRows * TileHeight)
        /// </summary>
        public override float Height {
            get { return TileRows * TileHeight; }
        }

        /// <summary>
        /// Convert an X position to a tile on the grid.
        /// </summary>
        /// <param name="x">The position to convert.</param>
        /// <returns>The X position of the tile that was found at that position.</returns>
        public int GridX(float x) {
            return (int)(Util.SnapToGrid(x - X - Entity.X, TileWidth) / TileWidth);
        }

        /// <summary>
        /// Convert an Y position to a tile on the grid.
        /// </summary>
        /// <param name="y">The position to convert.</param>
        /// <returns>The Y position of the tile that was found at that position.</returns>
        public int GridY(float y) {
            return (int)(Util.SnapToGrid(y - y - Entity.Y, TileHeight) / TileHeight);
        }

        /// <summary>
        /// The area in tile size.
        /// </summary>
        public int TileArea {
            get { return TileRows * TileHeight; }
        }

        /// <summary>
        /// The area in pixels.
        /// </summary>
        public int Area {
            get { return (int)(Width * Height); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new GridCollider.
        /// </summary>
        /// <param name="width">The width in pixels of the GridCollider.</param>
        /// <param name="height">The height in pixels of the GridCollider.</param>
        /// <param name="tileWidth">The width of each tile on the grid.</param>
        /// <param name="tileHeight">The heigth of each tile on the grid.</param>
        /// <param name="tags">The tags to register for the collider.</param>
        public GridCollider(int width, int height, int tileWidth, int tileHeight, params int[] tags) {
            if (width < 0) throw new ArgumentOutOfRangeException("Width must be greater than 0.");
            if (height < 0) throw new ArgumentOutOfRangeException("Height must be greater than 0.");

            TileColumns = (int)Util.Ceil((float)width / tileWidth);
            TileRows = (int)Util.Ceil((float)height / tileHeight);
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            for (int i = 0; i < TileRows * TileColumns; i++) {
                collisions.Add(false);
            }

            AddTag(tags);
        }

        public GridCollider(int width, int height, int tileWidth, int tileHeight, Enum tag, params Enum[] tags) : this(width, height, tileWidth, tileHeight) {
            AddTag(tag);
            AddTag(tags);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the collision status of a tile on the GridCollider.
        /// </summary>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <param name="collidable">True if collidable.</param>
        public void SetTile(int x, int y, bool collidable = true) {
            if (x < 0 || y < 0) return;
            if (x >= TileColumns || y >= TileRows) return;
            collisions[Util.OneDee((int)TileColumns, (int)x, (int)y)] = collidable;
        }

        /// <summary>
        /// Set the collision status of a rectangle area on the GridCollider.
        /// </summary>
        /// <param name="x">The X position of the top left corner of the rectangle.</param>
        /// <param name="y">The Y position of the top left corner of the rectangle.</param>
        /// <param name="width">The width in tiles of the rectangle.</param>
        /// <param name="height">The height in tiles of the rectangle.</param>
        /// <param name="collidable">True if collidable.</param>
        public void SetRect(int x, int y, int width, int height, bool collidable = true) {
            for (int i = x; i < x + width; i++) {
                for (int j = y; j < y + height; j++) {
                    SetTile(i, j, collidable);
                }
            }
        }

        /// <summary>
        /// Clears the entire grid.
        /// </summary>
        /// <param name="collidable">Optionally set to true to clear grid with collidable cells.</param>
        public void Clear(bool collidable = false) {
            for (var i = 0; i < collisions.Count; i++) {
                collisions[i] = collidable;
            }
        }

        /// <summary>
        /// Get the collision status of a tile on the GridCollider.
        /// </summary>
        /// <param name="x">The X position of the tile.</param>
        /// <param name="y">The Y position of the tile.</param>
        /// <returns>True if the tile is collidable.</returns>
        public bool GetTile(int x, int y) {
            if (x < 0 || y < 0) return false;
            if (x >= TileColumns || y >= TileRows) return false;
            var index = Util.OneDee((int)TileColumns, (int)x, (int)y);
            if (index >= collisions.Count) return false;
            return collisions[index];
        }

        /// <summary>
        /// Get the collision status of a tile at a position in the Scene.
        /// </summary>
        /// <param name="x">The X position in the Scene.</param>
        /// <param name="y">The Y position in the Scene.</param>
        /// <returns>True if the tile at that position is collidable.</returns>
        public bool GetTileAtPosition(float x, float y) {
            var ox = x;
            var oy = y;

            x -= Left;
            y -= Top;

            x = (float)Math.Floor(x / TileWidth);
            y = (float)Math.Floor(y / TileHeight);

            return GetTile((int)x, (int)y);
        }

        /// <summary>
        /// Check for a collidable tile in a rectangle on the GridCollider.
        /// </summary>
        /// <param name="x">The X position of the top left corner of the rectangle.</param>
        /// <param name="y">The Y position of the top left corner of the rectangle.</param>
        /// <param name="width">The width in tiles of the rectangle.</param>
        /// <param name="height">The height in tiles of the rectangle.</param>
        /// <param name="collidable">True if any tile in the rectangle is collidable.</param>
        public bool GetRect(float x, float y, float x2, float y2, bool usingGrid = true) {
            //adjust for grid position
            x -= Left;
            x2 -= Left;
            y -= Top;
            y2 -= Top;

            if (!usingGrid) {
                x = (int)(Util.SnapToGrid(x, TileWidth) / TileWidth);
                y = (int)(Util.SnapToGrid(y, TileHeight) / TileHeight);
                x2 = (int)(Util.SnapToGrid(x2, TileWidth) / TileWidth);
                y2 = (int)(Util.SnapToGrid(y2, TileHeight) / TileHeight);
            }

            for (int i = (int)x; i <= x2; i++) {
                for (int j = (int)y; j <= y2; j++) {
                    if (GetTile(i, j)) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Convert a string into tiles on the GridCollider.
        /// </summary>
        /// <param name="source">The source string data.</param>
        /// <param name="empty">The character representing an empty space on the grid.</param>
        /// <param name="filled">The character representing a collidable space on the grid.</param>
        public void LoadString(string source, char empty = '0', char filled = '1') {
            int xx = 0, yy = 0;

            for (int i = 0; i < source.Length; i++) {
                if (source[i] != empty && source[i] != filled) continue;

                if (xx == TileColumns) {
                    xx = 0;
                    yy++;
                }

                SetTile(xx, yy, source[i] == filled);

                xx++;
            }
        }

        /// <summary>
        /// Convert a CSV file into tiles on the GridCollider.
        /// </summary>
        /// <param name="source">The source CSV data.</param>
        /// <param name="empty">String representing an empty space on the grid.</param>
        /// <param name="filled">String representing a collidable space on the grid.</param>
        /// <param name="columnSep">The separator character for columns.</param>
        /// <param name="rowSep">The separator character for rows.</param>
        public void LoadCSV(string source, string empty = "0", string filled = "1", char columnSep = ',', char rowSep = '\n') {
            string[] row = source.Split(rowSep);
            int rows = row.Length;
            string[] col;
            int cols;
            int x;
            int y;

            for (y = 0; y < rows; y++) {
                if (row[y] == "") {
                    continue;
                }

                col = row[y].Split(columnSep);
                cols = col.Length;
                for (x = 0; x < cols; x++) {
                    if (col[x].Equals("") || !col[x].Equals(filled)) {
                        continue;
                    }

                    SetTile(x, y, col[x].Equals(filled));
                }
            }
        }

        /// <summary>
        /// Load collision data from a Tilemap.
        /// </summary>
        /// <param name="source">The source Tilemap.</param>
        /// <param name="layerDepth">The layer of tiles to use to mark collisions.</param>
        public void LoadTilemap(Tilemap source, int layerDepth) {
            if (source.Width != Width) throw new ArgumentException("Tilemap size and tile size must match grid size and tile size.");
            if (source.Height != Height) throw new ArgumentException("Tilemap size and tile size must match grid size and tile size.");
            if (source.TileWidth != TileWidth) throw new ArgumentException("Tilemap size and tile size must match grid size and tile size.");
            if (source.TileWidth != TileWidth) throw new ArgumentException("Tilemap size and tile size must match grid size and tile size.");
            foreach(var t in source.GetTiles(layerDepth)) {
                SetTile(t.X / TileWidth, t.Y / TileHeight, true);
            }
        }

        /// <summary>
        /// Load collision data from a Tilemap.
        /// </summary>
        /// <param name="source">The source Tilemap.</param>
        /// <param name="layerName">The layer of tiles to use to mark collisions.</param>
        public void LoadTilemap(Tilemap source, string layerName) {
            LoadTilemap(source, source.LayerDepth(layerName));
        }

        /// <summary>
        /// Load collision data from a Tilemap.
        /// </summary>
        /// <param name="source">The source Tilemap.</param>
        /// <param name="layerName">The layer of tiles to use to mark collisions.</param>
        public void LoadTilemap(Tilemap source, Enum layerName) {
            LoadTilemap(source, source.LayerDepth(layerName));
        }

        /// <summary>
        /// Draw the collider for debug purposes.
        /// </summary>
        public override void Render(Color color = null) {
            base.Render(color);
            if (color == null) color = Color.Red;

            if (Entity == null) return;

            float viewLeft = Game.Instance.Scene.CameraX - TileWidth;
            float viewRight = Game.Instance.Scene.CameraX + Game.Instance.Scene.CameraWidth;
            float viewTop = Game.Instance.Scene.CameraY - TileHeight;
            float viewBottom = Game.Instance.Scene.CameraY + Game.Instance.Scene.CameraHeight;

            for (int i = 0; i < TileColumns; i++) {
                for (int j = 0; j < TileRows; j++) {

                    if (Left + i * TileWidth > viewLeft && Left + i * TileWidth < viewRight && Top + j * TileHeight > viewTop  &&  Top + j * TileHeight < viewBottom )
                    {
                        if (GetTile(i, j))
                        {
                            Draw.Rectangle(Left + i * TileWidth + 1, Top + j * TileHeight + 1, TileWidth - 2, TileHeight - 2, Color.None, color, 1f);
                        }
                    }
                }
            }
        }

        #endregion       

    }
}
