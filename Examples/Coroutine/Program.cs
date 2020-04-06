using Otter;

namespace CoroutineExample {
    class Program {
        static void Main(string[] args) {
            var game = new Game();
            game.FirstScene = new CoroutineScene();
            game.Start();
        }
    }
}
