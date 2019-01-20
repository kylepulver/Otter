using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class HudTitleInfo : Entity {

        public RichText TextTitle = new RichText("FLIPPY FLOP", 50);
        public RichText TextDetails = new RichText("PUSH X A LOT", 20);
        public RichText TextWebzone = new RichText("kpulv.com", 10);

        public HudTitleInfo() {

            TextTitle.X = Game.Instance.HalfWidth;
            TextDetails.X = Game.Instance.HalfWidth;
            TextWebzone.X = Game.Instance.HalfWidth;

            TextTitle.DefaultShadowX = 3;
            TextTitle.DefaultShadowY = 3;
            TextTitle.DefaultSineAmpY = 10;
            TextTitle.DefaultSineRateY = 10;
            TextTitle.DefaultOffsetAmount = 5;

            TextDetails.DefaultShadowX = 1;
            TextDetails.DefaultShadowY = 1;
            TextDetails.DefaultSineAmpX = 1;
            TextDetails.DefaultSineAmpY = 2;
            TextDetails.DefaultSineRateX = 5;
            TextDetails.DefaultSineRateY = 5;

            TextWebzone.DefaultCharColor = new Color("749ace");

            TextTitle.Refresh();
            TextDetails.Refresh();
            TextWebzone.Refresh();

            TextTitle.Y = 200;
            TextDetails.Y = 300;
            TextWebzone.Y = 460;

            TextTitle.CenterOrigin();
            TextDetails.CenterOrigin();
            TextWebzone.CenterOrigin();

            AddGraphics(TextTitle, TextDetails, TextWebzone);

            EventRouter.Subscribe(Events.GameStarted, (EventRouter.Event e) => {
                Tween(this, new { Y = 480 }, 30).Ease(Ease.BackIn);
            });
        }
    }
}
