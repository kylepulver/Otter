using System;

namespace Otter {
    /// <summary>
    /// Component used for a value with built in min, max, and wrapping capabilities. Can be useful for making
    /// menus.
    /// </summary>
    public class Counter : Component {

        #region Public Fields

        /// <summary>
        /// The current value of the Counter.
        /// </summary>
        public int Value = 0;

        /// <summary>
        /// Determines if the value should wrap around when exceeding the min or max.
        /// </summary>
        public bool Wrap = false;

        /// <summary>
        /// Determines if the value should be clamped by the minimum and maximum value.
        /// </summary>
        public bool Cap = true;

        /// <summary>
        /// The minimum value of the Counter.
        /// </summary>
        public int Min = 0;

        /// <summary>
        /// The maximum value of the Counter.
        /// </summary>
        public int Max = 0;

        /// <summary>
        /// The starting value of the Counter.
        /// </summary>
        public int InitialValue = 0;

        /// <summary>
        /// A callback for when the Counter increments.
        /// </summary>
        public Action OnIncrement;

        /// <summary>
        /// A callback for when the Counter decrements.
        /// </summary>
        public Action OnDecrement;

        /// <summary>
        /// A callback for when the counter reaches the maximum value.
        /// </summary>
        public Action OnMax;

        /// <summary>
        /// A callback for when the counter reaches the minimum value.
        /// </summary>
        public Action OnMin;

        #endregion

        #region Public Properties

        /// <summary>
        /// If the Counter is currently at or exceeding the maximum value.
        /// </summary>
        public bool AtMax {
            get { return Value >= Max; }
        }

        /// <summary>
        /// If the Counter is currently at or exceeding the minimum value.
        /// </summary>
        public bool AtMin {
            get { return Value <= Min; }
        }

        /// <summary>
        /// The length of the Counter from Min to Max.
        /// </summary>
        public int Length {
            get { return Math.Abs(Max - Min) + 1; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Counter.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="wrap">If the counter should wrap when it reaches the minimum or maximum values.</param>
        /// <param name="cap">If the counter shouldn't be allowed to exceed the minimum or maximum values.</param>
        public Counter(int value, int min, int max, bool wrap = false, bool cap = true) {
            InitialValue = value;
            Value = value;
            if (min > max) throw new ArgumentException("Min must be lower than max!");

            Min = min;
            Max = max;
            Wrap = wrap;
            Cap = cap;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the Counter back to its initial value.
        /// </summary>
        public void Reset() {
            Value = InitialValue;
        }

        /// <summary>
        /// Increment the value of the Counter.
        /// </summary>
        /// <param name="value">How much to increment by.</param>
        /// <returns>The new value.</returns>
        public int Increment(int value = 1) {
            Value += value;
            if (Cap) {
                if (Value > Max) {
                    if (Wrap) {
                        while (Value > Max) {
                            Value -= (Length);
                        }
                    }
                    else Value = Max;
                }
            }
            return Value;
        }

        /// <summary>
        /// Decrement the value of the Counter.
        /// </summary>
        /// <param name="value">How much to decrement by.</param>
        /// <returns>The new value.</returns>
        public int Decrement(int value = 1) {
            Value -= value;
            if (Cap) {
                if (Value < Min) {
                    if (Wrap) {
                        while (Value < Min) {
                            Value += (Length);
                        }
                    }
                    else Value = Min;
                }
            }
            return Value;
        }

        /// <summary>
        /// Update the Counter.
        /// </summary>
        public override void Update() {
            if (Cap) {
                if (Value > Max) {
                    if (Wrap) {
                        while (Value > Max) {
                            Value -= (Length);
                        }
                    }
                    else Value = Max;
                }
                if (Value < Min) {
                    if (Wrap) {
                        while (Value < Min) {
                            Value += (Length);
                        }
                    }
                    else Value = Min;
                }
            }
        }

        /// <summary>
        /// Force the value to the maximum value.
        /// </summary>
        public void GoToMax() {
            Value = Max;
        }

        /// <summary>
        /// Force the value to the minimum value.
        /// </summary>
        public void GoToMin() {
            Value = Min;
        }

        #endregion

        #region Operators

        public static implicit operator float(Counter counter) {
            return counter.Value;
        }
        public static implicit operator int(Counter counter) {
            return (int)counter.Value;
        }

        #endregion
        
    }
}
