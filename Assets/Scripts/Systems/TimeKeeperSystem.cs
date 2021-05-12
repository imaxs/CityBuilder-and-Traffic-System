using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeKeeperSystem : MonoBehaviour
{
    public enum DayPeriod
    {
        Day,
        Night
    }

    //
    // Public Constants
    //
    public static int SECONDS_PER_MINUTE = 60;
    public static int MINUTES_PER_HOUR = 60;
    public static int SECONDS_PER_HOUR = 3600;
    public static int SECONDS_PER_DAY = 24 * 3600;
    public static int TIME_MIDNIGHT = 0;
    public static int TIME_MIDDAY = 12 * 3600;

    //
    // Configurable Parameters
    //
    public float gameSecondsPerLifeSeconds = 60;
    [Range(0.02f, 0.25f)] 
    public float timeDilation = 3.0f;

    //
    // Events
    //
    public event Action OnEventMinutesChanged;
    public event Action OnEventHoursChanged;
    public event Action OnEventSunset;
    public event Action OnEventSunrise;

    //
    // Internal Variables
    //
    private int m_Minutes;
    private int m_Hours; 
    private int m_DaysSinceStart = 0;
    private int m_currentDayTime = 12 * SECONDS_PER_HOUR;
    private DayPeriod m_DayPeriod = DayPeriod.Day;

    private void FixedUpdate()
    {
        //
        // Update Current DayTime
        //
        m_currentDayTime += (int)(gameSecondsPerLifeSeconds * Time.deltaTime * timeDilation);
        if (m_currentDayTime > SECONDS_PER_DAY)
        {
            m_DaysSinceStart++;
            m_currentDayTime = 0;
        }

        //
        // Execute Events and Update Day Period
        //
        int minutes = (m_currentDayTime % SECONDS_PER_HOUR) / SECONDS_PER_MINUTE;
        if (m_Minutes != minutes){
            m_Minutes = minutes;
            if (OnEventMinutesChanged != null)
                OnEventMinutesChanged.Invoke();

            int hours = m_currentDayTime / TimeKeeperSystem.SECONDS_PER_HOUR;
            if (m_Hours != hours){
                m_Hours = hours;
                if (OnEventHoursChanged != null)
                    OnEventHoursChanged.Invoke();

                // Day Period
                if (OnEventSunrise != null && m_DayPeriod == DayPeriod.Night){
                    if (m_currentDayTime > (6 * SECONDS_PER_HOUR) && m_currentDayTime < (20 * SECONDS_PER_HOUR))
                    {
                        m_DayPeriod = DayPeriod.Day;
                        OnEventSunrise.Invoke();
                    }
                }
                if (OnEventSunset != null && m_DayPeriod == DayPeriod.Day){
                    if (m_currentDayTime > (20 * SECONDS_PER_HOUR))
                    {
                        m_DayPeriod = DayPeriod.Night;
                        OnEventSunset.Invoke();
                    }
                }
            }
        }
    }

    public int GetMinutes(){
        return m_Minutes;
    }

    public int GetHours(){
        return m_Hours;
    }

    public int GetCurrentDayTime()
    {
        return m_currentDayTime;
    }

    public DayPeriod Getm_DayPeriod()
    {
        return m_DayPeriod;
    }
}
