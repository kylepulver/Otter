using Otter.Core;
using Otter.Graphics;

namespace FlippyFlop
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var game = new Game("Flippy Flop"))
            {
                game.Color = new Color("749ace");
                game.GameFolder = "FlippyFlop";

                game.AddSession("Player");
                var c = game.Session("Player").Controller;
                c.Enabled = true;
                c.AddButton("Action");
                c.Button("Action").AddKey(Key.Space);

                game.FirstScene = new GameScene();
                game.Start();
            }
        }
    }
}
