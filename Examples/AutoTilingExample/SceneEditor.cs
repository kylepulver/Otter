using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTilingExample {
    class SceneEditor : Scene {

        /// <summary>
        /// The entity that holds the tiles and grid collider.
        /// </summary>
        Tiles tiles = new Tiles();

        /// <summary>
        /// The entity that represents the mouse cursor.
        /// </summary>
        Cursor cursor = new Cursor();

        /// <summary>
        /// Some instructions to add to the background.
        /// </summary>
        Text textHelp = new Text("[Left Click: Add] [Right Click: Remove]", 10);

        /// <summary>
        /// Create a new editor scene.  Set the width and height to 320 x 240.
        /// </summary>
        public SceneEditor() : base(320, 240) {
            // Add the tiles to the scene.
            Add(tiles);

            // Add the cursor to the scene.
            Add(cursor);

            // Add the instructions text graphic.
            AddGraphic(textHelp);

            // Center the origin of the text.
            textHelp.CenterTextOrigin();

            // Set the origins to int to prevent blurry text at small resolutions.
            textHelp.OriginX = (int)textHelp.OriginX;
            textHelp.OriginY = (int)textHelp.OriginY;

            // Position the instruction text.
            textHelp.X = 160;
            textHelp.Y = 6;
        }

        public override void Update() {
            base.Update();

            // If the left mouse button is down place a tile at the mouse position.
            if (Input.MouseButtonDown(MouseButton.Left)) {
                tiles.PlaceTile(Input.MouseX, Input.MouseY);
            }

            // If the right mouse button is down remove a tile at the mouse position.
            if (Input.MouseButtonDown(MouseButton.Right)) {
                tiles.RemoveTile(Input.MouseX, Input.MouseY);
            }
        }
    }
}
