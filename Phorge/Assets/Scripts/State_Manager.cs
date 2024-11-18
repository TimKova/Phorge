using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class State_Manager : MonoBehaviour
{
    public GameObject player_manager;
    string state_to_be;
    public bool doingTask, inHammerRange, pause;
    public bool npc;
    public bool inTaskRange;
    bool inNpcRange;
    string taskName;
    public Canvas npcCanvas;
    public Canvas npcCanvas2;
    public string npcName;
    public string stage;
    public Time_Manager tm;
    public NewClockScript clock;
    public GameObject clockDisplay;
    public GameObject NewDay;
    public Button NewDayButton;
    public GameObject questDisplay;
    // Start is called before the first frame update
    void Start()
    {
        pause = false;
        doingTask = false;
        npc = false;
        state_to_be = "free_move";
        taskName = null;
        stage = "morning";
        stageSwitch();
        questDisplay.SetActive(false);
        //StartCoroutine(daytimeroutine());
    }

    // Update is called once per frame
    void Update()
    {
        NewDayButton.onClick.AddListener(ButtonStage);
        if (doingTask)
        {
            state_to_be = "task_int";
        }
        else if (npc)
        {
            state_to_be = "npc_int";
        }
        else if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            state_to_be = "free_move";
        }

        if (clock.internalMinutes > 1200)
        {
            stage = "evening";
            stageSwitch();
        }

        npcInteraction();
        taskInteraction();
        hammerInteraction();
        pauseState();
        eveningButton();
        player_manager.GetComponent<Player_Manager>().set_cur_state(state_to_be);
        player_manager.GetComponent<Player_Manager>().set_cur_task(taskName);
        player_manager.GetComponent<Player_Manager>().set_cur_npc(npcName);
        //print("Our NPC's name is " + npcName);
        //print(state_to_be);
    }

    void ButtonStage()
    {
        clock.internalMinutes = 479;
        stage = "morning";
        stageSwitch();
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
        //print("Task Initiated)");
        doingTask = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void endTask()
    {
        //print("Task Terminated");
        doingTask = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Camera.main.GetComponent<Camera_Controller>().SnapToPlayer();
    }

    void npcInteraction()
    {
        if (inNpcRange && Input.GetKeyDown(KeyCode.E))
            startNpcInt();
        if (inNpcRange && Input.GetKeyDown(KeyCode.Q))
            endNpcInt();
    }
    public void startNpcInt()
    {
        //print("Interaction Engaged");
        npc = true;
        if (npcName == "MrItemMan")
        {
            npcCanvas.enabled = false;
        }
        else if (npcName == "QuestGiver1")
        {
            questDisplay.SetActive(true);
        }
        else
        {
            npcCanvas2.enabled = false;
        }
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void endNpcInt()
    {
        //print("Interaction Terminated");
        if (npcName == "MrItemMan")
        {
            stage = "workday";
            stageSwitch();
        }
        if (npcName == "QuestGiver1")
        {
            questDisplay.SetActive(false);
        }
        npc = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void pauseState()
    {
        if (!pause && Input.GetKeyDown(KeyCode.Escape)){
            state_to_be = "pause";
            pause = true;
        }else if(pause && Input.GetKeyDown(KeyCode.Escape))
        {
            pause = false;
            Time.timeScale = 1;
        }

    }//end pauseState
    
    private void stageSwitch()
    {
        if (stage == "morning")
        {
            print("Hello john");
            clockDisplay.SetActive(false);
            NewDay.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (stage == "workday")
        {
            clockDisplay.SetActive(true);
        }
        else if (stage == "evening")
        {
            clockDisplay.SetActive(false);
            NewDay.SetActive(true);
        }
    }

    private void eveningButton()
    {
        if (stage == "evening" && Input.GetKeyDown(KeyCode.T))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    void hammerInteraction()
    {
        if (inHammerRange && Input.GetKeyDown(KeyCode.Q))
        {
            //print("attemping item pickup");
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
            //print("In Task Range of " + taskName);
        }
        else if (other.gameObject.tag == "Hammer")
        {
            //print("In Range of Hammer");
            inHammerRange = true;
        }
        else if (other.gameObject.tag == "NPC")
        {
            //print("In range of NPC");
            npcName = other.name;

            inNpcRange = true;
        }//end if-else

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Task") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            //print("Out of Task Range");
            inTaskRange = false;

        }
        else if (other.gameObject.tag == "NPC")
        {
            inNpcRange = false;
        }
        else if (other.gameObject.tag == "Door")
        {

            //print("Love is an open door");
            other.gameObject.transform.Rotate(0f, 90f, 0f, Space.Self);
        }//end if-else
    }

    // This function is DANGEROUS. May he who embarks upon its path beware
    private IEnumerator daytimeroutine()
    {
        while(true)
        {
            print("Definitely inside the coroutine");
           if (stage == "morning")
           {
                print("We here");
                //clockDisplay.SetText("");
           }
           if (stage == "workday")
           {

           }
           if (stage == "evening")
           {

           }
        }
    }
}
