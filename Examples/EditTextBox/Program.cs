using Otter.Core;
using Otter.Graphics;

namespace TextEditBox
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an Otter game with default parameters.
            using(var game = new Game())
            {
                // Set the background color.
                game.Color = new Color(0.2f, 0.2f, 0.5f);
                // Want to show the mouse for this example.
                game.MouseVisible = true;

                // Create a new scene.
                var scene = new Scene();

                // Add some text edit boxes to the scene at various positions.
                scene.Add(new TextEditBox(100, 100));
                scene.Add(new TextEditBox(100, 200));
                scene.Add(new TextEditBox(100, 300));

                // Start up the game using the scene we just made.
                game.Start(scene);
            }
        }
    }
}
