using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    // Start is called before the first frame update
    void Start()
    {
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
        if ((sm.npcName == "QuestGiver1" || sm.npcName == "QuestGiver2") && sm.npc == true);
        {
            //print("We did get in, but nothing happened");
            //print(dialogueOptions[0]);
            display.SetText(dialogueOptions[0]);
            displayCanvas.SetActive(true);
        }

    }
}
