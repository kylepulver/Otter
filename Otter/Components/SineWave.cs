namespace Otter {
    /// <summary>
    /// Component that controls a sine wave.  Can be useful for special effects and such.
    /// </summary>
    public class SineWave : Component {

        #region Public Fields

        /// <summary>
        /// The rate at which the sine wave moves.
        /// </summary>
        public float Rate;

        /// <summary>
        /// The amplitude of the sine wave.  When not zero Min and Max are ignored.
        /// </summary>
        public float Amplitude;

        /// <summary>
        /// The offset of the value processed.
        /// </summary>
        public float Offset;

        /// <summary>
        /// The minimum value of the wave.
        /// </summary>
        public float Min;

        /// <summary>
        /// The maximum value of the wave.
        /// </summary>
        public float Max;

        #endregion

        #region Public Properties

        /// <summary>
        /// The current value of the wave.
        /// </summary>
        public float Value {
            get {
                if (Amplitude == 0) {
                    return Util.SinScaleClamp((Timer + Offset) * Rate, Min, Max);
                }
                else {
                    return Util.Sin((Timer + Offset) * Rate) * Amplitude;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new SineWave.
        /// </summary>
        /// <param name="rate">The rate of the wave.</param>
        /// <param name="amp">The amplitude of the wave.</param>
        /// <param name="offset">The offset of the value processed.</param>
        public SineWave(float rate = 1, float amp = 1, float offset = 0) {
            Rate = rate;
            Amplitude = amp;
            Offset = offset;
        }

        /// <summary>
        /// Create a new SineWave.
        /// </summary>
        /// <param name="rate">The rate of the wave.</param>
        /// <param name="min">The minimum value of the wave.</param>
        /// <param name="max">The maximum value of the wave.</param>
        /// <param name="offset">The offset of the value processed.</param>
        public SineWave(float rate = 1, float min = -1, float max = 1, float offset = 0) {
            Rate = rate;
            Min = min;
            Max = max;
            Offset = offset;
        }

        #endregion

        #region Operators

        public static implicit operator float(SineWave s) {
            return s.Value;
        }
        public static implicit operator int(SineWave s) {
            return (int)s.Value;
        }

        #endregion

    }
}
