using System;

namespace Otter {
    /// <summary>
    /// Component that counts down and executes a function.  After it has executed it removes itself.
    /// </summary>
    public class Alarm : Component {

        #region Public Fields

        /// <summary>
        /// The amount of time that must pass before the function activates.
        /// </summary>
        public float Delay;

        /// <summary>
        /// The method to call when the timer reaches the set delay.
        /// </summary>
        public Action Function;

        #endregion

        #region Constructors

        /// <summary>
        /// Create an Alarm.
        /// </summary>
        /// <param name="function">The method to call when the timer reaches the set delay.</param>
        /// <param name="delay">The amount of time that must pass before the method is called.</param>
        public Alarm(Action function, float delay) {
            Function = function;
            Delay = delay;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the Alarm.
        /// </summary>
        public override void Update() {
            base.Update();

            if (Timer >= Delay) {
                if (Function != null) {
                    Function();
                    RemoveSelf();
                }
            }
        }

        #endregion
    }
}
