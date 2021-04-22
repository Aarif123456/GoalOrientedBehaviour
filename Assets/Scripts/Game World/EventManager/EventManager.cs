#region Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

// Microsoft Reciprocal License (Ms-RL)
//
// This license governs use of the accompanying software. If you use the software, you accept this
// license. If you do not accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
// meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// copyright license to reproduce its contribution, prepare derivative works of its contribution,
// and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works of the contribution in
// the software.
//
// 3. Conditions and Limitations
// (A) Reciprocal Grants- For any file you distribute that contains code from the software (in
// source code or binary format), you must provide recipients the source code to that file along
// with a copy of this license, which license will govern that file. You may license other files
// that are entirely your own work and do not contain code from the software under any terms you
// choose.
// (B) No Trademark License- This license does not grant you rights to use any contributors' name,
// logo, or trademarks.
// (C) If you bring a patent claim against any contributor over patents that you claim are
// infringed by the software, your patent license from such contributor to the software ends
// automatically.
// (D) If you distribute any portion of the software, you must retain all copyright, patent,
// trademark, and attribution notices that are present in the software.
// (E) If you distribute any portion of the software in source code form, you may do so only under
// this license by including a complete copy of this license with your distribution. If you
// distribute any portion of the software in compiled or object code form, you may only do so under
// a license that complies with this license.
// (F) The software is licensed "as-is." You bear the risk of using it. The contributors give no
// express warranties, guarantees or conditions. You may have additional consumer rights under your
// local laws which this license cannot change. To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for a particular purpose
// and non-infringement.

#endregion Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

using System.Collections.Generic;
using Entities;
using UnityEngine;
using Utility.Data_Structures;
using Debug = System.Diagnostics.Debug;

namespace GameWorld {
    /// <summary>
    ///     Manager for events. Events can be fired for immediate processing or queued for later processing. Objects that
    ///     subscribe to an event are notified when it is processed via its event delegate. Objects cease to be notified
    ///     when they unsubscribe from an event. Events can also be scheduled and can be directed to specified receivers.
    /// </summary>
    public sealed partial class EventManager : MonoBehaviour {
        /// <summary>
        ///     The id of the sender is irrelevant (system generated).
        /// </summary>
        public const int SENDER_ID_IRRELEVANT = -1;

        /// <summary>
        ///     The id of the receiver is irrelevant (system generated).
        /// </summary>
        public const int RECEIVER_ID_IRRELEVANT = -1;

        /// <summary>
        ///     Event should be dispatched without delay.
        /// </summary>
        public const double NO_DELAY = 0.0f;

        private static EventManager _instance;

        /// <summary>
        ///     Dictionary used to get event subscribers by event type.
        /// </summary>
        private readonly Dictionary<EventType, List<Subscription>> _eventSubscribers =
            new Dictionary<EventType, List<Subscription>>();

        /// <summary>
        ///     Priority queue to gather events as they are enqueued.
        /// </summary>
        private PriorityQueue<Event, double> _eventGatherQueue =
            new PriorityQueue<Event, double>(PriorityQueue<Event, double>.PriorityOrder.LowFirst);

        /// <summary>
        ///     Priority queue of events taken from the <see cref="_eventGatherQueue" /> that can now be processed.
        /// </summary>
        private PriorityQueue<Event, double> _eventProcessQueue =
            new PriorityQueue<Event, double>(PriorityQueue<Event, double>.PriorityOrder.LowFirst);

        /// <summary>
        ///     A value indicating whether the event manager is currently processing events.
        /// </summary>
        private bool _isProcessingEvents;

        /// <summary>
        ///     The next event id.
        /// </summary>
        private int _nextEventId;

        public static EventManager Instance {
            get { return _instance ??= GameObject.Find("Game").GetComponent<EventManager>(); }
        }

        /// <summary>
        ///     Process all events.
        /// </summary>
        public void Update(){
            // Fire a (non-queued) update event per cycle. Trigger processes the
            // event immediately without putting it on the event queue.
            Fire(Events.ImmediateUpdate, Time.time);

            // post a (queued) update event per cycle.
            Enqueue(Events.QueuedUpdate, Time.time);

            while (ProcessEvents()){
            }
        }

        public void Subscribe<T>(
            EventType eventType,
            EventDelegate<T> eventDelegate){
            Subscribe(eventType, eventDelegate, null);
        }

        public void Subscribe<T>(
            EventType eventType,
            EventDelegate<T> eventDelegate,
            object eventKey){
            var subscriptionToAdd = new Subscription(eventDelegate, eventKey);

            lock (_eventSubscribers){
                List<Subscription> eventSubscriptionList;
                if (_eventSubscribers.TryGetValue(eventType, out eventSubscriptionList) &&
                    eventSubscriptionList != null){
                    if (!eventSubscriptionList.Contains(subscriptionToAdd))
                        eventSubscriptionList.Add(subscriptionToAdd);

                    return;
                }

                _eventSubscribers[eventType] = new List<Subscription>{subscriptionToAdd};
            }
        }

        public void Unsubscribe<T>(
            EventType eventType,
            EventDelegate<T> eventDelegate){
            Unsubscribe(eventType, eventDelegate, null);
        }

        public void Unsubscribe<T>(
            EventType eventType,
            EventDelegate<T> eventDelegate,
            object eventKey){
            var subscriptionToRemove = new Subscription(eventDelegate, eventKey);

            lock (_eventSubscribers){
                List<Subscription> eventSubscriptionList;
                if (_eventSubscribers.TryGetValue(eventType, out eventSubscriptionList) &&
                    eventSubscriptionList != null)
                    eventSubscriptionList.Remove(subscriptionToRemove);
            }
        }

        public int Enqueue<T>(
            EventType eventType,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Cycle, NO_DELAY, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT,
                null,
                eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            double delay,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Level, delay, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT, null,
                eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            double delay,
            EventDelegate<T> eventDelegate,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Level, delay, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT,
                eventDelegate, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            EventDelegate<T> eventDelegate,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Cycle, NO_DELAY, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT,
                eventDelegate, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            double delay,
            int receiverId,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Level, delay, SENDER_ID_IRRELEVANT, receiverId, null, eventData,
                null);
        }

        public int Enqueue<T>(
            EventType eventType,
            int receiverId,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Cycle, NO_DELAY, SENDER_ID_IRRELEVANT, receiverId, null,
                eventData,
                null);
        }

        public int Enqueue<T>(
            EventType eventType,
            double delay,
            int senderId,
            int receiverId,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Level, delay, senderId, receiverId, null, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            int senderId,
            int receiverId,
            T eventData){
            return Enqueue(eventType, Event.Lifespans.Cycle, NO_DELAY, senderId, receiverId, null, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            int senderId,
            int receiverId,
            EventDelegate<T> eventDelegate,
            T eventData,
            object eventKey){
            return Enqueue(eventType, Event.Lifespans.Cycle, NO_DELAY, senderId, receiverId, eventDelegate, eventData,
                eventKey);
        }

        public int Enqueue<T>(
            EventType eventType,
            double delay,
            int senderId,
            int receiverId,
            EventDelegate<T> eventDelegate,
            T eventData,
            object eventKey){
            return Enqueue(eventType, Event.Lifespans.Level, delay, senderId, receiverId, eventDelegate, eventData,
                eventKey);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            T eventData){
            return Enqueue(eventType, lifespan, NO_DELAY, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT, null, eventData,
                null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            double delay,
            T eventData){
            return Enqueue(eventType, lifespan, delay, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT, null, eventData,
                null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            double delay,
            EventDelegate<T> eventDelegate,
            T eventData){
            return Enqueue(eventType, lifespan, delay, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT, eventDelegate,
                eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            EventDelegate<T> eventDelegate,
            T eventData){
            return Enqueue(eventType, lifespan, NO_DELAY, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT, eventDelegate,
                eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            double delay,
            int receiverId,
            T eventData){
            return Enqueue(eventType, lifespan, delay, SENDER_ID_IRRELEVANT, receiverId, null, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            int receiverId,
            T eventData){
            return Enqueue(eventType, lifespan, NO_DELAY, SENDER_ID_IRRELEVANT, receiverId, null, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            double delay,
            int senderId,
            int receiverId,
            T eventData){
            return Enqueue(eventType, lifespan, delay, senderId, receiverId, null, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            int senderId,
            int receiverId,
            T eventData){
            return Enqueue(eventType, lifespan, NO_DELAY, senderId, receiverId, null, eventData, null);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            int senderId,
            int receiverId,
            EventDelegate<T> eventDelegate,
            T eventData,
            object eventKey){
            return Enqueue(eventType, lifespan, NO_DELAY, senderId, receiverId, eventDelegate, eventData, eventKey);
        }

        public int Enqueue<T>(
            EventType eventType,
            Event.Lifespan lifespan,
            double delay,
            int senderId,
            int receiverId,
            EventDelegate<T> eventDelegate,
            T eventData,
            object eventKey){
            var eventToSchedule =
                Event<T>.Obtain(
                    ++_nextEventId,
                    eventType,
                    lifespan,
                    Time.time + delay, // dispatch time
                    senderId,
                    receiverId,
                    eventDelegate,
                    eventData);

            lock (_eventGatherQueue){
                _eventGatherQueue.Enqueue(eventToSchedule, eventToSchedule.DispatchTime);
            }

            return eventToSchedule.EventId;
        }

        public void Fire<T>(
            EventType eventType,
            T eventData){
            Fire(eventType, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT, null, eventData, null);
        }

        public void Fire<T>(
            EventType eventType,
            EventDelegate<T> eventDelegate,
            T eventData){
            Fire(eventType, SENDER_ID_IRRELEVANT, RECEIVER_ID_IRRELEVANT, eventDelegate, eventData, null);
        }

        public void Fire<T>(
            EventType eventType,
            int receiverId,
            T eventData){
            Fire(eventType, SENDER_ID_IRRELEVANT, receiverId, eventData);
        }

        public void Fire<T>(
            EventType eventType,
            int senderId,
            int receiverId,
            T eventData){
            Fire(eventType, senderId, receiverId, null, eventData, null);
        }

        public void Fire<T>(
            EventType eventType,
            int senderId,
            int receiverId,
            EventDelegate<T> eventDelegate,
            T eventData,
            object eventKey){
            var eventToFire =
                Event<T>.Obtain(++_nextEventId, eventType, Event.Lifespans.Cycle, NO_DELAY, senderId, receiverId,
                    eventDelegate, eventData);
            Fire(eventToFire);
        }

        /// <summary>
        ///     Remove the event with the given event ID.
        /// </summary>
        /// <param name="eventId">
        ///     The ID of the event to remove.
        /// </param>
        /// <returns>
        ///     True if the event was removed.
        /// </returns>
        public bool Remove(int eventId){
            lock (_eventGatherQueue){
                return _eventGatherQueue.Remove(i => i.EventId == eventId);
            }
        }

        public void RemoveAll(Event.Lifespan lifespan){
            lock (_eventGatherQueue){
                _eventGatherQueue.Remove(i => i.EventLifespan == lifespan);
            }
        }

        /// <summary>
        ///     Processes all events queued up since last ProcessEvents call.
        /// </summary>
        /// <returns>
        ///     True if any events were processed.
        /// </returns>
        private bool ProcessEvents(){
            // if already processing event, leave.
            if (_isProcessingEvents) return false;

            // if no events to process, leave.
            if (_eventGatherQueue.Count == 0 || _eventGatherQueue.Peek().Priority > Time.time) return false;

            _isProcessingEvents = true;

            if (_eventProcessQueue.Count != 0)
                Debug.WriteLine("EventManager: event process list should be empty at this point.");

            lock (_eventGatherQueue){
                // We use a double buffer scheme (gather, process) to minimize lock time.
                Swap(ref _eventProcessQueue, ref _eventGatherQueue);
            }

            while (_eventProcessQueue.Count > 0 && _eventProcessQueue.Peek().Priority <= Time.time){
                Fire(_eventProcessQueue.Dequeue().Value);
            }

            lock (_eventGatherQueue){
                // transfer remaining events
                while (_eventProcessQueue.Count > 0){
                    var queueItem = _eventProcessQueue.Dequeue();
                    var unprocessedEvent = queueItem.Value;
                    if (unprocessedEvent.EventLifespan == Event.Lifespans.Cycle){
                        //unprocessedEvent.Recycle(); // shouldn't happen. If it does, event is skipped.
                    }
                    else
                        _eventGatherQueue.Enqueue(queueItem);
                }
            }

            _eventProcessQueue.Clear();

            _isProcessingEvents = false;

            // if we get here, then some events where processed.
            return true;
        }

        /// <summary>
        ///     Fire an event (call the subscriber delegates).
        /// </summary>
        /// <param name="eventToFire">
        ///     The event to fire.
        /// </param>
        private void Fire(Event eventToFire){
            // call subscriber delegates
            List<Subscription> subscriptionList;
            if (eventToFire.EventType != Events.Message &&
                _eventSubscribers.TryGetValue(eventToFire.EventType, out subscriptionList)){
                if (subscriptionList != null){
                    for (var i = 0; i < subscriptionList.Count; i++){
                        eventToFire.Fire(subscriptionList[i].EventDelegate);
                    }
                }
            }

            // notify specified receiver
            if (eventToFire.ReceiverId != RECEIVER_ID_IRRELEVANT){
                if (EntityManager.Find<Entity>(eventToFire.ReceiverId)) eventToFire.Send();
            }
            else if (eventToFire.EventDelegate != null) eventToFire.Fire(eventToFire.EventDelegate);

            //eventToFire.Recycle();
        }

        /// <summary>
        ///     Swap references to two objects.
        /// </summary>
        /// <typeparam name="T">Type of objects to swap.</typeparam>
        /// <param name="a">First object to swap.</param>
        /// <param name="b">Second object to swap.</param>
        private static void Swap<T>(ref T a, ref T b){
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}