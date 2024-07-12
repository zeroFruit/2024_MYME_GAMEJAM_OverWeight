using System;
using System.Collections.Generic;
using UnityEngine;

namespace SSR.OverWeight
{
    // /// <summary>
    // /// MMGameEvents are used throughout the game for general game events (game started, game ended, life lost, etc.)
    // /// </summary>
    // public struct GameEvent
    // {
    //     public string EventName;
    //
    //     public GameEvent(string eventName)
    //     {
    //         this.EventName = eventName;
    //     }
    //
    //     static GameEvent e;
    //
    //     public static void Trigger(string eventName)
    //     {
    //         e.EventName = eventName;
    //         EventManager.TriggerEvent(e);
    //     }
    // }
    
    /// <summary>
    /// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
    /// Events are structs, you can define any kind of events you want. This manager comes with MMGameEvents, which are 
    /// basically just made of a string, but you can work with more complex ones if you want.
    /// 
    /// To trigger a new event, from anywhere, do YOUR_EVENT.Trigger(YOUR_PARAMETERS)
    /// So MMGameEvent.Trigger("Save"); for example will trigger a Save MMGameEvent
    /// 
    /// you can also call MMEventManager.TriggerEvent(YOUR_EVENT);
    /// For example : MMEventManager.TriggerEvent(new MMGameEvent("GameStart")); will broadcast an MMGameEvent named GameStart to all listeners.
    ///
    /// To start listening to an event from any class, there are 3 things you must do : 
    ///
    /// 1 - tell that your class implements the MMEventListener interface for that kind of event.
    /// For example: public class GUIManager : Singleton<GUIManager>, MMEventListener<MMGameEvent>
    /// You can have more than one of these (one per event type).
    ///
    /// 2 - On Enable and Disable, respectively start and stop listening to the event :
    /// void OnEnable()
    /// {
    /// 	this.MMEventStartListening<MMGameEvent>();
    /// }
    /// void OnDisable()
    /// {
    /// 	this.MMEventStopListening<MMGameEvent>();
    /// }
    /// 
    /// 3 - Implement the MMEventListener interface for that event. For example :
    /// public void OnMMEvent(MMGameEvent gameEvent)
    /// {
    /// 	if (gameEvent.EventName == "GameOver")
    ///		{
    ///			// DO SOMETHING
    ///		}
    /// } 
    /// will catch all events of type MMGameEvent emitted from anywhere in the game, and do something if it's named GameOver
    /// </summary>
    [ExecuteAlways]
    public static class EventManager
    {
        static Dictionary<Type, List<EventListenerBase>> _subscriberList;

        static EventManager()
        {
            _subscriberList = new Dictionary<Type, List<EventListenerBase>>();
        }

        public static void AddListener<EVENT>(EventListener<EVENT> listener) where EVENT : struct
        {
            Type eventType = typeof(EVENT);

            if (!_subscriberList.ContainsKey(eventType))
            {
                _subscriberList[eventType] = new List<EventListenerBase>();
            }

            if (!SubscriptionExists(eventType, listener))
            {
                _subscriberList[eventType].Add(listener);
            }
        }

        public static void RemoveListener<EVENT>(EventListener<EVENT> listener) where EVENT : struct
        {
            Type eventType = typeof(EVENT);
            
            if (!_subscriberList.ContainsKey(eventType))
            {
                throw new ArgumentException(
                    $"Removing listener \"{listener}\", but the event type \"{eventType}\" isn't registered.");
            }

            List<EventListenerBase> subscriberList = _subscriberList[eventType];
            bool listenerFound = false;
            
            for (int i = subscriberList.Count-1; i >= 0; i--)
            {
                if (subscriberList[i] != listener)
                {
                    continue;
                }

                subscriberList.Remove( subscriberList[i] );
                listenerFound = true;

                if ( subscriberList.Count == 0 )
                {
                    _subscriberList.Remove(eventType);
                }						

                return;
            }
            
            if(!listenerFound)
            {
                Debug.LogWarning($"Removing listener, but the supplied receiver isn't subscribed to event type \"{eventType}\".");
                // throw new ArgumentException(
                //     );
            }
        }

        /// <summary>
        /// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
        /// </summary>
        /// <param name="newEvent">The event to trigger.</param>
        /// <typeparam name="EVENT">The 1st type parameter.</typeparam>
        public static void TriggerEvent<EVENT>(EVENT newEvent) where EVENT : struct
        {
            List<EventListenerBase> receivers;

            if (!_subscriberList.TryGetValue(typeof(EVENT), out receivers))
            {
                return;
            }

            for (int i = receivers.Count - 1; i >= 0; i--)
            {
                (receivers[i] as EventListener<EVENT>).OnEvent(newEvent);
            }
        }

        static bool SubscriptionExists(Type type ,EventListenerBase receiver)
        {
            List<EventListenerBase> receivers;

            if (!_subscriberList.TryGetValue(type, out receivers))
            {
                return false;
            }

            bool exists = false;
            for (int i = receivers.Count-1; i >= 0; i--)
            {
                if ( receivers[i] == receiver )
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
    }

    /// <summary>
    /// Static class that allows any class to start or stop listening to events
    /// </summary>
    public static class EventRegister
    {
        public delegate void Delegate<EVENT>(EVENT eventType);

        public static void StartListeningEvent<EVENT>(this EventListener<EVENT> caller) where EVENT : struct
        {
            EventManager.AddListener(caller);
        }

        public static void StopListeningEvent<EVENT>(this EventListener<EVENT> caller) where EVENT : struct
        {
            EventManager.RemoveListener(caller);
        }
    }
    
    /// <summary>
    /// Event listener basic interface
    /// </summary>
    public interface EventListenerBase {}

    /// <summary>
    /// A public interface you'll need to implement for each type of event you want to listen to.
    /// </summary>
    public interface EventListener<EVENT> : EventListenerBase
    {
        void OnEvent(EVENT e);
    }
}