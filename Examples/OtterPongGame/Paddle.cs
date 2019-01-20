using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Otter;

namespace OtterPongGame {
    class Paddle : Entity {

        Text textScore = new Text(40);

        int speed = 6;

        Session player;

        Image imgPaddle = Image.CreateRectangle(10, 100);

        public Paddle(Session player) : base() {
            this.player = player;

            SetHitbox(10, 100, (int)Tags.Paddle);
            Collider.CenterOrigin();

            SetGraphic(imgPaddle);
            imgPaddle.CenterOrigin();

            if (player.Id == 0) { // player 1
                X = 50;
            }
            else { // player 2
                X = Game.Instance.Width - 50;
            }

            textScore.X = X;
            textScore.Y = 40;

            Y = Game.Instance.HalfHeight;
        }

        public override void Update() {
            base.Update();

            if (player.Controller.Button(Controls.Up).Down) {
                Y -= speed;
            }
            if (player.Controller.Button(Controls.Down).Down) {
                Y += speed;
            }

            if (player.Id == 0) {
                textScore.String = Global.PlayerOneScore.ToString("00");
            }
            else {
                textScore.String = Global.PlayerTwoScore.ToString("00");
            }

            textScore.CenterOrigin();

            Y = Util.Clamp(Y, imgPaddle.HalfHeight, Game.Instance.Height - imgPaddle.HalfHeight);
        }

        public override void Render() {
            base.Render();

            Draw.Graphic(textScore, 0, 0);
        }
    }
}
