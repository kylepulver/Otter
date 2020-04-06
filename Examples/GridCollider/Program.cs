using Otter;

namespace GridColliderPlayerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();    // Create an Otter game.             
            game.FirstScene = new PlayScene();  // Create and set our First Scene for Otter to run.            
            game.Start(); // Start the game!
        }
    }
}
