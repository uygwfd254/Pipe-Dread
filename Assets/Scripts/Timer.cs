using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float m_seconds;
    private int m_targetSeconds;

    private bool m_hasTimerStarted;
    private bool m_hasReachedTarget;

    // set up listeners
    private Func<System.Object, System.Object> TimeStartListener;
    private Func<System.Object, System.Object> TimeStopListener;
    private Func<System.Object, System.Object> GetTimeListener;

    void Awake() {
        TimeStartListener = new Func<System.Object, System.Object>(startTimer);
        TimeStopListener = new Func<System.Object, System.Object>(stopTimer);
        GetTimeListener = new Func<System.Object, System.Object>(getSecond);
    }

    void OnEnable()
    {
        Event_Manager.StartListening("start_timer", TimeStartListener);
        Event_Manager.StartListening("stop_timer", TimeStopListener);
        Event_Manager.StartListening("get_time", GetTimeListener);
    }

    void OnDisable()
    {
        Event_Manager.StopListening("start_timer", TimeStartListener);
        Event_Manager.StopListening("stop_timer", TimeStopListener);
        Event_Manager.StopListening("get_time", GetTimeListener);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_seconds = 0f;
        m_targetSeconds = 0;
        m_hasTimerStarted = false;
        m_hasReachedTarget = true;
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
    }

    void updateTimer() {
        if (m_hasTimerStarted)
            m_seconds += Time.deltaTime;
        
        if (m_seconds >= m_targetSeconds)
            m_hasReachedTarget = true;
        else
            m_hasReachedTarget = false;
    }

    // timer functions
    System.Object startTimer(System.Object p) {
        m_hasTimerStarted = true;
        return null;
    }

    System.Object stopTimer(System.Object p) {
        m_hasTimerStarted = false;
        return null;
    }

    void resetTimer() {
        if (m_hasTimerStarted)
            stopTimer(null);
        
        m_seconds = 0f;
    }

    // setters and getters
    System.Object getSecond(System.Object p) {
        return (int)Math.Floor((double)m_seconds);
    }

    void setTargetTime(int targetSeconds) {
        m_targetSeconds = targetSeconds;
    }

    bool hasReachedTarget() {
        return m_hasReachedTarget;
    }
}
