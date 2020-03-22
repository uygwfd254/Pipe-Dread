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
    //[System.Serializable]
    //public class Event : UnityEvent<System.Object> { }
    private Dictionary <string, Func<System.Object, System.Object>> eventDictionary;

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
            eventDictionary = new Dictionary<string, Func<System.Object, System.Object>>();
        }
    }

    public static void StartListening (string eventName, Func<System.Object, System.Object> listener)
    {
        Func<System.Object, System.Object> thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent += listener;
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            instance.eventDictionary.Add (eventName, thisEvent);
        }
    }

    public static void StopListening (string eventName, Func<System.Object, System.Object> listener)
    {
        if (eventManager == null) return;
        Func<System.Object, System.Object> thisEvent = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent -= listener;
            instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static System.Object TriggerEvent (string eventName, System.Object parameter=null)
    {
        Func<System.Object, System.Object> thisEvent = null;
        System.Object output = null;
        if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            output = thisEvent.Invoke(parameter);
        }

        return output;
    }

}