using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class Program {
        static void Main(string[] args) {

            var game = new Game("Flippy Flop");
            game.Color = new Color("749ace");

            game.AddSession("Player");

            var c = game.Session("Player").Controller;

            game.GameFolder = "FlippyFlop";

            c.A.AddKey(Key.Z, Key.C, Key.X, Key.Space);

            game.FirstScene = new GameScene();

            game.Start();
        }
    }
}
