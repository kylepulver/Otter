using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class EventProcessor : Component {
        /// <summary>
        /// The list of EventQueueEvents to execute.
        /// </summary>
        public List<EventProcessorEvent> Events = new List<EventProcessorEvent>();

        /// <summary>
        /// The current event that is being executed.
        /// </summary>
        public EventProcessorEvent CurrentEvent { get; protected set; }

        /// <summary>
        /// Determines if the events will be run.  Defaults to true.
        /// </summary>
        public bool RunEvents = true;

        /// <summary>
        /// True if the number of events in the queue is greater than zero.
        /// </summary>
        public bool HasEvents {
            get { return Events.Count > 0; }
        }

        protected bool isFreshEvent = true;
    }
}
