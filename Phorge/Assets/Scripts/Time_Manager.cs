using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Time_Manager : MonoBehaviour
{
    public const int hoursInDay = 24, minutesInHour = 60;
    public float dayDuration = 60f;

    float totalTime = 0;
    float currentTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        currentTime = totalTime % dayDuration;
    }

    public float GetHour()
    {
        return currentTime * hoursInDay / dayDuration;
    }

    public float GetMinutes()
    {
        return (currentTime * hoursInDay * minutesInHour / dayDuration) % minutesInHour; 
    }

    public string Clock24Hour()
    {
        int hour = Mathf.FloorToInt(GetHour());
        int minute = Mathf.FloorToInt(GetMinutes());
        if (minute < 10)
        {
            return hour + ":0" + minute;
        }
        else
        {
            return hour + ":" + minute;
        }
    }
    public string Clock12Hour()
    {
        int hour = Mathf.FloorToInt(GetHour());
        int minute = Mathf.FloorToInt(GetMinutes());
        string abbrev = "AM";

        if (hour >= 12)
        {
            abbrev = "PM";
            hour -= 12;
        }
        if (hour == 0)
        {
            hour = 12;
        }
        if (minute < 10)
        {
            return hour + ":0" + minute + " " + abbrev;
        }
        else
        {
            return hour + ":" + Mathf.FloorToInt(GetMinutes()) + " " + abbrev;
        }
    }
}
