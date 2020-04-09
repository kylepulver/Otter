using System;

using Otter.Components;
using Otter.Core;
using Otter.Graphics;
using Otter.Graphics.Drawables;
using Otter.Utility;
using Otter.Utility.MonoGame;

namespace GridColliderPlayerTest
{
    class Player : Entity
    {

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

        public Player(float x, float y) : base(x, y)
        {
            // Set the entity's graphic to the image of the rectangle.
            SetGraphic(image);

            // Create and use a simple hitbox the same size of the rectangle.
            SetHitbox(20, 20, 1);

            // Add the axis component so it updates and reads input.
            AddComponent(movementAxis);
        }

        public override void Update()
        {
            base.Update();

            // If the x axis is currently being used.
            if (movementAxis.X != 0)
            {
                // Accelerate the speed in the x direction.
                speed.X += acceleration * movementAxis.X;
            }
            else
            {
                // Otherwise make the speed in the x direction go back to zero.
                speed.X = Util.Approach(speed.X, 0, acceleration);
            }

            // If the y axis is currently being used.
            if (movementAxis.Y != 0)
            {
                // Accelerate the speed in the y direction.
                speed.Y += acceleration * movementAxis.Y;
            }
            else
            {
                // Otherwise make the speed in the y direction go back to zero.
                speed.Y = Util.Approach(speed.Y, 0, acceleration);
            }

            // Apply the speeds to the move buffer.
            moveBuffer.X += speed.X;
            moveBuffer.Y += speed.Y;

            // Will keep track of where to move during each collision check.
            float move;

            // While the move buffer has at least one pixel of movement stored, move the object and test for collisions.
            while (Math.Abs(moveBuffer.X) >= perPixel)
            {
                // Figure out which way the entity is moved (+1 or -1)
                move = Math.Sign(moveBuffer.X);

                // If there is no overlap with the tag 0 where we want to move
                if (!Overlap(X + move, Y, 0))
                {
                    // Then it's okay to move the X position of the entity by the "move" amount
                    X += move;
                }
                else
                {
                    // Otherwise, cancel all other X movement for this update (since there is a wall in the way.)
                    moveBuffer.X = 0;
                }

                // Reduce the move buffer by the per pixel amount.
                moveBuffer.X = Util.Approach(moveBuffer.X, 0, perPixel);
            }

            // Do the same thing for the Y movement.
            while (Math.Abs(moveBuffer.Y) >= perPixel)
            {
                move = Math.Sign(moveBuffer.Y);
                if (!Overlap(X, Y + move, 0))
                {
                    Y += move;
                }
                else
                {
                    moveBuffer.Y = 0;
                }

                moveBuffer.Y = Util.Approach(moveBuffer.Y, 0, perPixel);
            }
        }
    }

}
