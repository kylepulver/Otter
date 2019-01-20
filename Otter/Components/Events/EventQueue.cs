using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    /// <summary>
    /// A Component to manage and process queue of events.
    /// </summary>
    public class EventQueue : EventProcessor {
        
        #region Public Methods

        /// <summary>
        /// Add events to the queue.
        /// </summary>
        /// <param name="evt">The events to add.</param>
        public void Add(params EventProcessorEvent[] evt) {
            Events.AddRange(evt);
        }

        /// <summary>
        /// Push events into the front of the queue.
        /// </summary>
        /// <param name="evt">The events to push.</param>
        public void Push(params EventProcessorEvent[] evt) {
            Events.InsertRange(0, evt);
        }

        public override void Update() {
            base.Update();

            if (RunEvents) {
                if (CurrentEvent == null) {
                    NextEvent();
                }

                while (CurrentEvent != null) {
                    if (isFreshEvent) {
                        isFreshEvent = false;
                        CurrentEvent.EventProcessor = this;
                        CurrentEvent.Start();
                        CurrentEvent.Begin();
                    }

                    CurrentEvent.Update();
                    CurrentEvent.Timer += Entity.Game.DeltaTime;

                    if (CurrentEvent.IsFinished) {
                        isFreshEvent = true;
                        CurrentEvent.End();
                        CurrentEvent.EventProcessor = null;
                        Events.Remove(CurrentEvent);
                        NextEvent();
                    }
                    else {
                        break;
                    }
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        void NextEvent() {
            if (HasEvents) {
                CurrentEvent = Events[0];
            }
            else {
                CurrentEvent = null;
            }
        }

        #endregion Private Methods
    }
}