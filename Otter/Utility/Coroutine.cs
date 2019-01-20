using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// shout outs to Chevy Ray for this
namespace Otter
{
    /// <summary>
    /// Class that manages Coroutines.
    /// </summary>
    public class Coroutine {

        #region Static Fields

        /// <summary>
        /// The reference to the main Coroutine object managed by Otter.
        /// </summary>
        public static Coroutine Instance;

        #endregion

        #region Static Methods

        static int nextRoutineId = -1;

        #endregion

        #region Private Fields

        List<IEnumerator> routines = new List<IEnumerator>();
        Dictionary<int, IEnumerator> routineIds = new Dictionary<int, IEnumerator>();
        Dictionary<IEnumerator, int> routineInvertedIds = new Dictionary<IEnumerator, int>();
        Game game;
        List<string> events = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Coroutine manager.
        /// </summary>
        public Coroutine() {

        }

        #endregion

        #region Private Methods

        void Stop(IEnumerator routine) {
            if (routines.Contains(routine)) {
                routines.Remove(routine);
            }
            if (routineIds.ContainsValue(routine)) {
                var key = routineInvertedIds[routine];
                routineIds.Remove(key);
                routineInvertedIds.Remove(routine);
            }
        }

        bool MoveNext(IEnumerator routine) {
            if (routine.Current is IEnumerator)
                if (MoveNext((IEnumerator)routine.Current))
                    return true;
            return routine.MoveNext();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a new coroutine and returns an int id for that routine.
        /// </summary>
        /// <param name="routine">The coroutine to start running.</param>
        /// <returns>A unique int id for that routine.</returns>
        public int Start(IEnumerator routine) {
            routines.Add(routine);
            nextRoutineId++;
            routineIds.Add(nextRoutineId, routine);
            routineInvertedIds.Add(routine, nextRoutineId);
            return nextRoutineId;
        }

        /// <summary>
        /// Immediately clear and stop all Coroutines.
        /// </summary>
        public void StopAll() {
            routines.Clear();
        }

        /// <summary>
        /// Stop a routine from running based off its int id.
        /// </summary>
        /// <param name="routineId">The id of the routine to stop.</param>
        public void Stop(int routineId) {
            if (routineIds.ContainsKey(routineId)) {
                Stop(routineIds[routineId]);
            }
        }

        /// <summary>
        /// Updates all the routines.  The coroutine in the Game automatically runs this.
        /// </summary>
        public void Update() {
            for (int i = 0; i < routines.Count; i++) {
                if (routines[i].Current is IEnumerator)
                    if (MoveNext((IEnumerator)routines[i].Current))
                        continue;
                if (!routines[i].MoveNext()) {
                    var key = routineInvertedIds[routines[i]];
                    routineIds.Remove(key);
                    routineInvertedIds.Remove(routines[i]);
                    routines.RemoveAt(i--);
                }
            }

            events.Clear();
        }

        /// <summary>
        /// The current number of running routines.
        /// </summary>
        public int Count {
            get { return routines.Count; }
        }

        /// <summary>
        /// If any routines are currently running.
        /// </summary>
        public bool Running {
            get { return routines.Count > 0; }
        }

        /// <summary>
        /// Publishes an event to the coroutine manager.  Used for WaitForEvent.
        /// Events are cleared on every update.
        /// </summary>
        /// <param name="id">The string id of the event.</param>
        public void PublishEvent(string id) {
            events.Add(id);
        }

        /// <summary>
        /// Publishes an event to the coroutine manager.  Used for WaitForEvent.
        /// Events are cleared on every update.
        /// </summary>
        /// <param name="id">The enum id of the event.</param>
        public void PublishEvent(Enum id) {
            events.Add(Util.EnumValueToString(id));
        }

        /// <summary>
        /// Waits until a specific event has been published.
        /// </summary>
        /// <param name="id">The string id of the event.</param>
        /// <returns></returns>
        public IEnumerator WaitForEvent(string id) {
            while (!events.Contains(id)) {
                yield return 0;
            }
        }

        /// <summary>
        /// Waits until a specific event has been published.
        /// </summary>
        /// <param name="id">The enum id of the event.</param>
        /// <returns></returns>
        public IEnumerator WaitForEvent(Enum id) {
            while (!events.Contains(Util.EnumValueToString(id))) {
                yield return 0;
            }
        }

        /// <summary>
        /// Check if an event has been published.
        /// </summary>
        /// <param name="id">The string id of the event.</param>
        /// <returns>True if the event has been published.</returns>
        public bool HasEvent(string id) {
            return events.Contains(id);
        }

        /// <summary>
        /// Check if an event has been published.
        /// </summary>
        /// <param name="id">The enum id of the event.</param>
        /// <returns>True if the event has been published.</returns>
        public bool HasEvent(Enum id) {
            return events.Contains(Util.EnumValueToString(id));
        }

        /// <summary>
        /// Waits until an amount of time has passed.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        /// <returns></returns>
        public IEnumerator WaitForSeconds(float seconds) {
            if (game != null) { // Using the game's delta time.
                float elapsed = 0;
                while (elapsed < seconds) {
                    elapsed += game.RealDeltaTime * 0.001f;
                    yield return 0;
                }
            }
            else { // Using a stopwatch.
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds / 1000f < seconds) {
                    yield return 0;
                }
                watch.Stop();
            }
        }

        /// <summary>
        /// Waits until an amount of frames have passed.  Don't use this with non-fixed framerates.
        /// </summary>
        /// <param name="frames">The number of frames to wait.</param>
        /// <returns></returns>
        public IEnumerator WaitForFrames(int frames) {
            int elapsed = 0;
            while (elapsed < frames) {
                elapsed++;
                yield return 0;
            }
        }

        /// <summary>
        /// Waits for a Tween to complete.
        /// </summary>
        /// <param name="tween">The Tween to wait on.</param>
        /// <returns></returns>
        public IEnumerator WaitForTween(Tween tween) {
            while (tween.Completion != 1) yield return 0;
        }

        /// <summary>
        /// Waits for an anonymous method that returns true or false.
        /// </summary>
        /// <param name="func">The method to run until it returns true.</param>
        /// <returns></returns>
        public IEnumerator WaitForDelegate(Func<bool> func) {
            while (func() != true) yield return 0;
        }

        #endregion

        #region Internal

        internal Coroutine(Game game) {
            this.game = game;
            Instance = this;
        }

        #endregion
        
    }
}
