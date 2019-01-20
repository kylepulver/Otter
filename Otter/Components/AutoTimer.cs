namespace Otter {
    /// <summary>
    /// A timer that automatically counts on an increment.  Useful for handling things like cooldowns.
    /// </summary>
    public class AutoTimer : Component {

        #region Public Fields

        /// <summary>
        /// The current value of the timer.
        /// </summary>
        public float Value;

        /// <summary>
        /// The maximum possible value of the timer.
        /// </summary>
        public float Max;

        /// <summary>
        /// The minimum possible value of the timer.
        /// </summary>
        public float Min;

        /// <summary>
        /// How much the timer increments each update.
        /// </summary>
        public float Increment = 1;

        #endregion

        #region Public Properties

        /// <summary>
        /// If the timer is currently paused.
        /// </summary>
        public bool Paused { get; private set; }

        /// <summary>
        /// If the timer is currently at its maximum value.
        /// </summary>
        public bool AtMax {
            get { return Value == Max; }
        }

        /// <summary>
        /// If the timer is currently at its minimum value.
        /// </summary>
        public bool AtMin {
            get { return Value == Min; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create an AutoTimer.
        /// </summary>
        /// <param name="max">The maximum value of the timer.</param>
        public AutoTimer(float max) {
            Max = max;
        }

        /// <summary>
        /// Create an AutoTimer.
        /// </summary>
        /// <param name="value">The initial value of the timer.</param>
        /// <param name="min">The minimum value of the timer.</param>
        /// <param name="max">The maximum value of the timer.</param>
        /// <param name="increment">The value that the timer increments with each update.</param>
        public AutoTimer(float value, float min, float max, float increment) {
            Value = value;
            Max = max;
            Min = min;
            Increment = increment;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the AutoTimer.
        /// </summary>
        public override void Update() {
            base.Update();

            if (!Paused) {
                Value += Increment;
            }
            Value = Util.Clamp(Value, Min, Max);
        }

        /// <summary>
        /// Reset the timer to 0.
        /// </summary>
        public void Reset() {
            Value = 0;
        }

        /// <summary>
        /// Pause the timer.
        /// </summary>
        public void Pause() {
            Paused = true;
        }

        /// <summary>
        /// Resume the timer if paused.
        /// </summary>
        public void Resume() {
            Paused = false;
        }

        /// <summary>
        /// Start the timer again from 0.
        /// </summary>
        public void Start() {
            Reset();
            Paused = false;
        }

        /// <summary>
        /// Stop the timer and set the value to 0.
        /// </summary>
        public void Stop() {
            Paused = true;
            Reset();
        }

        #endregion

        #region Operators

        public static implicit operator float(AutoTimer timer) {
            return timer.Value;
        }

        public static implicit operator int(AutoTimer timer) {
            return (int)timer.Value;
        }

        #endregion
    }
}
