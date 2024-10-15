using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Manager : MonoBehaviour
{
    public GameObject player_manager;
    string state_to_be;
    public bool doingTask, inHammerRange;
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
        hammerInteraction();
        player_manager.GetComponent<Player_Manager>().set_cur_state(state_to_be);
        player_manager.GetComponent<Player_Manager>().set_cur_task(taskName);
        //print(state_to_be);
    }

    void taskInteraction()
    {
        if (inTaskRange && Input.GetKeyDown(KeyCode.E) && !doingTask)
            startTask();
        if (doingTask && Input.GetKeyDown(KeyCode.Q))
            endTask();
    }

    public void startTask()
    {
        print("Task Initiated)");
        doingTask = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void endTask()
    {
        print("Task Terminated");
        doingTask = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Camera.main.GetComponent<Camera_Controller>().SnapToPlayer();
    }

    void hammerInteraction()
    {
        if (inHammerRange && Input.GetKeyDown(KeyCode.Q))
        {
            print("attemping item pickup");
            GameObject hammer = GameObject.FindWithTag("Hammer");
            GameObject hands = GameObject.FindWithTag("Hands");
            GameObject test = GameObject.FindWithTag("Test");
            hammer.transform.position = hands.transform.position;
            print(hammer.transform.position);
            Vector3 offset = new Vector3(0.3f, -0.03f, -0.02f);
            hammer.transform.position = test.transform.position + offset;
            print(hammer.transform.position);
            //hammer.transform.rotation = Quaternion.Euler((float)-77.138, (float)272.87, (float)23.359);
            hammer.transform.parent = hands.transform;
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
        else if (other.gameObject.tag == "Hammer")
        {
            print("In Range of Hammer");
            inHammerRange = true;
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
