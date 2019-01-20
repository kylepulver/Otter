using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class HudHighScore : Entity {

        public RichText TextScore = new RichText(20);

        public HudHighScore() {

            AddGraphic(TextScore);

            TextScore.DefaultShadowX = 1;
            TextScore.DefaultShadowY = 1;

            TextScore.X = 10;
            TextScore.Y = 10;

            EventRouter.Subscribe(Events.UpdateBestScore, (EventRouter.Event e) => {
                var score = e.GetData<float>(0);
                var last = e.GetData<float>(1);

                if (last > 0) {
                    TextScore.String = string.Format("BEST {0:0,0} LAST {1:0,0}", score, last);
                }
                else {
                    if (score > 0) {
                        TextScore.String = string.Format("BEST {0:0,0}", score);
                    }
                }
            });
        }
    }
}
