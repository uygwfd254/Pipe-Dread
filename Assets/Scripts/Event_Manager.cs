using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// got from https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa
// and https://stackoverflow.com/questions/42034245/unity-eventmanager-with-delegate-instead-of-unityevent/42034899#42034899
// and https://gamedev.stackexchange.com/questions/137851/unity-events-with-arguments
public class Event_Manager : MonoBehaviour
{
    [System.Serializable]
    public class Event : UnityEvent<Event_Params> { }
    private Dictionary <string, Event> eventDictionary;

    private static Event_Manager eventManager;

    public static Event_Manager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType (typeof (Event_Manager)) as Event_Manager;

                if (!eventManager)
                {
                    Debug.Log("There needs to be one active Event_Manager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init ();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Event>();
        }
    }

    public static void StartListening (string eventName, UnityAction<Event_Params> listener)
    {
        Event thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
            // instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent = new Event();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add (eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, UnityAction<Event_Params> listener)
    {
        if (eventManager == null) return;
        Event thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
            // instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent (string eventName,Event_Params parameter)
    {
        Event thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke(parameter);
        }
    }
}

public struct Event_Params {
    public Pipe pipe_param;
    public bool bool_param;
}