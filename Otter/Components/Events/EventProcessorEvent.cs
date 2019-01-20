using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class EventProcessorEvent {

        #region Public Fields

        /// <summary>
        /// The EventProcessor that this event belongs to.
        /// </summary>
        public EventProcessor EventProcessor;

        /// <summary>
        /// The elapsed time for this event.
        /// </summary>
        public float Timer = 0;

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// The Entity that has the EventProcessor that this event belongs to.
        /// </summary>
        public Entity Entity {
            get { return EventProcessor.Entity; }
        }

        /// <summary>
        /// The Scene that has the Entity that has the EventProcessor that this event belongs to.
        /// </summary>
        public Scene Scene {
            get { return Entity.Scene; }
        }

        /// <summary>
        /// Whether or not the Event has finished.
        /// </summary>
        public bool IsFinished { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// The Scene that has the Entity that has the EventProcessor that this event belongs to.
        /// </summary>
        /// <typeparam name="T">The Type the Scene should be cast to.</typeparam>
        /// <returns>The Scene that has the Entity that has the EventProcessor that this event belongs to.</returns>
        public T GetScene<T>() where T : Scene {
            return (T)Scene;
        }

        /// <summary>
        /// Called when the Event first starts.
        /// </summary>
        public virtual void Begin() {

        }

        /// <summary>
        /// Called when the Event finishes and is cleared from the queue.
        /// </summary>
        public virtual void End() {

        }

        /// <summary>
        /// Finishes the event.
        /// </summary>
        public void Finish() {
            IsFinished = true;
        }

        /// <summary>
        /// Starts the event.
        /// </summary>
        public void Start() {
            IsFinished = false;
            Timer = 0;
        }

        /// <summary>
        /// Called every update from the EventProcessor.
        /// </summary>
        public virtual void Update() {

        }

        #endregion Public Methods
    }
}
