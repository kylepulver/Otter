using Otter.Core;

namespace CoroutineExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var game = new Game())
            {
                game.FirstScene = new CoroutineScene();
                game.Start();
            }
        }
    }
}
