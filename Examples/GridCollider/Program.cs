
using Otter.Core;

namespace GridColliderPlayerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an Otter game.
            using(var game = new Game())
            {
                game.FirstScene = new PlayScene();  // Create and set our First Scene for Otter to run.
                game.Start(); // Start the game!
            }
        }
    }
}
