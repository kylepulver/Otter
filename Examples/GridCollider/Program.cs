using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridColliderPlayerTest {
    class Program {
        static void Main(string[] args) {

            // Create an Otter game.
            var game = new Game();

            // Create and set our First Scene for Otter to run.
            game.FirstScene = new PlayScene();

            // Start the game!
            game.Start();

        }
    }

    class Player : Entity {

        // A default square image to use.
        Image image = Image.CreateRectangle(20, 20, Color.Red);

        // An axis to use to read input from the arrow keys.
        Axis movementAxis = new Axis(Key.Up, Key.Right, Key.Down, Key.Left);

        // Track the player's speed.  It can never exceed 300.
        Speed speed = new Speed(300);

        // How much to accelerate the speed by when taking input.
        int acceleration = 100;

        // How many units represents one pixel.  100 = 1 pixel.  300 = 3 pixels, etc.
        int perPixel = 100;

        // The buffered amount to move each update.
        Vector2 moveBuffer = new Vector2();

        public Player(float x, float y) : base(x, y) {
            // Set the entity's graphic to the image of the rectangle.
            SetGraphic(image);

            // Create and use a simple hitbox the same size of the rectangle.
            SetHitbox(20, 20, 1);

            // Add the axis component so it updates and reads input.
            AddComponent(movementAxis);
        }

        public override void Update() {
            base.Update();

            // If the x axis is currently being used.
            if (movementAxis.X != 0) {
                // Accelerate the speed in the x direction.
                speed.X += acceleration * movementAxis.X;
            }
            else {
                // Otherwise make the speed in the x direction go back to zero.
                speed.X = Util.Approach(speed.X, 0, acceleration);
            }

            // If the y axis is currently being used.
            if (movementAxis.Y != 0) {
                // Accelerate the speed in the y direction.
                speed.Y += acceleration * movementAxis.Y;
            }
            else {
                // Otherwise make the speed in the y direction go back to zero.
                speed.Y = Util.Approach(speed.Y, 0, acceleration);
            }

            // Apply the speeds to the move buffer.
            moveBuffer.X += speed.X;
            moveBuffer.Y += speed.Y;

            // Will keep track of where to move during each collision check.
            float move;

            // While the move buffer has at least one pixel of movement stored, move the object and test for collisions.
            while (Math.Abs(moveBuffer.X) >= perPixel) {
                // Figure out which way the entity is moved (+1 or -1)
                move = Math.Sign(moveBuffer.X);

                // If there is no overlap with the tag 0 where we want to move
                if (!Overlap(X + move, Y, 0)) {
                    // Then it's okay to move the X position of the entity by the "move" amount
                    X += move;
                }
                else {
                    // Otherwise, cancel all other X movement for this update (since there is a wall in the way.)
                    moveBuffer.X = 0;
                }

                // Reduce the move buffer by the per pixel amount.
                moveBuffer.X = Util.Approach(moveBuffer.X, 0, perPixel);
            }

            // Do the same thing for the Y movement.
            while (Math.Abs(moveBuffer.Y) >= perPixel) {
                move = Math.Sign(moveBuffer.Y);
                if (!Overlap(X, Y + move, 0)) {
                    Y += move;
                }
                else {
                    moveBuffer.Y = 0;
                }

                moveBuffer.Y = Util.Approach(moveBuffer.Y, 0, perPixel);
            }
        }

    }

    class PlayScene : Scene {

        // The tilemap that will be used to show the grid collider.
        Tilemap tiles = new Tilemap(640, 480, 20, 20);

        // The grid collider that the player will collide with.
        GridCollider grid = new GridCollider(640, 480, 20, 20, 0);

        public PlayScene() {
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
