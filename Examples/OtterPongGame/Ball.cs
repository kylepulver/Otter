using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Otter;

namespace OtterPongGame {
    class Ball : Entity {

        Image imgBall = Image.CreateCircle(7);

        Speed speed = new Speed(10, 10);

        int startCountdown = 0;
        int startTime = 60;

        public Ball() : base() {
            SetHitbox(7, 7, (int)Tags.Ball);
            Collider.CenterOrigin();

            SetGraphic(imgBall);
            imgBall.CenterOrigin();

            X = Game.Instance.HalfWidth;
            Y = Game.Instance.HalfHeight;

            startCountdown = startTime;
        }

        public override void Update() {
            base.Update();

            if (startCountdown > 0) {
                startCountdown--;
                if (startCountdown == 0) {
                    Start();
                }
            }

            X += speed.X;
            Y += speed.Y;

            var c = Collider.Collide(X, Y, (int)Tags.Paddle);
            if (c != null) {
                var paddle = c.Entity;
                speed.X *= -1;
                speed.Y = (Y - paddle.Y) * 0.5f;

                imgBall.Scale = 3;
                Tween(imgBall, new { ScaleX = 1, ScaleY = 1 }, 60).Ease(Ease.ElasticOut);
            }

            if (Y < 0) {
                speed.Y *= -1;
            }
            if (Y > Game.Instance.Height) {
                speed.Y *= -1;
            }

            if (X > Game.Instance.Width) {
                // player 1 scores
                Global.PlayerOneScore++;
                Score();
            }
            if (X < 0) {
                // player 2 scores
                Global.PlayerTwoScore++;
                Score();
            }

            Scene.Add(new BallTrail(X, Y));
        }

        public void Score() {
            startCountdown = startTime;
            speed.X = 0;
            speed.Y = 0;
            X = Game.Instance.HalfWidth;
            Y = Game.Instance.HalfHeight;
        }

        public void Start() {
            speed.X = Rand.Choose(-5, 5);
        }


    }
}
