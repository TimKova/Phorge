using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class DialogueScript : MonoBehaviour
{
    public List<string> dialogueOptions = new List<string>();
    public string itemName;
    public string itemMetal;
    public string itemFinish;
    public State_Manager sm;
    //TODO: Add lists containing all possible options for each category
    public TMP_Text display;
    public GameObject displayCanvas;
    public bool hasRun1;
    public bool hasRun2;
    //public Player_Inventory pi;
    // Start is called before the first frame update
    void Start()
    {
        hasRun1 = false;
        hasRun2 = false;
        display = GetComponent<TMP_Text>();
        sm = FindObjectOfType<State_Manager>();
        itemName = "sword";
        itemMetal = "uranium";
        itemFinish = "oil";
        dialogueOptions.Add("I would like to purchase a " + itemName + " made of " + itemMetal + ".");
        display.SetText(dialogueOptions[0]);
        displayCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (sm.npcName == "QuestGiver1" && sm.npc == true && (hasRun1 == false))
        {
            print("hasRun1: " + hasRun1);
            hasRun1 = true;
            print("hasRun1: " + hasRun1);
            //print(dialogueOptions[0]);
            itemName = Player_Inventory.weaponNames[Random.Range(0, Player_Inventory.weaponNames.Length)];
            itemMetal = Player_Inventory.materialNames[Random.Range(0, Player_Inventory.materialNames.Length)];
            print(itemName);
            print(itemMetal);
            display.SetText("I would like to purchase a " + itemName + " made of " + itemMetal + ".");
            displayCanvas.SetActive(true);
            hasRun1 = true;
        }
        if ((sm.npcName == "QuestGiver2") && (sm.npc == true) && (hasRun2 == false))
        {
            print("hasRun2: " + hasRun2);
            //print(dialogueOptions[0]);
            itemName = Player_Inventory.weaponNames[Random.Range(0, Player_Inventory.weaponNames.Length)];
            itemMetal = Player_Inventory.materialNames[Random.Range(0, Player_Inventory.materialNames.Length)];
            print(itemName);
            print(itemMetal);
            display.SetText("I would like to purchase a " + itemName + " made of " + itemMetal + ".");
            displayCanvas.SetActive(true);
            hasRun2 = true;
        }

    }
}
