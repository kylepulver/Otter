using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoroutineExample {
    class Program {
        static void Main(string[] args) {
            var game = new Game();
            game.FirstScene = new CoroutineScene();
            game.Start();
        }
    }
}
