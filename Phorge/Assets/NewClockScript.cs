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
    // Start is called before the first frame update
    
    void Start()
    {
        tm = FindObjectOfType<Time_Manager>();
        display = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_24Hour)
        {
            display.SetText(tm.Clock24Hour());
        }
        else
        {
            display.SetText(tm.Clock12Hour());
        }
    }
}
