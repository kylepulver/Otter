using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Otter;

namespace OtterPongGame {
    class BallTrail : Entity {

        Image imgBall = Image.CreateCircle(7, Color.Cyan);

        public BallTrail(float x, float y) : base(x, y) {
            SetGraphic(imgBall);
            imgBall.CenterOrigin();

            LifeSpan = 60;

            Layer = 100;
        }

        public override void Update() {
            base.Update();

            imgBall.Alpha = Util.ScaleClamp(Timer, 0, LifeSpan, 1, 0);
        }
    }
}
