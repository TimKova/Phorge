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
    public string itemQuality;
    public State_Manager sm;
    //TODO: Add lists containing all possible options for each category
    public TMP_Text display;
    public GameObject displayCanvas;
    public bool hasRun1;
    public bool hasRun2;
    public Player_Inventory pi;
    public Inventory_Item inv;
    public static float[] qualityValues = { 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f };
    public static string[] qualityNames = { "Poor", "Basic", "Good", "Fine", "Flawless", "Supernatural"};
    private int numMaterials = Player_Inventory.numMaterials;
    private static int maxQuality = qualityNames.Length -1;
    private static int minQuality = 0;
    private static int maxMaterial = Player_Inventory.materialNames.Length;
    string[] materialNames = Player_Inventory.materialNames;
    string[] weaponNames = Player_Inventory.weaponNames;
    public Weapon currentRequest;

    // Start is called before the first frame update
    void Start()
    {
        hasRun1 = false;
        hasRun2 = false;
        display = GetComponent<TMP_Text>();
        sm = FindObjectOfType<State_Manager>();
        //itemName = "sword";
        //itemMetal = "uranium";
        //itemFinish = "oil";
        currentRequest = MakeRequest();
        dialogueOptions.Add("I would like to purchase a " + currentRequest.getMaterial() + " " + currentRequest.getName() + " of at least " + itemQuality + " quality.");
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
            currentRequest = MakeRequest();
            print(itemMetal);
            display.SetText("I would like to purchase a " + currentRequest.getMaterial() + " "+currentRequest.getName()+" of at least " + itemQuality+ " quality.");
            displayCanvas.SetActive(true);
            hasRun1 = true;
        }
        if ((sm.npcName == "QuestGiver2") && (sm.npc == true) && (hasRun2 == false))
        {
            print("hasRun2: " + hasRun2);
            //print(dialogueOptions[0]);
            currentRequest = MakeRequest();
            print(itemName);
            print(itemMetal);
            display.SetText("I would like to purchase a " + currentRequest.getMaterial() + " " + currentRequest.getName() + " of at least " + itemQuality + " quality.");
            displayCanvas.SetActive(true);
            hasRun2 = true;
        }

    }

    private Weapon MakeRequest()
    {
        var tempMetal = Random.Range(0, maxMaterial);
        itemName = Player_Inventory.weaponNames[Random.Range(0, Player_Inventory.weaponNames.Length)];
        itemMetal = Player_Inventory.materialNames[tempMetal];
        var tempIndex = Random.Range(0, maxQuality);
        itemQuality = qualityNames[tempIndex];
        return (new Weapon(qualityValues[tempIndex], itemMetal, itemName, 1, ((tempMetal + 1)*2 * (0.75f + qualityValues[tempIndex]))));
    }
}
