using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    [Header("Attributes")]
    public GameObject player_model;
    string[] states = {"free_move","task_int","npc_int"};
    public GameObject camera;

    string cur_state;

    public void set_cur_state(string new_state)
    {
        cur_state = new_state;

    }//end set_cur_state

    public string get_cur_state()
    {
        return cur_state;//needs to be dynamic later. For now this helps with testing.
    }//end get_cur_state
}
