using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Otter {
    /// <summary>
    /// Class used for managing input from the keyboard, mouse, and joysticks. Updated by the active Game.
    /// </summary>
    public class Input {

        #region Static Fields

        /// <summary>
        /// A reference to the current active instance.
        /// </summary>
        public static Input Instance;

        #endregion

        #region Static Properties

        /// <summary>
        /// The current number of joysticks connected.
        /// </summary>
        public static int JoysticksConnected {
            get {
                int count = 0;
                for (uint i = 0; i < Joystick.Count; i++) {
                    if (Joystick.IsConnected(i)) {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// The maximum possible amount of joysticks that can be connected.
        /// </summary>
        public static int JoysticksSupported {
            get {
                return (int)Joystick.Count;
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Convert a char to a Key code.
        /// </summary>
        /// <param name="key">The char to convert into a Key.</param>
        /// <returns>The Key.  Returns Key.Unknown if no match is found.</returns>
        public static Key CharToKey(char key) {
            key = char.ToUpper(key);
            switch (key) {
                case 'A': return Key.A;
                case 'B': return Key.B;
                case 'C': return Key.C;
                case 'D': return Key.D;
                case 'E': return Key.E;
                case 'F': return Key.F;
                case 'G': return Key.G;
                case 'H': return Key.H;
                case 'I': return Key.I;
                case 'J': return Key.J;
                case 'K': return Key.K;
                case 'L': return Key.L;
                case 'M': return Key.M;
                case 'N': return Key.N;
                case 'O': return Key.O;
                case 'P': return Key.P;
                case 'Q': return Key.Q;
                case 'R': return Key.R;
                case 'S': return Key.S;
                case 'T': return Key.T;
                case 'U': return Key.U;
                case 'V': return Key.V;
                case 'W': return Key.W;
                case 'X': return Key.X;
                case 'Y': return Key.Y;
                case 'Z': return Key.Z;
                case '0': return Key.Num0;
                case '1': return Key.Num1;
                case '2': return Key.Num2;
                case '3': return Key.Num3;
                case '4': return Key.Num4;
                case '5': return Key.Num5;
                case '6': return Key.Num6;
                case '7': return Key.Num7;
                case '8': return Key.Num8;
                case '9': return Key.Num9;
                case '[': return Key.LBracket;
                case ']': return Key.RBracket;
                case ';': return Key.SemiColon;
                case ',': return Key.Comma;
                case '.': return Key.Period;
                case '/': return Key.Slash;
                case '\\': return Key.BackSlash;
                case '~': return Key.Tilde;
                case '=': return Key.Equal;
                case '+': return Key.Add;
                case '-': return Key.Dash;
                case ' ': return Key.Space;
            }
            return Key.Unknown;
        }

        /// <summary>
        /// Get the name of the Joystick.
        /// </summary>
        /// <param name="id">The connection id of the Joystick.</param>
        /// <returns>The name of the Joystick.</returns>
        public static string GetJoystickName(int id) {
            return Joystick.GetIdentification((uint)id).Name;
        }

        /// <summary>
        /// Get the vendor id of the Joystick.
        /// </summary>
        /// <param name="id">The connection id of the Joystick.</param>
        /// <returns>The vendor id of the Joystick.</returns>
        public static int GetJoystickVendorId(int id) {
            return (int)Joystick.GetIdentification((uint)id).VendorId;
        }

        /// <summary>
        /// Get the product id of the Joystick.
        /// </summary>
        /// <param name="id">The connection id of the Joystick.</param>
        /// <returns>The name of the Joystick.</returns>
        public static int GetJoystickProductId(int id) {
            return (int)Joystick.GetIdentification((uint)id).ProductId;
        }

        #endregion

        #region Private Fields

        float mouseWheelDelta, currentMouseWheelDelta;

        int
            keysPressed,
            currentKeysPressed,
            prevKeysPressed,
            mouseButtonsPressed,
            currentMouseButtonsPressed,
            prevMouseButtonsPressed;

        List<int>
            buttonsPressed = new List<int>(),
            prevButtonsPressed = new List<int>();

        internal bool bufferReleases = true;

        Dictionary<Key, bool> activeKeys = new Dictionary<Key, bool>();
        Dictionary<Key, bool> currentKeys = new Dictionary<Key, bool>();
        Dictionary<Key, bool> previousKeys = new Dictionary<Key, bool>();

        Dictionary<MouseButton, bool> activeMouseButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> currentMouseButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> previousMouseButtons = new Dictionary<MouseButton, bool>();

        List<Dictionary<uint, bool>> activeButtons = new List<Dictionary<uint, bool>>();
        List<Dictionary<uint, bool>> currentButtons = new List<Dictionary<uint, bool>>();
        List<Dictionary<uint, bool>> previousButtons = new List<Dictionary<uint, bool>>();

        List<Key> keyReleaseBuffer = new List<Key>();
        List<MouseButton> mouseReleaseBuffer = new List<MouseButton>();
        List<List<uint>> buttonReleaseBuffer = new List<List<uint>>();

        Dictionary<JoyAxis, float> axisThreshold = new Dictionary<JoyAxis, float>();

        Dictionary<JoyAxis, AxisSet> axisSet = new Dictionary<JoyAxis, AxisSet>();

        struct AxisSet {
            public AxisButton Plus;
            public AxisButton Minus;
        };

        int gameMouseX;
        int gameMouseY;

        int mouseDeltaBufferX;
        int mouseDeltaBufferY;

        #endregion

        #region Public Fields

        /// <summary>
        /// The maximum size of the string of recent key presses.
        /// </summary>
        public static int KeystringSize = 500;

        /// <summary>
        /// Determines if the mouse should be locked to the center of the screen.
        /// </summary>
        public static bool CenteredMouse = false;

        /// <summary>
        /// The current string of keys that were pressed.
        /// </summary>
        public string KeyString = "";

        #endregion

        #region Public Properties

        /// <summary>
        /// The reference to the game that owns this class.
        /// </summary>
        public Game Game { get; internal set; }

        /// <summary>
        /// The last known key that was pressed.
        /// </summary>
        public Key LastKey { get; private set; }

        /// <summary>
        /// The last known mouse button that was pressed.
        /// </summary>
        public MouseButton LastMouseButton { get; private set; }

        /// <summary>
        /// The last known button pressed on each joystick.
        /// </summary>
        public List<int> LastButton { get; private set; }

        /// <summary>
        /// The X movement of the mouse since the last update.  Only updates if the mouse is locked inside the Game window.
        /// </summary>
        public int MouseDeltaX { get; private set; }

        /// <summary>
        /// The Y movement of the mouse since the last update.  Only updates if the mouse is locked inside the Game window.
        /// </summary>
        public int MouseDeltaY { get; private set; }

        /// <summary>
        /// The current X position of the mouse.
        /// </summary>
        public int MouseX {
            get {
                float pos = 0;

                if (Game.LockMouseCenter) {
                    pos = gameMouseX;
                }
                else {
                    pos = SFML.Window.Mouse.GetPosition(Game.Window).X;
                    pos -= Game.Surface.X - Game.Surface.ScaledWidth * 0.5f;
                    pos /= Game.Surface.ScaleX;
                }

                return (int)pos;
            }
        }

        /// <summary>
        /// The current Y position of the mouse.
        /// </summary>
        public int MouseY {
            get {
                float pos = 0;

                if (Game.LockMouseCenter) {
                    pos = gameMouseY;
                }
                else {
                    pos = SFML.Window.Mouse.GetPosition(Game.Window).Y;
                    pos -= Game.Surface.Y - Game.Surface.ScaledHeight * 0.5f;
                    pos /= Game.Surface.ScaleY;
                }

                return (int)pos;
            }
        }

        /// <summary>
        /// The raw X position of the mouse.  This can be set.
        /// </summary>
        public int MouseRawX {
            get {
                if (Game.LockMouseCenter) {
                    return gameMouseX;
                }

                return SFML.Window.Mouse.GetPosition(Game.Window).X;
            }
            set {
                if (Game.LockMouseCenter) {
                    gameMouseX = value;
                }
                else {
                    SFML.Window.Mouse.SetPosition(new Vector2i(value, MouseRawY), Game.Window);
                }
            }
        }

        /// <summary>
        /// The raw Y position of the mouse.  This can be set.
        /// </summary>
        public int MouseRawY {
            get {
                if (Game.LockMouseCenter) {
                    return gameMouseY;
                }

                return SFML.Window.Mouse.GetPosition(Game.Window).Y;
            }
            set {
                if (Game.LockMouseCenter) {
                    gameMouseY = value;
                }
                else {
                    SFML.Window.Mouse.SetPosition(new Vector2i(MouseRawX, value), Game.Window);
                }
            }
        }

        /// <summary>
        /// The X position of the mouse in screen space.
        /// </summary>
        public float MouseScreenX {
            get { return MouseX + Game.Scene.CameraX; }
        }

        /// <summary>
        /// The Y position of the mouse in screen space.
        /// </summary>
        public float MouseScreenY {
            get { return MouseY + Game.Scene.CameraY; }
        }

        /// <summary>
        /// The change in the mouse wheel value this update.
        /// </summary>
        public float MouseWheelDelta {
            get { return mouseWheelDelta; }
        }

        /// <summary>
        /// The X position of the mouse in the game.  Use if the mouse is locked in the game window.
        /// This can be set if the mouse is locked inside the game window.
        /// </summary>
        public int GameMouseX {
            get {
                return gameMouseX;
            }
            set {
                gameMouseX = value;
            }
        }

        /// <summary>
        /// The Y position of the mouse in the game.  Use if the mouse is locked in the game window.
        /// This can be set if the mouse is locked inside the game window.
        /// </summary>
        public int GameMouseY {
            get {
                return gameMouseY;
            }
            set {
                gameMouseY = value;
            }
        }

        #endregion

        #region Constructors

        internal Input(Game game) {
            Game = game;
            Instance = this;
            Init();
        }

        #endregion

        #region Private Methods

        void OnJoystickConnected(object sender, JoystickConnectEventArgs e) {
        }

        void OnTextEntered(object sender, TextEventArgs e) {
            //convert unicode to ascii to check range later
            string hexValue = (Encoding.ASCII.GetBytes(e.Unicode)[0].ToString("X"));
            int ascii = (int.Parse(hexValue, NumberStyles.HexNumber));

            if (e.Unicode == "\b") {
                if (KeyString.Length > 0) {
                    KeyString = KeyString.Remove(KeyString.Length - 1, 1);
                }
            }
            else if (e.Unicode == "\r") {
                KeyString += "\n";
            }
            else if (ascii >= 32 && ascii < 128) { //only add to keystring if actual character
                KeyString += e.Unicode;
            }
        }

        void OnKeyPressed(object sender, KeyEventArgs e) {
            if (Game.Debugger != null) {
                // Ignore presses from the debug toggle key.
                if ((Key)e.Code == Game.Debugger.ToggleKey) return;
            }

            if (!activeKeys[(Key)e.Code]) {
                keysPressed++;
            }
            activeKeys[(Key)e.Code] = true;
            LastKey = (Key)e.Code;
        }

        void OnKeyReleased(object sender, KeyEventArgs e) {
            if (bufferReleases) {
                keyReleaseBuffer.Add((Key)e.Code);
            }
            else {
                activeKeys[(Key)e.Code] = false;
            }
            keysPressed--;
        }

        void OnMousePressed(object sender, MouseButtonEventArgs e) {
            activeMouseButtons[(MouseButton)e.Button] = true;
            mouseButtonsPressed++;
            LastMouseButton = (MouseButton)e.Button;
        }

        void OnMouseReleased(object sender, MouseButtonEventArgs e) {
            if (bufferReleases) {
                mouseReleaseBuffer.Add((MouseButton)e.Button);
            }
            else {
                activeMouseButtons[(MouseButton)e.Button] = false;
            }
            mouseButtonsPressed--;
        }

        void OnButtonPressed(object sender, JoystickButtonEventArgs e) {
            activeButtons[(int)e.JoystickId][e.Button] = true;
            buttonsPressed[(int)e.JoystickId]++;
            LastButton[(int)e.JoystickId] = (int)e.Button;
            //Console.WriteLine("{0} pressed on joy {1}", e.Button, e.JoystickId);
        }

        void OnButtonReleased(object sender, JoystickButtonEventArgs e) {
            if (bufferReleases) {
                buttonReleaseBuffer[(int)e.JoystickId].Add(e.Button);
            }
            else {
                activeButtons[(int)e.JoystickId][e.Button] = false;
            }
            buttonsPressed[(int)e.JoystickId]--;
        }

        void OnJoystickMoved(object sender, JoystickMoveEventArgs e) {
            //Console.WriteLine("Joystick " + e.JoystickId + " moved axis " + e.Axis + " to " + e.Position);
        }

        void OnMouseWheelMoved(object sender, MouseWheelEventArgs e) {
            currentMouseWheelDelta = e.Delta;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check if a key has been pressed this update.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key has been pressed.</returns>
        public bool KeyPressed(Key k) {
            if (k == Key.Any) {
                return keysPressed > prevKeysPressed;
            }
            return currentKeys[k] && !previousKeys[k];
        }

        /// <summary>
        /// Check if a key has been pressed this update.
        /// </summary>
        /// <param name="c">The key to check.</param>
        /// <returns>True if the key has been pressed.</returns>
        public bool KeyPressed(char c) {
            return KeyPressed(CharToKey(c));
        }

        /// <summary>
        /// Check if a key has been released this update.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key has been released.</returns>
        public bool KeyReleased(Key k) {
            if (k == Key.Any) {
                return keysPressed < prevKeysPressed;
            }
            return !currentKeys[k] && previousKeys[k];
        }

        /// <summary>
        /// Check if a key has been released this update.
        /// </summary>
        /// <param name="c">The key to check.</param>
        /// <returns>True if the key has been released.</returns>
        public bool KeyReleased(char c) {
            return KeyReleased(CharToKey(c));
        }

        /// <summary>
        /// Check if a key is currently down.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key is down.</returns>
        public bool KeyDown(Key k) {
            if (k == Key.Any) {
                return keysPressed > 0;
            }
            return currentKeys[k];
        }

        /// <summary>
        /// Check if a key is currently down.
        /// </summary>
        /// <param name="c">The key to check.</param>
        /// <returns>True if the key is down.</returns>
        public bool KeyDown(char c) {
            return KeyDown(CharToKey(c));
        }

        /// <summary>
        /// Check if a key is currently up.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key is up.</returns>
        public bool KeyUp(Key k) {
            return !KeyDown(k);
        }

        /// <summary>
        /// Check if a key is currently up.
        /// </summary>
        /// <param name="c">The key to check.</param>
        /// <returns>True if the key is up.</returns>
        public bool KeyUp(char c) {
            return KeyUp(CharToKey(c));
        }

        /// <summary>
        /// Check if a joystick button is pressed.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the button is pressed.</returns>
        public bool ButtonPressed(int button, int joystick = 0) {
            return currentButtons[joystick][(uint)button] && !previousButtons[joystick][(uint)button];
        }

        /// <summary>
        /// Check if a joystick AxisButton is pressed.
        /// </summary>
        /// <param name="button">The AxisButton to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the button is pressed.</returns>
        public bool ButtonPressed(AxisButton button, int joystick = 0) {
            return ButtonPressed((int)button, joystick);
        }

        /// <summary>
        /// Check if the joystick button is released.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the button is released.</returns>
        public bool ButtonReleased(int button, int joystick = 0) {
            return !currentButtons[joystick][(uint)button] && previousButtons[joystick][(uint)button];
        }

        /// <summary>
        /// Check if the joystick AxisButton is released.
        /// </summary>
        /// <param name="button">The AxisButton to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the AxisButton is released.</returns>
        public bool ButtonReleased(AxisButton button, int joystick = 0) {
            return ButtonReleased((int)button, joystick);
        }

        /// <summary>
        /// Check if the joystick button is down.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the button is down.</returns>
        public bool ButtonDown(int button, int joystick = 0) {
            return currentButtons[joystick][(uint)button];
        }

        /// <summary>
        /// Check if the joystick AxisButton is down.
        /// </summary>
        /// <param name="button">The AxisButton to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the AxisButton is down.</returns>
        public bool ButtonDown(AxisButton button, int joystick = 0) {
            return ButtonDown((int)button, joystick);
        }

        /// <summary>
        /// Check if the joystick button is up.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the button is up.</returns>
        public bool ButtonUp(int button, int joystick = 0) {
            return !currentButtons[joystick][(uint)button];
        }

        /// <summary>
        /// Check if the joystick button is up.
        /// </summary>
        /// <param name="button">The AxisButton to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>True if the AxisButton is up.</returns>
        public bool ButtonUp(AxisButton button, int joystick = 0) {
            return ButtonUp((int)button, joystick);
        }

        /// <summary>
        /// Get the value of a joystick axis from -100 to 100.
        /// </summary>
        /// <param name="axis">The axis to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>The axis value from -100 to 100.</returns>
        public float GetAxis(JoyAxis axis, int joystick = 0) {
            if (Joystick.HasAxis((uint)joystick, (Joystick.Axis)axis)) {
                if (axis == JoyAxis.PovY) { //special case for dpad y
                    return Joystick.GetAxisPosition((uint)joystick, (Joystick.Axis)axis) * -1;
                }
                return Joystick.GetAxisPosition((uint)joystick, (Joystick.Axis)axis);
            }
            return 0;
        }

        /// <summary>
        /// Set the threshold for an axis to act as an AxisButton.  Defaults to 50 or one half of the joystick's total range.
        /// </summary>
        /// <param name="axis">The JoyAxis to set.</param>
        /// <param name="threshold">The threshold that the axis must pass to act as a button press.</param>
        public void SetAxisThreshold(JoyAxis axis, float threshold) {
            axisThreshold[axis] = threshold;
        }

        /// <summary>
        /// Gets the axis threshold for an axis to act as an AxisButton.
        /// </summary>
        /// <param name="axis">The JoyAxis.</param>
        public float GetAxisThreshold(JoyAxis axis) {
            return axisThreshold[axis];
        }

        /// <summary>
        /// Check if a MouseButton is pressed.
        /// </summary>
        /// <param name="b">The MouseButton to check.</param>
        /// <returns>True if the MouseButton is pressed.</returns>
        public bool MouseButtonPressed(MouseButton b) {
            if (b == MouseButton.Any) {
                return mouseButtonsPressed > prevMouseButtonsPressed;
            }
            return currentMouseButtons[b] && !previousMouseButtons[b];
        }

        /// <summary>
        /// Check if a MouseButton is pressed.
        /// </summary>
        /// <param name="b">The MouseButton to check.</param>
        /// <returns>True if the MouseButton is pressed.</returns>
        public bool MouseButtonReleased(MouseButton b) {
            if (b == MouseButton.Any) {
                return mouseButtonsPressed < prevMouseButtonsPressed;
            }
            return !currentMouseButtons[b] && previousMouseButtons[b];
        }

        /// <summary>
        /// Check if a MouseButton is pressed.
        /// </summary>
        /// <param name="b">The MouseButton to check.</param>
        /// <returns>True if the MouseButton is pressed.</returns>
        public bool MouseButtonDown(MouseButton b) {
            if (b == MouseButton.Any) {
                return mouseButtonsPressed > 0;
            }
            return currentMouseButtons[b];
        }

        /// <summary>
        /// Check if a MouseButton is pressed.
        /// </summary>
        /// <param name="b">The MouseButton to check.</param>
        /// <returns>True if the MouseButton is pressed.</returns>
        public bool MouseButtonUp(MouseButton b) {
            return !MouseButtonDown(b);
        }

        /// <summary>
        /// Clear the string of recently pressed keys.
        /// </summary>
        public void ClearKeystring() {
            KeyString = "";
        }

        #endregion

        #region Internal

        internal void WindowInit() {
            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.TextEntered += OnTextEntered;
            Game.Window.MouseButtonPressed += OnMousePressed;
            Game.Window.KeyReleased += OnKeyReleased;
            Game.Window.MouseButtonReleased += OnMouseReleased;
            Game.Window.JoystickButtonPressed += OnButtonPressed;
            Game.Window.JoystickButtonReleased += OnButtonReleased;
            Game.Window.JoystickConnected += OnJoystickConnected;
            Game.Window.JoystickMoved += OnJoystickMoved;
            Game.Window.MouseWheelMoved += OnMouseWheelMoved;
        }

        internal void GameMouseUpdate(int x, int y) {
            mouseDeltaBufferX += x;
            mouseDeltaBufferY += y;
            gameMouseX += x;
            gameMouseY += y;
            gameMouseX = (int)Util.Clamp(gameMouseX, 0, Game.Width);
            gameMouseY = (int)Util.Clamp(gameMouseY, 0, Game.Height);
        }

        internal void Init() {
            LastButton = new List<int>();

            for (uint i = 0; i < Joystick.Count; i++) {
                activeButtons.Add(new Dictionary<uint, bool>());
                currentButtons.Add(new Dictionary<uint, bool>());
                previousButtons.Add(new Dictionary<uint, bool>());

                for (uint j = 0; j < Joystick.ButtonCount; j++) {
                    activeButtons[(int)i][j] = false;
                    currentButtons[(int)i][j] = false;
                    previousButtons[(int)i][j] = false;
                }
                foreach (AxisButton axisButton in Enum.GetValues(typeof(AxisButton))) {
                    activeButtons[(int)i][(uint)axisButton] = false;
                    currentButtons[(int)i][(uint)axisButton] = false;
                    previousButtons[(int)i][(uint)axisButton] = false;
                }

                buttonsPressed.Add(0);
                prevButtonsPressed.Add(0);

                LastButton.Add(-1);

                buttonReleaseBuffer.Add(new List<uint>());

                Joystick.Update();
            }

            foreach (Key key in Enum.GetValues(typeof(Key))) {
                activeKeys.Add(key, false);
                currentKeys.Add(key, false);
                previousKeys.Add(key, false);
            }

            foreach (MouseButton button in Enum.GetValues(typeof(MouseButton))) {
                activeMouseButtons.Add(button, false);
                currentMouseButtons.Add(button, false);
                previousMouseButtons.Add(button, false);
            }

            foreach (JoyAxis axis in Enum.GetValues(typeof(JoyAxis))) {
                axisThreshold.Add(axis, 0.5f);
            }

            AxisSet ax;

            ax.Plus = AxisButton.XPlus;
            ax.Minus = AxisButton.XMinus;
            axisSet.Add(JoyAxis.X, ax);

            ax.Plus = AxisButton.YPlus;
            ax.Minus = AxisButton.YMinus;
            axisSet.Add(JoyAxis.Y, ax);

            ax.Plus = AxisButton.ZPlus;
            ax.Minus = AxisButton.ZMinus;
            axisSet.Add(JoyAxis.Z, ax);

            ax.Plus = AxisButton.RPlus;
            ax.Minus = AxisButton.RMinus;
            axisSet.Add(JoyAxis.R, ax);

            ax.Plus = AxisButton.UPlus;
            ax.Minus = AxisButton.UMinus;
            axisSet.Add(JoyAxis.U, ax);

            ax.Plus = AxisButton.VPlus;
            ax.Minus = AxisButton.VMinus;
            axisSet.Add(JoyAxis.V, ax);

            ax.Plus = AxisButton.PovXPlus;
            ax.Minus = AxisButton.PovXMinus;
            axisSet.Add(JoyAxis.PovX, ax);

            ax.Plus = AxisButton.PovYPlus;
            ax.Minus = AxisButton.PovYMinus;
            axisSet.Add(JoyAxis.PovY, ax);

        }

        internal void Update() {
            // Set instance pointer to this object.
            Instance = this;

            // Do mouse delta stuff for when the mouse is locked in the game window.
            MouseDeltaX = mouseDeltaBufferX;
            MouseDeltaY = mouseDeltaBufferY;

            mouseDeltaBufferX = 0;
            mouseDeltaBufferY = 0;

            // Force update all joysticks.
            Joystick.Update();

            // Update the previous button dictionaries.
            previousKeys = new Dictionary<Key, bool>(currentKeys);
            previousMouseButtons = new Dictionary<MouseButton, bool>(currentMouseButtons);
            for (int i = 0; i < previousButtons.Count; i++) {
                previousButtons[i] = new Dictionary<uint, bool>(currentButtons[i]);
            }

            // Update the previous press counts
            prevKeysPressed = currentKeysPressed;
            prevMouseButtonsPressed = currentMouseButtonsPressed;
            for (int i = 0; i < prevButtonsPressed.Count; i++) {
                prevButtonsPressed[i] = buttonsPressed[i];
            }

            // Update the current to the active keys.
            currentKeys = new Dictionary<Key, bool>(activeKeys);
            currentMouseButtons = new Dictionary<MouseButton, bool>(activeMouseButtons);
            for (int i = 0; i < currentButtons.Count; i++) {
                currentButtons[i] = new Dictionary<uint, bool>(activeButtons[i]);
            }

            foreach (var k in keyReleaseBuffer) {
                activeKeys[k] = false;
            }

            currentKeysPressed = keysPressed;

            keyReleaseBuffer.Clear();

            foreach (var m in mouseReleaseBuffer) {
                activeMouseButtons[m] = false;
            }

            currentMouseButtonsPressed = mouseButtonsPressed;

            mouseReleaseBuffer.Clear();

            for (int i = 0; i < Joystick.Count; i++) {
                foreach (var b in buttonReleaseBuffer[i]) {
                    activeButtons[i][b] = false;
                }

                buttonReleaseBuffer[i].Clear();
            }

            // Update the Joystick axes to use as buttons
            for (int i = 0; i < Joystick.Count; i++) {
                if (Joystick.IsConnected((uint)i)) {
                    foreach (JoyAxis axis in Enum.GetValues(typeof(JoyAxis))) {
                        float a = GetAxis(axis, i) * 0.01f;
                        if (a >= axisThreshold[axis]) {
                            if (!currentButtons[i][(uint)axisSet[axis].Plus]) {
                                buttonsPressed[i]++;
                            }
                            currentButtons[i][(uint)axisSet[axis].Plus] = true;
                        }
                        else {
                            if (currentButtons[i][(uint)axisSet[axis].Plus]) {
                                buttonsPressed[i]--;
                            }
                            currentButtons[i][(uint)axisSet[axis].Plus] = false;
                        }

                        if (a <= -axisThreshold[axis]) {
                            if (!currentButtons[i][(uint)axisSet[axis].Minus]) {
                                buttonsPressed[i]++;
                            }
                            currentButtons[i][(uint)axisSet[axis].Minus] = true;
                        }
                        else {
                            if (currentButtons[i][(uint)axisSet[axis].Minus]) {
                                buttonsPressed[i]--;
                            }
                            currentButtons[i][(uint)axisSet[axis].Minus] = false;
                        }
                    }
                }
            }

            mouseWheelDelta = currentMouseWheelDelta;
            currentMouseWheelDelta = 0;
        }

        #endregion

    }

    #region Enum

    /// <summary>
    /// Buttons that represent the possible axes on a joystick.
    /// </summary>
    public enum AxisButton {
        XPlus = 100,
        XMinus,
        YPlus,
        YMinus,
        ZPlus,
        ZMinus,
        RPlus,
        RMinus,
        UPlus,
        UMinus,
        VPlus,
        VMinus,
        PovXPlus,
        PovXMinus,
        PovYPlus,
        PovYMinus,
        Any = 1000
    }

    /// <summary>
    /// Mouse buttons.  Buttons on your mouse.
    /// </summary>
    public enum MouseButton {
        Left = 0,
        Right = 1,
        Middle = 2,
        XButton1 = 3,
        XButton2 = 4,
        ButtonCount = 5,
        Any = 1000
    }

    /// <summary>
    /// The direction of the mouse wheel.
    /// </summary>
    public enum MouseWheelDirection {
        Up = 0,
        Down
    }

    /// <summary>
    /// All the keys on your keyboard.
    /// </summary>
    public enum Key {
        Unknown = -1,
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
        K = 10,
        L = 11,
        M = 12,
        N = 13,
        O = 14,
        P = 15,
        Q = 16,
        R = 17,
        S = 18,
        T = 19,
        U = 20,
        V = 21,
        W = 22,
        X = 23,
        Y = 24,
        Z = 25,
        Num0 = 26,
        Num1 = 27,
        Num2 = 28,
        Num3 = 29,
        Num4 = 30,
        Num5 = 31,
        Num6 = 32,
        Num7 = 33,
        Num8 = 34,
        Num9 = 35,
        Escape = 36,
        LControl = 37,
        LShift = 38,
        LAlt = 39,
        LSystem = 40,
        RControl = 41,
        RShift = 42,
        RAlt = 43,
        RSystem = 44,
        Menu = 45,
        LBracket = 46,
        RBracket = 47,
        SemiColon = 48,
        Comma = 49,
        Period = 50,
        Quote = 51,
        Slash = 52,
        BackSlash = 53,
        Tilde = 54,
        Equal = 55,
        Dash = 56,
        Space = 57,
        Return = 58,
        Back = 59,
        Tab = 60,
        PageUp = 61,
        PageDown = 62,
        End = 63,
        Home = 64,
        Insert = 65,
        Delete = 66,
        Add = 67,
        Subtract = 68,
        Multiply = 69,
        Divide = 70,
        Left = 71,
        Right = 72,
        Up = 73,
        Down = 74,
        Numpad0 = 75,
        Numpad1 = 76,
        Numpad2 = 77,
        Numpad3 = 78,
        Numpad4 = 79,
        Numpad5 = 80,
        Numpad6 = 81,
        Numpad7 = 82,
        Numpad8 = 83,
        Numpad9 = 84,
        F1 = 85,
        F2 = 86,
        F3 = 87,
        F4 = 88,
        F5 = 89,
        F6 = 90,
        F7 = 91,
        F8 = 92,
        F9 = 93,
        F10 = 94,
        F11 = 95,
        F12 = 96,
        F13 = 97,
        F14 = 98,
        F15 = 99,
        Pause = 100,
        KeyCount = 101,
        Any = 1000
    }

    /// <summary>
    /// Flags to represent Direction.
    /// </summary>
    [Flags]
    public enum Direction {
        None = 0,
        Up = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Left = 1 << 3,
        UpRight = Up | Right,
        UpLeft = Up | Left,
        DownRight = Down | Right,
        DownLeft = Down | Left
    }

    /// <summary>
    /// Axes on a joystick.
    /// </summary>
    public enum JoyAxis {
        X,
        Y,
        Z,
        R,
        U,
        V,
        PovX,
        PovY
    }

    #endregion

}
