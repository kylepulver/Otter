using Otter.Core;

namespace OtterPongGame
{
    class PongScene : Scene
    {
        public PongScene() : base()
        {
            Add(new Paddle(Global.PlayerOne));
            Add(new Paddle(Global.PlayerTwo));
            Add(new Ball());
        }
    }
}
