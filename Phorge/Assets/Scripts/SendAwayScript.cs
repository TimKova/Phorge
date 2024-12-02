using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendAwayScript : MonoBehaviour
{
    public Player_Inventory pi;
    public DialogueScript ds;
    public NavScript nav;

    public int readyToLeave;
    public State_Manager sm;
    // Start is called before the first frame update
    void Start()
    {
        readyToLeave = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void updateVarsSend()
    {
        nav.readyToLeaveLocal = 1;
        if (nav.gameObject.name == "QuestGiver1")
        {
            sm.swapQ1toggle = true;
        }
        if (nav.gameObject.name == "QuestGiver2")
        {
            sm.swapQ2toggle = true;
        }
        print("Bugtest");
    }
}