using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class HudScore : Entity {

        public RichText TextScore = new RichText(20);

        public HudScore() {
            Graphic = TextScore;

            X = Game.Instance.Width - 100;
            Y = 10;

            TextScore.DefaultShadowX = 1;
            TextScore.DefaultShadowY = 1;
            TextScore.TextAlign = TextAlign.Right;

            TextScore.TextWidth = 200;

            EventRouter.Subscribe(Events.ScoreUpdated, (EventRouter.Event e) => {
                var score = e.GetData<float>(0);
                var scoreMultiplier = e.GetData<float>(1);

                TextScore.String = string.Format("{0:0,0} x {1:0,0}", score, scoreMultiplier);

                TextScore.X = -120;
            });

            EventRouter.Subscribe(Events.ShowFinalScore, (EventRouter.Event e) => {
                var score = e.GetData<float>(0);
                TextScore.DefaultCharColor = Color.Gold;
                TextScore.String = string.Format("{0:0,0}", score);
            });

            EventRouter.Subscribe(Events.GameStarted, (EventRouter.Event e) => {
                TextScore.DefaultCharColor = Color.White;
            });

            Layer = -1000;
        }
    }
}
