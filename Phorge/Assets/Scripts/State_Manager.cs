using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Manager : MonoBehaviour
{
    public GameObject player_manager;
    string state_to_be;
    public bool task;
    bool npc;
    
    // Start is called before the first frame update
    void Start()
    {
        task = false;
        npc = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (task)
        {
            state_to_be = "task_int";
        }
        else if (npc)
        {
            state_to_be = "npc_int";
        }
        else 
        {
            state_to_be = "free_move";
        }//end if-else
        player_manager.GetComponent<Player_Manager>().set_cur_state(state_to_be);
    }

    private void OnTriggerEnter(Collider other) // to see when the player enters the collider
    {
        if (other.gameObject.tag == "Task") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            task = true;
            npc = false;
        }
        else if(other.gameObject.tag == "NPC")
        {
            npc = true;
            task = false;
        }//end if-else
        
    }
}
