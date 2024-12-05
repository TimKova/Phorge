using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;

public class State_Manager : MonoBehaviour
{
    public float BIG_FACTION_CHANGE = 0.05f;
    public float SMALL_FACTION_CHANGE = 0.1f;

    public GameObject player_manager;
    string state_to_be;
    [SerializeField]
    public bool doingTask, inHammerRange, pause;
    public bool npc;
    public bool inTaskRange;
    bool inNpcRange;
    string taskName;
    public Canvas npcCanvas;
    public Canvas npcCanvas2;
    public Canvas npcCanvas3;
    public string npcName;
    public string npcTag;
    public string stage;
    public Time_Manager tm;
    public NewClockScript clock;
    public GameObject clockDisplay;
    public GameObject NewDay;
    public Button NewDayButton;
    public GameObject questDisplay;
    public GameObject questDisplay2;
    public DialogueScript ds1;
    public DialogueScript ds2;
    [SerializeField] public List<GameObject> NPCs;
    //[SerializeField] public List<Slider> sliders;
    public GameObject forgeMusic;
    public GameObject shopMusic;
    public float thiefRep;
    [SerializeField]
    public float knightRep;
    public float beastRep;
    public float elfRep;
    public string activeFaction1;
    public string activeFaction2;
    public bool inInventory;
    public NavScript navScript1;
    public NavScript navScript2;
    public QuestScript qs1;
    public QuestScript qs2;
    public Player_Inventory pi;
    public Canvas FactionCanvas;
    public Slider KnightsSlider;
    public Slider ThievesSlider;
    public Slider ElvesSlider;
    public Slider BeastsSlider;
    public List<Slider> sliders = new List<Slider>();
    public bool swapQ1toggle;
    public bool swapQ2toggle;
    public Canvas winner;
    public Canvas loser;

    public TMP_Text dayDisplay;


    // Start is called before the first frame update
    void Start()
    {
        swapQ1toggle = true;
        swapQ2toggle = true;
        thiefRep = 0.2f;
        knightRep = 0.2f;
        beastRep = 0.2f;
        elfRep = 0.2f;
        pause = false;
        doingTask = false;
        inInventory = false;
        npc = false;
        state_to_be = "free_move";
        taskName = null;
        stage = "morning";
        stageSwitch();
        hideSliders();
        questDisplay.SetActive(false);
        loser.gameObject.SetActive(false);
        winner.gameObject.SetActive(false);
        //print(FactionCanvas.transform.childCount + " BWAAAAAAAAA");
        sliders.Add(KnightsSlider);
        sliders.Add(ThievesSlider);
        sliders.Add(ElvesSlider);
        sliders.Add(BeastsSlider);
        foreach (Slider slidey in sliders)
            slidey.value = 0.2f;
        StartCoroutine(profileSwapR());

        //StartCoroutine(daytimeroutine());
    }

    // Update is called once per frame
    void Update()
    {
        //NewDayButton.onClick.AddListener(ButtonStage());
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
        else if (inInventory)
        {
            state_to_be = "inventory";
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            FactionCanvas.enabled = !FactionCanvas.enabled;
            print("This many sliders: " + sliders.Count);

        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            sliders[0].value += SMALL_FACTION_CHANGE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            sliders[1].value += BIG_FACTION_CHANGE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            sliders[2].value += SMALL_FACTION_CHANGE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            sliders[3].value += BIG_FACTION_CHANGE;
        }
        if(thiefRep <= 0f || knightRep <= 0f || beastRep <= 0f || elfRep <= 0f)
        {
            loser.gameObject.SetActive(true);
            Time.timeScale = 0;
            StartCoroutine(WinLoseExit());
        }
        if (thiefRep >= 1 || knightRep >= 1 || beastRep >= 1 || elfRep >= 1)
        {
            //print($"T: {thiefRep}, K: {knightRep},T: {beastRep},T: {elfRep},");
            winner.gameObject.SetActive(true);
            Time.timeScale = 0;
            StartCoroutine(WinLoseExit());
        }
        npcInteraction();
        taskInteraction();
        hammerInteraction();
        pauseState();
        eveningButton();
        endNPCQuest();
        //profileSwapTargeted();
        player_manager.GetComponent<Player_Manager>().set_cur_state(state_to_be);
        player_manager.GetComponent<Player_Manager>().set_cur_task(taskName);
        player_manager.GetComponent<Player_Manager>().set_cur_npc(npcName);
        //print("Our NPC's name is " + npcName);
        //print(state_to_be);
    }

    public void ButtonStage()
    {
        clock.internalMinutes = 479;
        stage = "morning";
        stageSwitch();
    }
    public void endNPCQuest()
    {
        if (qs1.readyToLeave == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            npc = false;
            qs1.readyToLeave = 0;
        }
        if (qs2.readyToLeave == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            npc = false;
            qs2.readyToLeave = 0;
        }
    }
    void hideSliders()
    {
        FactionCanvas.enabled = false;
    }

    void showSliders()
    {
        FactionCanvas.enabled = true;
    }

    void taskInteraction()
    {
        if (inTaskRange && Input.GetKeyDown(KeyCode.E) && !doingTask)
            startTask();
        if (Input.GetKeyDown(KeyCode.I) && !doingTask)
        {
            inInventory = !inInventory;
        }
        if ((doingTask || inInventory) && Input.GetKeyDown(KeyCode.Q))
            endTask();
    }

    public void startTask()
    {
        //print("Task Initiated)");
        doingTask = true;
        Cursor.lockState = CursorLockMode.Confined;
        //hideSliders();
    }

    public void endTask()
    {
        //print("Task Terminated");
        doingTask = false;
        if (inInventory)
            inInventory = false;
        Cursor.lockState = CursorLockMode.Locked;
        //showSliders();
        //Camera.main.GetComponent<Camera_Controller>().SnapToPlayer();
    }

    void npcInteraction()
    {
        if (inNpcRange && Input.GetKeyDown(KeyCode.E))
            startNpcInt();
        if (inNpcRange && (Input.GetKeyDown(KeyCode.Q)))
            endNpcInt();
        if (inNpcRange && (npcName == "QuestGiver1" && swapQ1toggle == true))
        {
            endNpcInt();
        }
        if (inNpcRange && (npcName == "QuestGiver2" && swapQ2toggle == true))
        {
            endNpcInt();
        }
    }
    public void startNpcInt()
    {
        //hideSliders();
        //print("Interaction Engaged");
        npc = true;
        if (npcName == "MrItemMan")
        {
            npcCanvas.enabled = false;
        }
        else if (npcName == "QuestGiver1")
        {
            npcCanvas2.enabled = false;
            questDisplay.SetActive(true);
        }
        else if (npcName == "QuestGiver2")
        {
            npcCanvas3.enabled = false;
            questDisplay2.SetActive(true);
        }
        else
        {
            //npcCanvas2.enabled = false;
            //npcCanvas3.enabled = false;
        }
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void endNpcInt()
    {
        //showSliders();
        print("Interaction Terminated");
        if (npcName == "MrItemMan")
        {
            stage = "workday";
            stageSwitch();
            clock.runDay();
        }
        if (npcName == "QuestGiver1")
        {
            questDisplay.SetActive(false);
        }
        else if (npcName == "QuestGiver2")
        {
            questDisplay2.SetActive(false);
        }
        else
        {
            //npcCanvas2.enabled = false;
        }
        npc = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void pauseState()
    {
        if (!pause && Input.GetKeyDown(KeyCode.Escape))
        {
            state_to_be = "pause";
            pause = true;
        }
        else if (pause && Input.GetKeyDown(KeyCode.Escape))
        {
            pause = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }//end pauseState

    private void stageSwitch()
    {
        if (stage == "morning")
        {
            npcCanvas.enabled = true;
            npcCanvas2.enabled = true;
            npcCanvas3.enabled = true;
            //dayDisplay.SetText($"Day {clock.internalDays}");
            profileSwap();


            forgeMusic.GetComponent<AudioSource>().Stop();
            shopMusic.GetComponent<AudioSource>().Play();
            //print("Hello john");
            clockDisplay.SetActive(false);
            NewDay.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (stage == "workday")
        {
            shopMusic.GetComponent<AudioSource>().Stop();
            forgeMusic.GetComponent<AudioSource>().Play();
            clockDisplay.SetActive(true);
        }
        else if (stage == "evening")
        {
            forgeMusic.GetComponent<AudioSource>().Stop();
            shopMusic.GetComponent<AudioSource>().Play();
            clockDisplay.SetActive(false);
            NewDay.SetActive(true);
            //if (navScript1.atCounter1)
            //{
            //    qs1.sendAway(1);
            //}
            //else if(navScript2.atCounter2) {
            //    qs2.sendAway(2);
            //}
        }
    }

    private void profileSwap()
    {
        for (int i = 0; i < NPCs.Count; i++)
        {
            //print("We do be in the for loop");
            Vector3 AdjustedPos = NPCs[i].transform.position;
            // NPC mesh's origin are at their feet, while the NPC capsules themselves seem to have the origin at the center of their body. so i moved them down a little
            AdjustedPos.y -= 1.005f;
            int randomInt = Random.Range(0, 3);
            // if you don't transform the position to align with the npc the moment they are set active, they seem to reinstantiate right where
            // the npc was first created, but they still would follow the npc so they get swung around lmao

            // do NOT remove the filler objects under each ambience NPC, this will cause a buffer overflow in the GetChild functions!
            // do NOT create game objects that are above the 6 children, the following 4 if statements will disable them!
            NPCs[i].transform.GetChild(3).gameObject.SetActive(false);
            NPCs[i].transform.GetChild(4).gameObject.SetActive(false);
            NPCs[i].transform.GetChild(5).gameObject.SetActive(false);
            NPCs[i].transform.GetChild(6).gameObject.SetActive(false);

            if (randomInt == 0)
            {
                //print("Suh");
                NPCs[i].transform.GetChild(3).gameObject.SetActive(true);
                NPCs[i].transform.GetChild(3).gameObject.transform.position = AdjustedPos;
                NPCs[i].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(6).gameObject.SetActive(false);
                if (i == 1)
                { activeFaction1 = "beast"; }
                if (i == 2)
                {activeFaction2 = "beast";}
            }
            else if (randomInt == 1)
            {
                //print("Suh2");
                NPCs[i].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(4).gameObject.SetActive(true);
                NPCs[i].transform.GetChild(4).gameObject.transform.position = AdjustedPos;
                NPCs[i].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(6).gameObject.SetActive(false);
                if (i == 1)
                { activeFaction1 = "elf"; }
                if (i == 2)
                { activeFaction2 = "elf"; }
            }
            else if (randomInt == 2)
            {
                //print("Suh3");
                NPCs[i].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(5).gameObject.SetActive(true);
                NPCs[i].transform.GetChild(5).gameObject.transform.position = AdjustedPos;
                NPCs[i].transform.GetChild(6).gameObject.SetActive(false);
                if (i == 1)
                { activeFaction1 = "knight"; }
                if (i == 2)
                { activeFaction2 = "knight"; }
            }
            else if (randomInt == 3)
            {
                //print("Suh4");
                NPCs[i].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[i].transform.GetChild(6).gameObject.SetActive(true);
                NPCs[i].transform.GetChild(6).gameObject.transform.position = AdjustedPos;
                if (i == 1)
                { activeFaction1 = "thief"; }
                if (i == 2)
                { activeFaction2 = "thief"; }
            }
        }
    }

    private void profileSwapTargeted()
    {
        if (navScript1.q1trigger == 1 && swapQ1toggle == true)
        {
            npcCanvas2.enabled = true;
            ds1.hasRun1 = false;
            print("Wir sind in die erste if");
            swapQ1toggle = false;
            //navScript1.q1trigger = 0;
            Vector3 AdjustedPos = NPCs[1].transform.position;
            AdjustedPos.y -= 1.005f;
            int randomInt = Random.Range(0, 3);
            NPCs[1].transform.GetChild(3).gameObject.SetActive(false);
            NPCs[1].transform.GetChild(4).gameObject.SetActive(false);
            NPCs[1].transform.GetChild(5).gameObject.SetActive(false);
            NPCs[1].transform.GetChild(6).gameObject.SetActive(false);
            if (randomInt == 0)
            {
                NPCs[1].transform.GetChild(3).gameObject.SetActive(true);
                NPCs[1].transform.GetChild(3).gameObject.transform.position = AdjustedPos;
                NPCs[1].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(6).gameObject.SetActive(false);
                activeFaction1 = "beast";
            }
            else if (randomInt == 1)
            {
                NPCs[1].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(4).gameObject.SetActive(true);
                NPCs[1].transform.GetChild(4).gameObject.transform.position = AdjustedPos;
                NPCs[1].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(6).gameObject.SetActive(false);
                activeFaction1 = "elf";
            }
            else if (randomInt == 2)
            {
                NPCs[1].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(5).gameObject.SetActive(true);
                NPCs[1].transform.GetChild(5).gameObject.transform.position = AdjustedPos;
                NPCs[1].transform.GetChild(6).gameObject.SetActive(false);
                activeFaction1 = "knight";
            }
            else if (randomInt == 3)
            {
                NPCs[1].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[1].transform.GetChild(6).gameObject.SetActive(true);
                NPCs[1].transform.GetChild(6).gameObject.transform.position = AdjustedPos;
                activeFaction1 = "thief";
            }
        }
        if (navScript2.q2trigger == 1 && swapQ2toggle == true)
        {
            npcCanvas3.enabled = true;
            ds2.hasRun2 = false;
            print("Wir sind in die zweite if");
            swapQ2toggle = false;
            //navScript2.q2trigger = 0;
            Vector3 AdjustedPos = NPCs[2].transform.position;
            AdjustedPos.y -= 1.005f;
            int randomInt = Random.Range(0, 3);
            NPCs[2].transform.GetChild(3).gameObject.SetActive(false);
            NPCs[2].transform.GetChild(4).gameObject.SetActive(false);
            NPCs[2].transform.GetChild(5).gameObject.SetActive(false);
            NPCs[2].transform.GetChild(6).gameObject.SetActive(false);
            if (randomInt == 0)
            {
                //print("Suh");
                NPCs[2].transform.GetChild(3).gameObject.SetActive(true);
                NPCs[2].transform.GetChild(3).gameObject.transform.position = AdjustedPos;
                NPCs[2].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(6).gameObject.SetActive(false);
                activeFaction2 = "beast";
            }
            else if (randomInt == 1)
            {
                //print("Suh2");
                NPCs[2].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(4).gameObject.SetActive(true);
                NPCs[2].transform.GetChild(4).gameObject.transform.position = AdjustedPos;
                NPCs[2].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(6).gameObject.SetActive(false);
                activeFaction2 = "elf";
            }
            else if (randomInt == 2)
            {
                //print("Suh3");
                NPCs[2].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(5).gameObject.SetActive(true);
                NPCs[2].transform.GetChild(5).gameObject.transform.position = AdjustedPos;
                NPCs[2].transform.GetChild(6).gameObject.SetActive(false);
                activeFaction2 = "knight";
            }
            else if (randomInt == 3)
            {
                //print("Suh4");
                NPCs[2].transform.GetChild(3).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(4).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(5).gameObject.SetActive(false);
                NPCs[2].transform.GetChild(6).gameObject.SetActive(true);
                NPCs[2].transform.GetChild(6).gameObject.transform.position = AdjustedPos;
                activeFaction2 = "thief";
            }
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
        if (other.gameObject.tag == "Task" && stage == "workday") //on the object you want to pick up set the tag to be anything, in this case "object"
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
            npcTag = other.tag;

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
        while (true)
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
    private IEnumerator stateExit()
    {
        yield return new WaitForSeconds(3);
        Application.Quit();
    }

    private void gameExit()
    {
        //yield return new WaitForSeconds(3);
        Application.Quit();
    }

    private IEnumerator WinLoseExit()
    {
        yield return new WaitForSecondsRealtime(4);
        Application.Quit();
    }

    private IEnumerator profileSwapR()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            profileSwapTargeted();
        }
    }
}
