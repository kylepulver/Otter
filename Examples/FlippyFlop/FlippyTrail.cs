using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class FlippyTrail : Entity {

        public Image Image = Image.CreateRectangle(20, Color.Orange);

        public FlippyTrail(float x, float y) : base(x, y) {
            Graphic = Image;

            Image.CenterOrigin();

            Layer = 100;

            Tween(Image, new { ScaleX = 0, ScaleY = 0 }, 30).OnComplete(() => RemoveSelf());
        }

        public override void Update() {
            base.Update();

            X -= 5;
        }

        
    }
}
