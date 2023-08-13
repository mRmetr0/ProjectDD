using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class EventBus<T> where T : UnityEvent
    {
        public static event Action<T> OnEvent;
        public static void Subscribe(Action<T> handler)
        {
            OnEvent += handler;
        }
        public static void Unsubscribe(Action<T> handler)
        {
            OnEvent -= handler;
        }

        public static void Publish(T pEvent)
        {
            OnEvent?.Invoke(pEvent);
        }
    }
    public sealed class OnShowEvent : UnityEvent
    {
        public Transform CamPos;
        public EventData Data;

        public OnShowEvent(EventTile pEventTile)
        {
            CamPos = pEventTile.CamPos;
            Data = pEventTile.eventData;
        }
    }
}
