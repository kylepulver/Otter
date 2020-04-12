using Otter.Core;
using Otter.Graphics;
using Otter.Graphics.Drawables;

namespace FlippyFlop
{
    class FlippyTrail : Entity
    {

        public Image Image = Image.CreateRectangle(20, Color.Orange);

        public FlippyTrail(float x, float y) : base(x, y)
        {
            Graphic = Image;

            Image.CenterOrigin();

            Layer = 100;

            Tween(Image, new { ScaleX = 0, ScaleY = 0 }, 30).OnComplete(() => RemoveSelf());
        }

        public override void Update()
        {
            base.Update();

            X -= 5;
        }
    }
}
