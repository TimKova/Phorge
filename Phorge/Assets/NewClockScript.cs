using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using TMPro;

public class NewClockScript : MonoBehaviour
{
    Time_Manager tm;
    public TMP_Text display;
    public bool _24Hour = false;

    const int MINUTES_PER_HOUR = 60;
    const float DAY_SPEED_SCALE = 3f;
    const int DAY_DURATION = 48;
    const bool DO_DAY_CYCLE = true;

    public Light MainLight;

    public Material MorningSky;
    public Material DaySky;
    public Material AfternoonSky;
    public Material EveningSky;
    public Material NightSky;
    public int internalMinutes;
    public int internalHours;

    // Start is called before the first frame update

    void Start()
    {
        tm = FindObjectOfType<Time_Manager>();
        display = GetComponent<TMP_Text>();
        if (DO_DAY_CYCLE)
            StartCoroutine(DayTimer(DAY_DURATION));
    }

    // Update is called once per frame
    void Update()
    {
        if (internalMinutes == 479)
        {
            StartCoroutine(DayTimer(DAY_DURATION));
        }
    }

    IEnumerator DayTimer(int hourDuration)
    {
        for (internalMinutes = (19*MINUTES_PER_HOUR); internalMinutes < hourDuration * 60; internalMinutes++)
        {
            //print(internalMinutes);
            yield return new WaitForSeconds(1f / DAY_SPEED_SCALE);
            int displayMinutes = internalMinutes % MINUTES_PER_HOUR;
            int displayHours = internalMinutes / MINUTES_PER_HOUR % 24;
            internalHours = internalHours;
            display.SetText($"{GetNormalHour(displayHours).ToString("D2")}:{displayMinutes.ToString("D2")}");
            switch (displayHours)
            {
                case 0:
                    RenderSettings.skybox = NightSky;
                    DynamicGI.UpdateEnvironment();
                    MainLight.intensity = 0.2f;
                    break;
                case 5:
                    RenderSettings.skybox = MorningSky;
                    DynamicGI.UpdateEnvironment();
                    MainLight.intensity = 0.55f;
                    break;
                case 9:
                    RenderSettings.skybox = DaySky;
                    DynamicGI.UpdateEnvironment();
                    MainLight.intensity = 1f;
                    break;
                case 14:
                    RenderSettings.skybox = AfternoonSky;
                    DynamicGI.UpdateEnvironment();
                    MainLight.intensity = 0.8f;
                    break;
                case 18:
                    RenderSettings.skybox = EveningSky;
                    DynamicGI.UpdateEnvironment();
                    MainLight.intensity = 0.65f;
                    break;
                case 21:
                    RenderSettings.skybox = NightSky;
                    DynamicGI.UpdateEnvironment();
                    MainLight.intensity = 0.2f;
                    break;
            }

        }
    }

    int GetNormalHour(int militaryHour)
    {
        return ((militaryHour + 11) % 12 + 1);
    }
}

