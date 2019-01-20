namespace Otter {
    /// <summary>
    /// Class used to represent a range using a min and max.
    /// </summary>
    public class Range {

        #region Public Fields

        /// <summary>
        /// The minimum of the range.
        /// </summary>
        public float Min;

        /// <summary>
        /// The maximum of the range.
        /// </summary>
        public float Max;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get a random int from the range.  Floors the Min and Ceils the Max.
        /// </summary>
        /// <returns>A random int.</returns>
        public int RandInt {
            get {
                return Rand.Int((int)Min, (int)Util.Ceil(Max));
            }
        }

        /// <summary>
        /// Get a random float from the range.
        /// </summary>
        /// <returns>A random float.</returns>
        public float RandFloat {
            get {
                return Rand.Float(Min, Max);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Range.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public Range(float min, float max) {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Create a new Range.
        /// </summary>
        /// <param name="max">Maximum value.  Minimum is -Maximum.</param>
        public Range(float max) : this(-max, max) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test if this Range overlaps another Range.
        /// </summary>
        /// <param name="r">The Range to test against.</param>
        /// <returns>True if the ranges overlap.</returns>
        public bool Overlap(Range r) {
            if (r.Max < Min) return false;
            if (r.Min > Max) return false;
            return true;
        }

        public override string ToString() {
            return string.Format("{0}, {1}", Min, Max);
        }

        #endregion

    }
}
