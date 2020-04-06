using Otter;

namespace PlatformingExample
{
    class Program
    {
        // Quick example on how to use Otter/PlatformingMovement
        static void Main(string[] args)
        {
            // Make a new game.
            var game = new Game("Platformer Example");

            // Start the game with a new PlatformerScene.
            game.Start(new PlatformerScene());
        }
    }
}
