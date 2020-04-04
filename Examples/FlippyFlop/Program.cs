using Otter;

namespace FlippyFlop {
    class Program {
        static void Main(string[] args) {

            var game = new Game("Flippy Flop");
            game.Color = new Color("749ace");

            game.AddSession("Player");

            var c = game.Session("Player").Controller;

            game.GameFolder = "FlippyFlop";

            c.AddButton(Key.Z, Key.C, Key.X, Key.Space);

            game.FirstScene = new GameScene();

            game.Start();
        }
    }
}
