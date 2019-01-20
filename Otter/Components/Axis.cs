using SFML.Window;
using System;
using System.Collections.Generic;

namespace Otter {
    /// <summary>
    /// Component that represents an axis of input.  Interprets both X and Y from -1 to 1.  Can use multiple
    /// sources of input like keyboard, mouse buttons, or joystick axes and buttons.  Input can also be delivered from code.
    /// </summary>
    public class Axis : Component {

        #region Static Methods

        /// <summary>
        /// Create a new Axis that uses the arrow keys for movement.
        /// </summary>
        /// <returns>A new Axis.</returns>
        public static Axis CreateArrowKeys() {
            return new Axis(Key.Up, Key.Right, Key.Down, Key.Left);
        }

        /// <summary>
        /// Create a new Axis that uses WASD for movement.
        /// </summary>
        /// <returns>A new Axis.</returns>
        public static Axis CreateWASD() {
            return new Axis(Key.W, Key.D, Key.S, Key.A);
        }

        #endregion

        #region Private Fields

        

        #endregion

        #region Public Fields

        /// <summary>
        /// The Keys to uss.
        /// </summary>
        public Dictionary<Direction, List<Key>> Keys = new Dictionary<Direction, List<Key>>();

        /// <summary>
        /// The joystick buttons to use.
        /// </summary>
        public Dictionary<Direction, List<List<int>>> JoyButtons = new Dictionary<Direction, List<List<int>>>();

        /// <summary>
        /// The X axes to use.
        /// </summary>
        public List<List<JoyAxis>> AxesX = new List<List<JoyAxis>>();

        /// <summary>
        /// The Y axes to use.
        /// </summary>
        public List<List<JoyAxis>> AxesY = new List<List<JoyAxis>>();

        /// <summary>
        /// Determines if the axis is currently enabled.  If false, X and Y will report 0.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// The range that must be exceeded by joysticks in order for their input to register.
        /// </summary>
        public float DeadZone = 0.15f;

        /// <summary>
        /// Determines if the DeadZone will be treated as 0 for joysticks.
        /// If true, remaps the range DeadZone to 100 to 0 to 1.
        /// If false, remaps the range 0 to 100 to 0 to 1.
        /// </summary>
        public bool RemapRange = true;

        /// <summary>
        /// Determines if raw data coming from the joysticks should be rounded to 2 digits.
        /// </summary>
        public bool RoundInput = true;

        /// <summary>
        /// Determines if input has any effect on the axis.  When set to true the axis will remain at
        /// the X and Y it was at when locked.
        /// </summary>
        public bool Locked = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// The current Vector2 position of the axis.
        /// </summary>
        public Vector2 Position {
            get {
                return new Vector2(X, Y);
            }
        }

        /// <summary>
        /// The X position of the axis from -1 to 1.
        /// </summary>
        public float X { get; private set; }

        /// <summary>
        /// The Y position of the axis from -1 to 1.
        /// </summary>
        public float Y { get; private set; }

        /// <summary>
        /// The previous X position of the axis.
        /// </summary>
        public float LastX { get; private set; }

        /// <summary>
        /// The previous Y position of the axis.
        /// </summary>
        public float LastY { get; private set; }

        /// <summary>
        /// Check if the axis is currently forced.
        /// </summary>
        public bool ForcedInput { get; private set; }

        /// <summary>
        /// The the up Button for the Axis.
        /// </summary>
        public Button Up { get; private set; }
       
        /// <summary>
        /// The the left Button for the Axis.
        /// </summary>
        public Button Left { get; private set; }

        /// <summary>
        /// Gets the down Button for the Axis.
        /// </summary>
        public Button Down { get; private set; }

        /// <summary>
        /// Gets the right Button for the Axis.
        /// </summary>
        public Button Right { get; private set; }

        /// <summary>
        /// Check if the axis has any means of input currently registered to it.
        /// </summary>
        public bool HasInput {
            get {
                if (ForcedInput) return true;
                if (Keys.Count > 0) return true;
                if (JoyButtons.Count > 0) return true;
                if (AxesX.Count > 0) return true;
                if (AxesY.Count > 0) return true;
                return false;
            }
        }

        /// <summary>
        /// Check of the axis is completely neutral.
        /// </summary>
        public bool Neutral {
            get {
                return X == 0 && Y == 0;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Axis.
        /// </summary>
        public Axis() {
            ForcedInput = false;

            foreach (Direction d in Enum.GetValues(typeof(Direction))) {
                Keys[d] = new List<Key>();
                JoyButtons.Add(d, new List<List<int>>());
                for (int i = 0; i < Joystick.Count; i++) {
                    JoyButtons[d].Add(new List<int>());
                }
            }

            for (int i = 0; i < Joystick.Count; i++) {
                AxesX.Add(new List<JoyAxis>());
                AxesY.Add(new List<JoyAxis>());
            }

            // Create buttons for Axis.
            Up = new Button();
            Down = new Button();
            Left = new Button();
            Right = new Button();
        }

        /// <summary>
        /// Create a new Axis using Keys.
        /// </summary>
        /// <param name="up">The Key for Up.</param>
        /// <param name="right">The Key for Right.</param>
        /// <param name="down">The Key for Down.</param>
        /// <param name="left">The Key for Left.</param>
        public Axis(Key up, Key right, Key down, Key left)
            : this() {
            AddKeys(up, right, down, left);
        }

        /// <summary>
        /// Create a new Axis using a joystick axis.
        /// </summary>
        /// <param name="x">The JoyAxis to use for X.</param>
        /// <param name="y">The JoyAxis to use for Y.</param>
        /// <param name="joystick">The joystick id to use.</param>
        public Axis(JoyAxis x, JoyAxis y, params int[] joystick)
            : this() {
                foreach (var j in joystick) {
                    AddJoyAxis(x, y, j);
                }
        }

        /// <summary>
        /// Create a new Axis using AxisButtons.
        /// </summary>
        /// <param name="up">The AxisButton for Up.</param>
        /// <param name="right">The AxisButton for Right.</param>
        /// <param name="down">The AxisButton for Down.</param>
        /// <param name="left">The AxisButton for Left.</param>
        /// <param name="joystick">The joystick id to use.</param>
        public Axis(AxisButton up, AxisButton right, AxisButton down, AxisButton left, params int[] joystick)
            : this() {
            AddButton(up, Direction.Up, joystick);
            AddButton(right, Direction.Right, joystick);
            AddButton(down, Direction.Down, joystick);
            AddButton(left, Direction.Left, joystick);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the Axis to report no input.
        /// </summary>
        public void Reset() {
            X = 0;
            Y = 0;
            LastX = 0;
            LastY = 0;
            Left.Reset();
            Right.Reset();
            Up.Reset();
            Down.Reset();
        }

        /// <summary>
        /// Clear all registered inputs for the Axis.
        /// </summary>
        public void Clear() {
            Keys.Clear();
            JoyButtons.Clear();
            AxesX.Clear();
            AxesY.Clear();
        }

        /// <summary>
        /// Add a joystick axis.
        /// </summary>
        /// <param name="x">The x axis of the joystick.</param>
        /// <param name="y">The y axis of the joystick.</param>
        /// <param name="joystick">The joystick id.</param>
        /// <returns>The Axis.</returns>
        public Axis AddJoyAxis(JoyAxis x, JoyAxis y, params int[] joystick) {
            foreach (var j in joystick) {
                AxesX[j].Add(x);
                AxesY[j].Add(y);
            }
            return this;
        }

        /// <summary>
        /// Add another Axis to this Axis.
        /// </summary>
        /// <param name="source">The source Axis to use.</param>
        /// <returns>This Axis.</returns>
        public Axis AddAxis(Axis source) {
            foreach (Direction d in Enum.GetValues(typeof(Direction))) {
                // Copy keys from source Axis.
                Keys[d].AddRange(source.Keys[d]);
                for (int i = 0; i < Joystick.Count; i++) {
                    // Copy buttons from source Axis.
                    JoyButtons[d].AddRange(source.JoyButtons[d]);
                }
            }

            for (int i = 0; i < Joystick.Count; i++) {
                // Copy joy axes from source Axis.
                AxesX[i].AddRange(source.AxesX[i]);
                AxesY[i].AddRange(source.AxesY[i]);
            }

            return this;
        }

        /// <summary>
        /// Add a joystick button.
        /// </summary>
        /// <param name="button">The joystick button id.</param>
        /// <param name="direction">The direction this button should effect.</param>
        /// <param name="joystick">The joystick id.</param>
        /// <returns>The Axis.</returns>
        public Axis AddButton(int button, Direction direction, params int[] joystick) {
            foreach (var j in joystick) {
                JoyButtons[direction][j].Add(button);
            }
            return this;
        }

        /// <summary>
        /// Add a joystick axis button.
        /// </summary>
        /// <param name="button">The joystick axis button.</param>
        /// <param name="direction">The direction this axis button should effect.</param>
        /// <param name="joystick">The joystick id.</param>
        /// <returns>The Axis.</returns>
        public Axis AddButton(AxisButton button, Direction direction, params int[] joystick) {
            foreach (var j in joystick) {
                AddButton((int)button, direction, j);
            }
            return this;
        }

        /// <summary>
        /// Add a key.
        /// </summary>
        /// <param name="key">The keyboard key.</param>
        /// <param name="direction">The direction this key should effect.</param>
        /// <returns>The Axis.</returns>
        public Axis AddKey(Key key, Direction direction) {
            Keys[direction].Add(key);

            return this;
        }

        /// <summary>
        /// Add keys.
        /// </summary>
        /// <param name="upRightDownLeft">Four keys to create a pair of axes from (Up, Right, Down, Left).</param>
        /// <returns>The Axis.</returns>
        public Axis AddKeys(params Key[] upRightDownLeft) {
            if (upRightDownLeft.Length != 4) {
                throw new ArgumentException("Must use four keys for an axis!");
            }

            AddKey(upRightDownLeft[0], Direction.Up);
            AddKey(upRightDownLeft[1], Direction.Right);
            AddKey(upRightDownLeft[2], Direction.Down);
            AddKey(upRightDownLeft[3], Direction.Left);

            return this;
        }

        /// <summary>
        /// Force the axis state.
        /// </summary>
        /// <param name="x">The forced x state.</param>
        /// <param name="y">The forced y state.</param>
        public void ForceState(float x, float y) {
            ForcedInput = true;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Force the axis state.
        /// </summary>
        /// <param name="xy">The forced x and y state.</param>
        public void ForceState(Vector2 xy) {
            ForceState(xy.X, xy.Y);
        }

        /// <summary>
        /// Force the axis x state.
        /// </summary>
        /// <param name="x">The forced x state.</param>
        public void ForceStateX(float x) {
            ForceState(x, Y);
        }

        /// <summary>
        /// Force the axis y state.
        /// </summary>
        /// <param name="y">The forced y state.</param>
        public void ForceStateY(float y) {
            ForceState(X, y);
        }

        /// <summary>
        /// Relinquish control of the axis back to input.
        /// </summary>
        public void ReleaseState() {
            ForcedInput = false;
        }

        /// <summary>
        /// Update the Axis.
        /// </summary>
        public override void UpdateFirst() {
            base.UpdateFirst();

            LastX = X;
            LastY = Y;

            if (Locked) return;

            if (!Enabled) {
                X = 0;
                Y = 0;
                return;
            }

            if (ForcedInput) {
                return;
            }

            X = 0;
            Y = 0;

            for (int i = 0; i < Joystick.Count; i++) {
                foreach (JoyAxis axis in AxesX[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                }
                foreach (JoyAxis axis in AxesY[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                }

                foreach (JoyAxis axis in AxesX[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                    if (Math.Abs(a) >= DeadZone) {
                        if (RemapRange) {
                            if (a > 0) {
                                X += Util.ScaleClamp(a, 0, 1, 0, 1);
                            }
                            else {
                                X += Util.ScaleClamp(a, -1, -0, -1, 0);
                            }
                        }
                        else {
                            X += a;
                        }
                    }
                    if (RoundInput) X = (float)Math.Round(X, 2);
                }

                foreach (JoyAxis axis in AxesY[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                    if (Math.Abs(a) >= DeadZone) {
                        if (RemapRange) {
                            if (a > 0) {
                                Y += Util.ScaleClamp(a, 0, 1, 0, 1);
                            }
                            else {
                                Y += Util.ScaleClamp(a, -1, -0, -1, 0);
                            }
                        }
                        else {
                            Y += a;
                        }
                    }
                    if (RoundInput) Y = (float)Math.Round(Y, 2);
                }

            }

            foreach (Key k in Keys[Direction.Up]) {
                if (Input.Instance.KeyDown(k)) {
                    Y -= 1;
                }
            }
            foreach (Key k in Keys[Direction.Down]) {
                if (Input.Instance.KeyDown(k)) {
                    Y += 1;
                }
            }
            foreach (Key k in Keys[Direction.Left]) {
                if (Input.Instance.KeyDown(k)) {
                    X -= 1;
                }
            }
            foreach (Key k in Keys[Direction.Right]) {
                if (Input.Instance.KeyDown(k)) {
                    X += 1;
                }
            }

            for (int i = 0; i < Joystick.Count; i++) {
                foreach (int b in JoyButtons[Direction.Up][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        Y -= 1;
                    }
                }
                foreach (int b in JoyButtons[Direction.Down][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        Y += 1;
                    }
                }
                foreach (int b in JoyButtons[Direction.Left][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        X -= 1;
                    }
                }
                foreach (int b in JoyButtons[Direction.Right][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        X += 1;
                    }
                }
            }

            X = Util.Clamp(X, -1, 1);
            Y = Util.Clamp(Y, -1, 1);

            // Update the buttons.  This makes it easy to read the Axis as buttons for Up, Right, Left, Down.
            Right.UpdateFirst();
            Up.UpdateFirst();
            Left.UpdateFirst();
            Down.UpdateFirst();

            Right.ForceState(false);
            Up.ForceState(false);
            Left.ForceState(false);
            Down.ForceState(false);

            if (X > 0.5f) {
                Right.ForceState(true);
            }
            if (X < -0.5f) {
                Left.ForceState(true);
            }
            if (Y > 0.5f) {
                Down.ForceState(true);
            }
            if (Y < -0.5f) {
                Up.ForceState(true);
            }
        }

        public override string ToString() {
            return "[Axis X: " + X.ToString() + " Y: " + Y.ToString() + "]";
        }

        #endregion
        
    }
}
