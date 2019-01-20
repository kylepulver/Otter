using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class Wall : Entity {

        public Image Image = Image.CreateRectangle(40, 480);

        public int Speed = 6;

        public Wall(float x, float y) : base(x, y) {
            Graphic = Image;
            SetHitbox(40, 480, (int)Tags.Wall);
            X = 700;
            Layer = 10;

            Image.OutlineColor = new Color("364298");
            Image.OutlineThickness = 4;

            if (Y < 0) {
                Tween(this, new { Y = Y }, 40).From(new { Y = Y - 480 }).Ease(Ease.ElasticOut);
            }
            else {
                Tween(this, new { Y = Y }, 40).From(new { Y = Y + 480 }).Ease(Ease.ElasticOut);
            }

            EventRouter.Subscribe(Events.FlippyDied, HandleDeath);

        }

        void HandleDeath(EventRouter.Event e) {
            Tween(this, new { Speed = -12 }, 30);
        }

        public override void Update() {
            base.Update();

            X -= Speed;

            if (X < -40) {
                RemoveSelf();
            }
        }

        public override void Removed() {
            base.Removed();

            EventRouter.Unsubscribe(Events.FlippyDied, HandleDeath);
        }
    }
}
