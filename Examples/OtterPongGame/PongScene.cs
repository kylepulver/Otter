using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Otter;

namespace OtterPongGame {
    class PongScene : Scene {

        public PongScene() : base() {
            Add(new Paddle(Global.PlayerOne));
            Add(new Paddle(Global.PlayerTwo));
            Add(new Ball());
        }
    }
}
