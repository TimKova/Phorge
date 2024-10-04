using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Manager : MonoBehaviour
{
    public GameObject player_manager;
    string state_to_be;
    public bool doingTask;
    bool npc;
    public bool inTaskRange;
    bool inNpcRange;
    string taskName;

    // Start is called before the first frame update
    void Start()
    {
        doingTask = false;
        npc = false;
        state_to_be = "free_move";
        taskName = null;
    }

    // Update is called once per frame
    void Update()
    {

        if (doingTask)
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
        }

        taskInteraction();
        player_manager.GetComponent<Player_Manager>().set_cur_state(state_to_be);
        player_manager.GetComponent<Player_Manager>().set_cur_task(taskName);
        //print(state_to_be);
    }

    void taskInteraction()
    {
        if (inTaskRange && Input.GetKeyDown(KeyCode.E))
        {
            print("Task Initiated)");
            doingTask = true;
        }

        if (doingTask && Input.GetKeyDown(KeyCode.Escape))
        {
            print("Task Terminated");
            doingTask = false;
        }
    }

    private void OnTriggerEnter(Collider other) // to see when the player enters the collider
    {
        //print("Trigger Enter");
        if (other.gameObject.tag == "Task") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            inTaskRange = true;
            taskName = other.name;
            print("In Task Range of " + taskName);
        }
        else if (other.gameObject.tag == "NPC")
        {
            inNpcRange = true;
        }//end if-else

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Task") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            print("Out of Task Range");
            inTaskRange = false;

        }
        else if (other.gameObject.tag == "NPC")
        {
            inNpcRange = false;
        }
        else if (other.gameObject.tag == "Door")
        {
            print("Love is an open door");
            other.gameObject.transform.Rotate(0f, 90f, 0f, Space.Self);
        }//end if-else
    }
}
