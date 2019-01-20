// Copyright (c) 2009-2012 David Koontz, Dan Peschman, and Logan Barnett
// Please direct any bugs/comments/suggestions to david@koontzfamily.org
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;

/// <summary>
/// 	Class for broadcasting messages to subscribers.  Clients of the EventRouter register 
/// 	themselves by calling Subscribe and passing in the even they are interested in along with
/// 	a delegate to be called back when the event is received.  Events are published via the 
/// 	Publish method which allows arbitrary data to be passed along with the event to be interpreted
/// 	by the event receiver.  Events do not have to be pre-defined.
/// 
/// 	Publish and Subscribe methods also have an optional id parameter which will filter the events
/// 	sent to just those that match the provided id.  Subscribing to an event with no id will result 
/// 	in receiving all events of the specified type.
/// </summary>
/// <example>
/// 	Example of a simple publish and subscribe class, not using any id filtering.
/// 
/// 	Sender.cs
///		<code>
///		
///		public enum SenderEvent {
///			Test
///		}
///		
///		public class Sender {
///			public void Send() {
///				EventRouter.Publish(SenderEvent.Test, "Hello World");
///			}
///		}
/// 	</code>
///
/// 	Receiver.cs
/// 	<code>
///		
///		public class Receiver {
///			public Receiver() {
///				EventRouter.Subscribe(SenderEvent.Test, OnSenderEvent);
///			}
///			
///			void OnSenderEvent(EventRouter.Event evt) {
///				if(evt.HasData) {
///					Console.WriteLine("Received event: " + evt.Type + " with data: " + evt.GetData<string>(0));
///				}
///				else {
///					Console.WriteLine("Received event: " + evt.Type + " with no data");
///				}
///			}
///		}
/// 	</code>
/// </example>
/// 
/// <example>
/// 	Example of a publish and subscribe using id's to filter specific messages.
/// 
/// 	Sender.cs
///		<code>
///		
///		public enum SenderEvent {
///			Test
///		}
///		
///		public class Sender {
///			public void SendA() {
///				EventRouter.Publish("A", SenderEvent.Test);
///			}
///			
///			public void SendB() {
///				EventRouter.Publish("B", SenderEvent.Test);
///			}
///		}
/// 	</code>
///
/// 	Receiver.cs
/// 	<code>
///		
///		public class Receiver {
///			public Receiver() {
///				EventRouter.Subscribe("A", SenderEvent.Test, OnSenderEventA);
///				EventRouter.Subscribe("B", SenderEvent.Test, OnSenderEventB);
///			}
///			
///			void OnSenderEventA(EventRouter.Event evt) {
///				Console.WriteLine("Received SenderEvent.Test with id 'A'");
///			}
///
///			void OnSenderEventB(EventRouter.Event evt) {
///				Console.WriteLine("Received SenderEvent.Test with id 'B'");
///			}
///		}
/// 	</code>
/// </example>
public class EventRouter {
    /// <summary>
    /// 	Event data class, passed in to the subscriber whenever an event occours.
    /// </summary>
    public class Event {
        public string Type;
        public string Id;
        public object[] Data;

        public bool HasData {
            get { return Data != null && Data.Length > 0; }
        }

        public T GetData<T>(int index) {
            return (T)Data[index];
        }
    }

    public delegate void Handler(Event e);

    public static bool LogEvents = false;

    static Dictionary<string, Dictionary<string, Handler>> handlers = new Dictionary<string, Dictionary<string, Handler>>();
    static Dictionary<object, Dictionary<string, Handler>> owners = new Dictionary<object, Dictionary<string, Handler>>(); // For unsubscribing via object reference
    static Queue<Event> queuedEvents = new Queue<Event>();

    /// <summary>
    /// Subscribe to the event specified by evt.  Pass in a delegate to be called back when the even occurs.
    /// </summary>
    /// <param name='evt'>
    /// The event enumeration value.
    /// </param>
    /// <param name='h'>
    /// The delegate to be called when the even occurs.
    /// </param>
    public static void Subscribe(Enum evt, Handler h, object owner = null) {
        Subscribe("", EnumValueToString(evt), h, owner);
    }

    /// <summary>
    /// Subscribe to the event specified by evt filtered by the specified id.  Pass in a delegate to be called back when the even occurs.
    /// </summary>
    /// <param name='id'>
    /// The id of the event
    /// </param>
    /// <param name='evt'>
    /// The event enumeration value.
    /// </param>
    /// <param name='h'>
    /// The delegate to be called when the even occurs.
    /// </param>
    public static void Subscribe(string id, Enum evt, Handler h, object owner = null) {
        Subscribe(id, EnumValueToString(evt), h, owner);
    }

    /// <summary>
    /// Subscribe to the event specified by evt.  Pass in a delegate to be called back when the even occurs.
    /// </summary>
    /// <param name='id'>
    /// The string representing the id.
    /// </param>
    /// <param name='evt'>
    /// The string representing the event.
    /// </param>
    /// <param name='h'>
    /// The delegate to be called when the even occurs.
    /// </param>
    public static void Subscribe(string id, string evt, Handler h, object owner = null) {
        if (!handlers.ContainsKey(evt)) {
            handlers.Add(evt, new Dictionary<string, Handler>());
        }

        if (!handlers[evt].ContainsKey(id)) {
            handlers[evt].Add(id, null);
        }

        if (owner != null) {
            if (!owners.ContainsKey(owner)) {
                owners.Add(owner, new Dictionary<string, Handler>());
            }

            owners[owner].Add(evt, h);
        }

        handlers[evt][id] += h;
    }

    /// <summary>
    /// Subscribe to the event specified by evt.  Pass in a delegate to be called back when the even occurs.
    /// </summary>
    /// <param name='evt'>
    /// The string representing the event.
    /// </param>
    /// <param name='h'>
    /// The delegate to be called when the even occurs.
    /// </param>
    public static void Subscribe(string evt, Handler h, object owner = null) {
        Subscribe("", evt, h, owner);
    }

    /// <summary>
    /// Unsubscribe the specified delegate from the event.
    /// </summary>
    /// <param name='id'>
    /// The id of the event
    /// </param>
    /// <param name='evt'>
    /// The event enumeration value.
    /// </param>
    /// <param name='h'>
    /// The delegate to be removed from the event handlers.
    /// </param>
    public static void Unsubscribe(string id, Enum evt, Handler h) {
        Unsubscribe(id, EnumValueToString(evt), h);
    }

    /// <summary>
    /// Unsubscribe the specified delegate from the event.
    /// </summary>
    /// <param name='evt'>
    /// The event enumeration value.
    /// </param>
    /// <param name='h'>
    /// The delegate to be removed from the event handlers.
    /// </param>
    public static void Unsubscribe(Enum evt, Handler h) {
        Unsubscribe(EnumValueToString(evt), h);
    }

    /// <summary>
    /// Unsubscribe the specified delegate from the event.
    /// </summary>
    /// <param name='evt'>
    /// The string representing the event.
    /// </param>
    /// <param name='h'>
    /// The delegate to be removed from the event handlers.
    /// </param>
    public static void Unsubscribe(string evt, Handler h) {
        Unsubscribe("", evt, h);
    }

    /// <summary>
    /// Unsubscribe the specified delegate from the event.
    /// </summary>
    /// <param name='id'>
    /// The string representing the id.
    /// </param>
    /// <param name='evt'>
    /// The string representing the event.
    /// </param>
    /// <param name='h'>
    /// The delegate to be removed from the event handlers.
    /// </param>
    public static void Unsubscribe(string id, string evt, Handler h) {
        if (handlers.ContainsKey(evt) && handlers[evt].ContainsKey(id)) {
            handlers[evt][id] -= h;
            if (handlers[evt][id] == null) handlers.Remove(evt);
        }
    }

    /// <summary>
    /// Unsubscribe to all events that were subscribed to by a specific owner object.
    /// </summary>
    /// <param name="owner">The object that subscribed to the object.</param>
    public static void Unsubscribe(object owner) {
        if (owners.ContainsKey(owner)) {
            foreach (var kv in owners[owner]) {
                Unsubscribe(kv.Key, kv.Value);
            }
            owners.Remove(owner);
        }
    }

    /// <summary>
    /// Publish the specified event with extra data.
    /// </summary>
    /// <param name='id'>
    /// The string representing the object.
    /// </para>
    /// <param name='evt'>
    /// The string representing the event.
    /// </param>
    /// <param name='data'>
    /// An arbitrary params array of objects to be interpreted by the receiver of the event.
    /// </param>
    public static void Publish(string id, string evt, params object[] data) {
        //if (LogEvents) Debug.Log(string.Format("Event Published: '{0}' with id: '{1}' and {2} parameters", evt, id, data.Length));

        ProcessEvent(id, evt, data);

        while (queuedEvents.Count > 0) {
            var queuedEvent = queuedEvents.Dequeue();
            ProcessEvent(queuedEvent.Id, queuedEvent.Type, queuedEvent.Data);
        }
    }

    /// <summary>
    /// Publish the specified event with extra data.
    /// </summary>
    /// <param name='evt'>
    /// The string representing the event.
    /// </param>
    /// <param name='data'>
    /// An arbitrary params array of objects to be interpreted by the receiver of the event.
    /// </param>
    public static void Publish(string evt, params object[] data) {
        Publish("", evt, data);
    }

    /// <summary>
    /// Publish the specified event with extra data.
    /// </summary>
    /// <param name='id'>
    /// The string representing the object.
    /// </para>
    /// <param name='evt'>
    /// The event enumeration value.
    /// </param>
    /// <param name='data'>
    /// An arbitrary params array of objects to be interpreted by the receiver of the event.
    /// </param>
    public static void Publish(string id, Enum evt, params object[] data) {
        Publish(id, EnumValueToString(evt), data);
    }

    /// <summary>
    /// Publish the specified event with extra data.
    /// </summary>
    /// <param name='evt'>
    /// The event enumeration value.
    /// </param>
    /// <param name='data'>
    /// An arbitrary params array of objects to be interpreted by the receiver of the event.
    /// </param>
    public static void Publish(Enum evt, params object[] data) {
        Publish(EnumValueToString(evt), data);
    }

    public static void Queue(string id, string evt, params object[] data) {
        var e = new Event { Type = evt, Id = id };
        if (data != null && data.Length > 0) e.Data = data;
        queuedEvents.Enqueue(e);
    }

    public static void Queue(string evt, params object[] data) {
        Queue(string.Empty, evt, data);
    }

    public static void Queue(string id, Enum evt, params object[] data) {
        Queue(id, EnumValueToString(evt), data);
    }

    public static void Queue(Enum evt, params object[] data) {
        Queue(string.Empty, EnumValueToString(evt), data);
    }

    /// <summary>
    /// Clear all event subscribers, used primarily when switching or resetting a level.
    /// </summary>
    public static void Clear() {
        handlers.Clear();
    }

    static void ProcessEvent(string id, string evt, params object[] data) {
        Dictionary<string, Handler> handlersForType;
        if (handlers.TryGetValue(evt, out handlersForType)) {
            Handler handler;
            if (handlersForType.TryGetValue(id, out handler)) {
                var e = new Event { Type = evt, Id = id };
                if (data != null && data.Length > 0) e.Data = data;
                handler(e);
            }

            // invoke handlers that don't care about a particular id
            if (id != string.Empty && handlersForType.TryGetValue(string.Empty, out handler)) {
                var e = new Event { Type = evt, Id = id };
                if (data != null && data.Length > 0) e.Data = data;
                handler(e);
            }
        }
    }

    static string EnumValueToString(Enum value) {
        return string.Format("{0}.{1}", value.GetType(), value);
    }
}