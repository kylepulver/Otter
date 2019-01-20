using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTilingExample {
    class Cursor : Entity {

        /// <summary>
        /// The image that will show the exact mouse position.
        /// </summary>
        Image imageCenter = Image.CreateCircle(2, Color.White);

        /// <summary>
        /// The image that will show the tile position.
        /// </summary>
        Image imageTile = Image.CreateRectangle(Tiles.GridSize, Color.None);

        public Cursor() : base() {
            // Center the imageCenter graphic and give it an outline.
            imageCenter.CenterOrigin();
            imageCenter.OutlineColor = Color.Black;
            imageCenter.OutlineThickness = 1;

            // Set up the outline of the imageTile.
            imageTile.OutlineColor = Color.Yellow;
            imageTile.OutlineThickness = 1;

            // The imageTile will be rendering based off its own X and Y so set relative to false.
            imageTile.Relative = false;

            // Add the graphics.
            AddGraphic(imageTile);
            AddGraphic(imageCenter);

            // Put the cursor on top of everything.
            Layer = -100;
        }

        public override void Update() {
            base.Update();

            // Set the default visibility states for the images.
            imageCenter.Visible = true;
            imageTile.Visible = false;

            // If either mouse button is down hide the circle image, and show the tile image.
            if (Input.MouseButtonDown(MouseButton.Left) || Input.MouseButtonDown(MouseButton.Right)) {
                imageCenter.Visible = false;
                imageTile.Visible = true;
            }

            // Set the entity's X and Y position to the cursor.
            X = Input.MouseX;
            Y = Input.MouseY;

            // Set the imageTile position to the entity's coordinates but snapped to the grid size.
            imageTile.X = Util.SnapToGrid(X, Tiles.GridSize);
            imageTile.Y = Util.SnapToGrid(Y, Tiles.GridSize);
        }
    }
}
