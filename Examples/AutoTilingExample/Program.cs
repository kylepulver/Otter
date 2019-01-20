using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTilingExample {
    class Program {
        /// <summary>
        /// This program serves as an example on how to get started with Otter's auto tiling functionality.
        /// 
        /// The auto tiling uses a texture and a data file to determine which tile to use.  If there is no
        /// data file included then the default data will be used.  To see the default data go to the
        /// Tilemap.cs class and find the field autoTileDefaultData.  There are 47 different possible tiles
        /// that can be placed with the current algorithm.
        /// 
        /// The data is arranged by specificing the tile index first, and then the layout for the tile's
        /// neighbors.  Here's an example from the tile data:
        /// 
        /// 3:
        /// ? 0 ?
        /// 1 x 0
        /// ? 0 ?
        /// =
        /// 
        /// This is a single entry in the data.  This is for placing the tile with an index of 3 from the
        /// tileset.  This tile will be placed if the neighbors match the 3 x 3 grid of characters.
        /// 
        /// A "?" character means that this tile can be occupied, or not.
        /// 
        /// A "0" character means that this tile MUST be empty.
        /// 
        /// A "1" character means that this tile MUST be occupied.
        /// 
        /// A "x" character represents the tile itself, and the surrounding 8 tiles are the neighbors.
        /// 
        /// So whenever the auto tiling algorithm finds a tile with neighbors matching this data the tile
        /// with an index of 3 will be placed in that spot.
        /// 
        /// To see how tile data is arranged for using the default data set check out the tiles.png file
        /// also included with this example.
        /// 
        /// You can also list multiple tiles for the same set of neighbors.  For example:
        /// 
        /// 3, 4, 5, 6:
        /// ? 0 ?
        /// 1 x 0
        /// ? 0 ?
        /// =
        /// 
        /// This means that instead of placing the tile with the index of 3 it instead chooses randomly to
        /// place tiles 3, 4, 5, or 6.  You can list the same tile multiple times to change the weights.
        /// For example listing 3, 3, 3, 4, 4, 5, 6 also works, and the result is that 3 is more likely
        /// to appear in the random choice.
        /// 
        /// To load your own custom data use the function SetAutoTileData and pass in a string of data to
        /// use. Make sure it is formatted properly, or you might get a crash or crazy results.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {

            // Create a new game.
            var game = new Game("AutoTiling Example", 320, 240);

            // Scale up the window since it's a small resolution.
            game.SetWindowScale(3);

            // Set the background color of the game.
            game.Color = new Color(0.3f, 0.2f, 0.2f);

            // Start the game with a new SceneEditor scene.
            game.Start(new SceneEditor());

        }


    }
}
