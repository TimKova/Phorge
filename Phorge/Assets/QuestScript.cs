using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestScript : MonoBehaviour
{
    public Player_Inventory pi;
    public DialogueScript ds;
    public NavScript nav;
    public int readyToLeave;
    // Start is called before the first frame update
    void Start()
    {
        readyToLeave = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (Weapon weppy in weapons)
        //{
        //    if (weppy.INSERTFIELDHERE == ds.itemName && weppy.INSERTFIELDHERE == ds.itemMetal)
        //    {
        //        //DECREMENT WEPPY

        //    }
        //}

    }
    public void updateVars()
    {
        nav.readyToLeaveLocal = 1;
        readyToLeave = 1;
        print("Bugtest");
    }

}
