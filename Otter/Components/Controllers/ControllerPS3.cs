namespace Otter {
    public class ControllerPS3 : Controller {

        public Button Triangle { get { return Button(Controls.Triangle); } }
        public Button Circle { get { return Button(Controls.Circle); } }
        public Button Cross { get { return Button(Controls.Cross); } }
        public Button Square { get { return Button(Controls.Square); } }
        public Button R1 { get { return Button(Controls.R1); } }
        public Button L1 { get { return Button(Controls.L1); } }
        public Button L3 { get { return Button(Controls.L3); } }
        public Button R3 { get { return Button(Controls.R3); } }
        public Button Start { get { return Button(Controls.Start); } }
        public Button Select { get { return Button(Controls.Select); } }
        public Button R2 { get { return Button(Controls.R2); } }
        public Button L2 { get { return Button(Controls.L2); } }

        public Axis LeftStick { get { return Axis(Controls.LStick); } }
        public Axis RightStick { get { return Axis(Controls.RStick); } }
        public Axis DPad { get { return Axis(Controls.DPad); } }

        public ControllerPS3(params int[] joystickId) {
            AddButton(Controls.Triangle);
            AddButton(Controls.Circle);
            AddButton(Controls.Cross);
            AddButton(Controls.Square);
            AddButton(Controls.R1);
            AddButton(Controls.L1);
            AddButton(Controls.L3);
            AddButton(Controls.R3);
            AddButton(Controls.Start);
            AddButton(Controls.Select);
            AddButton(Controls.R2);
            AddButton(Controls.L2);

            AddAxis(Controls.LStick);
            AddAxis(Controls.RStick);
            AddAxis(Controls.DPad);

            foreach (var joy in joystickId) {
                Triangle.AddJoyButton(0, joy);
                Circle.AddJoyButton(1, joy);
                Cross.AddJoyButton(2, joy);
                Square.AddJoyButton(3, joy);
                L2.AddJoyButton(4, joy);
                R2.AddJoyButton(5, joy);
                L1.AddJoyButton(6, joy);
                R1.AddJoyButton(7, joy);
                Select.AddJoyButton(8, joy);
                Start.AddJoyButton(9, joy);
                L3.AddJoyButton(10, joy);
                R3.AddJoyButton(11, joy);

                R2.AddAxisButton(AxisButton.ZMinus, joy);
                L2.AddAxisButton(AxisButton.ZPlus, joy);

                LeftStick.AddJoyAxis(JoyAxis.X, JoyAxis.Y, joy);
                RightStick.AddJoyAxis(JoyAxis.U, JoyAxis.R, joy);
                DPad.AddJoyAxis(JoyAxis.PovX, JoyAxis.PovY, joy);
            }
        }

        enum Controls {
            Triangle,
            Circle,
            Cross,
            Square,
            R1,
            L1,
            R2,
            L2,
            L3,
            R3,
            LStick,
            RStick,
            Start,
            Select,
            DPad,
            Triggers
        }
    }
}
