using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    [Header("Attributes")]
    public GameObject player_model;
    public GameObject camera;
    string[] states = {"free_move","task_int","npc_int"};
    public string cur_task;
    public string cur_state;
   
    public void set_cur_task(string taskName)
    {
        cur_task = taskName;
    }

    public string get_cur_task() 
    { 
        return cur_task; 
    }

    public void set_cur_state(string new_state)
    {
        cur_state = new_state;

    }//end set_cur_state

    public string get_cur_state()
    {
        //print("GETTING STATE HERE______________________________________");
        return cur_state;//needs to be dynamic later. For now this helps with testing.
    }//end get_cur_state

}
