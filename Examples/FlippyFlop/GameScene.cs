using Otter;
using Otter.Core;

namespace FlippyFlop
{
    class GameScene : Scene
    {
        public Image ImageTop = Image.CreateRectangle(640, 50, new Color("364298"));
        public Image ImageBottom = Image.CreateRectangle(640, 50, new Color("364298"));

        public GameScene()
        {
            AddGraphics(ImageTop, ImageBottom);
            ImageBottom.Y = Game.Instance.Height - ImageBottom.Height;
            Add(new GameManager());
        }
    }
}
