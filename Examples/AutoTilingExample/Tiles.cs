using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTilingExample {
    class Tiles : Entity {

        /// <summary>
        /// The GridCollider that the auto tiling will use.
        /// </summary>
        public GridCollider GridCollider;

        /// <summary>
        /// The Tilemap that will use auto tiling.
        /// </summary>
        public Tilemap Tilemap;

        /// <summary>
        /// The size of the grid.
        /// </summary>
        public static int GridSize = 8;

        /// <summary>
        /// Determines if the tiles need to be updated because something changed.
        /// </summary>
        bool needsUpdate = false;

        /// <summary>
        /// Nothing special needed for the constructor.
        /// </summary>
        public Tiles() : base() {

        }

        public override void Added() {
            base.Added();

            // Create the grid collider based off of the scene's dimensions.
            GridCollider = new GridCollider(Scene.Width, Scene.Height, GridSize, GridSize);

            // Create the tilemap based off of the scene's dimensions.
            Tilemap = new Tilemap("tiles.png", Scene.Width, Scene.Height, GridSize, GridSize);

            // Add the tilemap graphic.
            AddGraphic(Tilemap);

            // Add the grid collider.
            AddCollider(GridCollider);
        }

        public void PlaceTile(int x, int y) {
            // Convert the x and y to a grid position.
            x = (int)Util.Floor(x / GridSize);
            y = (int)Util.Floor(y / GridSize);

            // Place a tile if a tile isn't already there.
            if (!GridCollider.GetTile(x, y)) {
                GridCollider.SetTile(x, y, true);
                needsUpdate = true;
            }
        }

        public void RemoveTile(int x, int y) {
            // Convert the x and y to a grid position.
            x = (int)Util.Floor(x / GridSize);
            y = (int)Util.Floor(y / GridSize);

            // Remove a tile if a tile is there.
            if (GridCollider.GetTile(x, y)) {
                GridCollider.SetTile(x, y, false);
                needsUpdate = true;
            }
        }


        public override void Update() {
            base.Update();

            // If the tiles need to update then clear them and reload the grid using auto tiling.
            if (needsUpdate) {
                needsUpdate = false;
                Tilemap.ClearAll();
                Tilemap.LoadGridAutoTile(GridCollider);
            }
        }
        
    }
}
