using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Otter;

namespace OtterPongGame {
    class Global {

        public static Session
            PlayerOne,
            PlayerTwo;

        public static int
            PlayerOneScore = 0,
            PlayerTwoScore = 0;
    }

    public enum Tags {
        Paddle,
        Ball
    }

    public enum Controls {
        Up,
        Down
    }
}
