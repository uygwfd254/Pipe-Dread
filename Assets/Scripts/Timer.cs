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
    void startTimer() {
        m_hasTimerStarted = true;
    }

    void stopTimer() {
        m_hasTimerStarted = false;
    }

    void resetTimer() {
        if (m_hasTimerStarted)
            stopTimer();
        
        m_seconds = 0f;
    }

    // setters and getters
    int getSecond() {
        return (int)Math.Floor((double)m_seconds);
    }

    void setTargetTime(int targetSeconds) {
        m_targetSeconds = targetSeconds;
    }

    bool hasReachedTarget() {
        return m_hasReachedTarget;
    }
}
