using Otter.Core;
using Otter.Colliders;
using Otter.Graphics;
using Otter.Graphics.Drawables;

namespace GridColliderPlayerTest
{
    class PlayScene : Scene
    {
        // The tilemap that will be used to show the grid collider.
        Tilemap tiles = new Tilemap(640, 480, 20, 20);

        // The grid collider that the player will collide with.
        GridCollider grid = new GridCollider(640, 480, 20, 20, 0);

        public PlayScene()
        {
            // Add the tilemap graphic to the scene.
            AddGraphic(tiles);

            // Create a new entity to hold the grid collider.  This lets us use Otter's collision system with the grid.
            var e = new Entity(0, 0, null, grid);

            // Add the new entity.
            Add(e);

            // Add the outside border.
            grid.SetRect(0, 0, 32, 24, true);
            grid.SetRect(1, 1, 30, 22, false);

            // Add some random rectangles.
            grid.SetRect(4, 4, 3, 3, true);
            grid.SetRect(0, 10, 10, 2, true);
            grid.SetRect(20, 20, 2, 2, true);
            grid.SetRect(25, 4, 2, 10, true);
            grid.SetRect(14, 6, 5, 1, true);

            // Load tiles to the tilemap from the grid.
            tiles.LoadGrid(grid, Color.White);

            // Add the player to the scene.
            Add(new Player(200, 200));
        }

    }
}
