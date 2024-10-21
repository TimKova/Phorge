using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;



public class AnvilGame : MonoBehaviour
{
    //---------------------------------------------------------------------------------------------
    // Constants
    //---------------------------------------------------------------------------------------------

    // Positions
    private readonly Vector3 ANVIL_TOP = new Vector3(-15.321f, 0.9684f, -11.472f);  // World position (I think) for top of anvil
    public readonly Vector3 HIT_ICON_START_POS = new Vector3(-1436f, -456f, 61f);   // Local position for where hit icons start relative to rhythm game canvas
    public readonly Vector3 HIT_ICON_END_POS = new Vector3(1460f, -456f, 61f);      // Same as above, but for end
    //public const float HIT_ICON_CHECK_X = 0f;                                     // UNUSED
    //public const float HIT_ICON_DONE_X = 1460f;                                   // UNUSED


    // Scalars
    const float INGOT_SCALE = 2.5f;                     // Scale factor for ingots, applied to all 3-dims

    // Margin for acceptable error in rhythm game. Represents % of bar length.
    // Current value gives 1/8 or 12.5% wiggle room to both sides of center
    public const float forgivenessWindow = 0.125f;

    // % of bar length before the next HIT target becomes the current one
    // Current value means old target must travel 10 more % past the forgiveness window
    // to pass the torch
    public const float transitionWindow = 0.1f;

    // An array representing delays in seconds between HIT targets for minigame
    // # of targets increases automatically for each delay added
    private float[] offsets = { 1.5f, 1.5f, 1.5f, 2f, 2f, 1.5f, 1f, 1f };


    // Names
    public const string MATERIAL_QUANTITY_TAG = "materialQuantity";             // Tagname of the number fields on the menu buttons
    public readonly string[] READY_PHRASES = { "Ready", "Set", "Phorge!" };     // Countdown phrases for minigame

    //---------------------------------------------------------------------------------------------
    // Inspector Variables
    //---------------------------------------------------------------------------------------------

    // Important Scene Objects
    public GameObject player_manager;   // The Player Manager object
    public GameObject hammer;           // The animated hammer that the player uses
    public GameObject anvil;            // The anvil grouping. Holds this script, controls the game.
    //private GameObject forShowHammer; // UNUSED

    // Menu Elements
    public Canvas AnvilRhythmGUI;       // The Canvas that holds the minigame's moving parts
    public Canvas AnvilMenu;            // The Canvas that holds the inventory for the minigame
    public Canvas Countdown;            // The Canvas that does the minigame countdown
    private Image timingIndicator;      // The small triangle at the center of the rhythm line.
    private TextMeshProUGUI NowText;    // The result text next to the small triangle in the minigame

    // Prefabs
    public GameObject ingotPrefab;      // Carlos' animated ingot prefab (very cool)
    public GameObject hitIconPrefab;    // CJ's silly little hit icon prefab (bang)

    //---------------------------------------------------------------------------------------------
    // Resolved at Start() / Operational Vars
    //---------------------------------------------------------------------------------------------
    private string cur_state;                 // Current state, set in State_Manager
    private string cur_task;                  // Name of current task, set in State_Manager
    Animator hammerAnimator;                  // Hammer's animator
    private bool anvilStarted;                // Is the anvil minigame truly underway? Set in RunMinigame() and StopGame()
    private bool canStillScore;               // Is a hit icon in the target zone for the minigame? Set in SwingHammer() and HitIconMover()
    private bool hasMissed;                   // Has the player swung and missed?
    private bool hasScored;                   // Has the player swung and hit?
    public float resultQuality;               // The outcome of the minigame
    private int currentStriker;               // The index of the current hit target that the player is attempting to hit
    float damage;                             // The damage the bar takes. Should scale with misses;
    float squish;                             // The amount the bar gets mushed. Should scale with successful hits.
    private List<GameObject> hitIcons;        // The list of all HIT! icons for minigame
    private static int currentIngot;          // The index of the ingot most recently selected from the menu for the minigame
    private Player_Inventory playerInventory; // The Player_Inventory class from playerInventory.cs

    //private float playerHammerTime;         // UNUSED
    //private float hammerTimeStart;          // UNUSED
    //private readonly Color RED = new Color(1f, 0f, 0f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        hammerAnimator = hammer.GetComponent<Animator>();
        //forShowHammer = GameObject.FindGameObjectsWithTag('')
        playerInventory = player_manager.GetComponent<Player_Inventory>();
        Countdown.gameObject.SetActive(false);

        // Set important flag variables
        anvilStarted = false;
        canStillScore = false;
        hasMissed = false;
        hasScored = false;
        // Set initial scalars
        currentIngot = -1;
        resultQuality = 0;
        // Gets solid references for all the important UI components & sets states
        hitIcons = new List<GameObject>();
        AnvilRhythmGUI.enabled = false;
        timingIndicator = AnvilRhythmGUI.transform.GetChild(1).gameObject.GetComponentAtIndex(2) as Image;
        NowText = AnvilRhythmGUI.transform.GetChild(2).gameObject.GetComponentAtIndex(2) as TextMeshProUGUI;
        NowText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();
        if (cur_state == "task_int")
        {
            // If the actual minigame is playing, then update hammer. Don't bother if not
            cur_task = player_manager.GetComponent<Player_Manager>().get_cur_task();
            if (anvilStarted)
            {
                SwingHammer();
                changeIngotAppearance();
            }
        }
        // Extra cleanup on minigame exit
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ClearIngots();
            hammer.SetActive(false);
            stopGame();
        }
    }

    GameObject templateIngot;
    // Takes in index of ingot, passed in onclick from inventory buttons
    public void SpawnIngot(int ingotType)
    {
        // Clear all pre-existing ingots
        ClearIngots();
        // OoB check
        if (ingotType < Player_Inventory.materialNames.Length)
        {
            currentIngot = ingotType;
            // Gets corresponding Material object with index
            var mat = playerInventory.getMaterial(ingotType);
            if (mat == null || mat.getQuantity() <= 0)
            {
                print("Not enough of that Material!");
                return;
            }
            // Names, spawns, scales, tags, and adds a material to the newly-spawned ingot
            string ingotName = playerInventory.getMaterial(ingotType).name;
            templateIngot = Instantiate(ingotPrefab, ANVIL_TOP, Quaternion.identity);
            templateIngot.transform.localScale = new Vector3(INGOT_SCALE, INGOT_SCALE, INGOT_SCALE);
            Material ingotMat = Resources.Load(ingotName) as Material;
            templateIngot.GetComponent<Renderer>().material = ingotMat;
            templateIngot.tag = "ingotPrefab";
        }
    }

    // Runs onclick for the confirm button
    // Launches the minigame and confirms metal selection, officially subtracting from inventory
    public void ConfirmMaterial()
    {
        if (currentIngot < 0)
            return;
        if (playerInventory.getMaterial(currentIngot).getQuantity() <= 0)
        {
            ClearIngots();
            print("Not enough of that Material!");
            return;
        }
        // Decrements one of the chosen material and updates on the UI
        var mat = playerInventory.getMaterial(currentIngot);
        print(mat.spend() + " " + mat.getName() + " ingots left");
        setCount();
        // Hides the anvil inventory and starts the game with the offsets[] array
        AnvilMenu.enabled = false;
        StartCoroutine(RunMinigame(offsets));
    }

    // Deletes all existing ingots spawned with SpawnIngot()
    public void ClearIngots()
    {
        foreach (GameObject ingot in GameObject.FindGameObjectsWithTag("ingotPrefab"))
        {
            Destroy(ingot);
        }
    }

    // Hammer update function
    public void SwingHammer()
    {
        // Make shammer follow horizontal mouse movements
        hammer.transform.parent.position = new Vector3(hammer.transform.position.x, hammer.transform.position.y, (Input.mousePosition.x / 2560f * 2) - 12.319f);
        // Onclick if the minigame is really going
        if (Input.GetKeyDown(KeyCode.Mouse0) && anvilStarted)
        {
            // If there is a hit target in clickable range, and the player hasn't swung + missed on the current target
            // Squish the ingot, turn everything green, update flags to reflect a score
            if (canStillScore && !hasMissed)
            {
                canStillScore = false;
                hasScored = true;
                resultQuality += 1f / (float)offsets.Length;
                print("Item quality = " + resultQuality);
                squish += 100f / (float)offsets.Length;
                timingIndicator.color = Color.green;
                (hitIcons[currentStriker].GetComponentAtIndex(1) as SpriteRenderer).color = Color.green;
                NowText.color = Color.green;
                NowText.text = "HIT!";
            }
            // Otherwise, if the player hasn't scored on the most recent target
            // Damage the ingot, turn everything red, update flags to reflect a miss
            else
            {
                if (!hasScored)
                {
                    canStillScore = false;
                    hasMissed = true;
                    print("MISS! (Bad Swing)");
                    //the ingot takes damage and squishes even if you miss the timing of the swing since the player is still "hitting" the ingot
                    damage += 100f / (float)offsets.Length;
                    squish += 100f / (float)offsets.Length;
                    timingIndicator.color = Color.red;
                    (hitIcons[currentStriker].GetComponentAtIndex(1) as SpriteRenderer).color = Color.red;
                    NowText.color = Color.red;
                    NowText.text = "MISS!";
                    NowText.enabled = true;
                }
            }
            // Always swing on click during the game
            hammerAnimator.SetBool("HammerSwing", true);
            hammerAnimator.speed = 1.5f;
            squish = Mathf.Clamp(squish, 0f, 100f);
            damage = Mathf.Clamp(damage, 0f, 100f);
        }
    }

    // Subroutine to run the minigame.
    // Takes an array of hit icon offsets as a parameter
     IEnumerator RunMinigame(float[] offsetList)
    {
        // Show relevant assets (rhythm game canvas, hammer, etc.)
        AnvilRhythmGUI.enabled = true;
        Cursor.visible = false;
        hammer.SetActive(true);
        squish = 0f;
        damage = 0f;
        Countdown.gameObject.SetActive(true);
        anvilStarted = true;

        // Get text inside of countdown and dynamically change it to create countdown
        var county = Countdown.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        for (int c = 0; c < 3; c++)
        {
            county.text = READY_PHRASES[c];
            yield return new WaitForSeconds(1);
        }

        // For every offset value in given list,
        // Make a new hit target icon and run the timing function on it
        Countdown.gameObject.SetActive(false);
        int tempy = 0;
        currentStriker = 0;
        foreach (float offset in offsetList)
        {
            GameObject tempIcon = Instantiate(hitIconPrefab, Vector3.zero, Quaternion.identity);
            tempIcon.transform.SetParent(AnvilRhythmGUI.gameObject.transform);
            tempIcon.transform.localScale = new Vector3(12, 12, 12);
            tempIcon.transform.localPosition = HIT_ICON_START_POS;
            tempIcon.transform.localRotation = Quaternion.identity;
            hitIcons.Add(tempIcon);
            StartCoroutine(HitIconMover(tempIcon, offset * 1.5f, tempy++));
            yield return new WaitForSeconds(offset);
            Countdown.gameObject.SetActive(false);
        }
        // Wait for the icons to finish running, then gauge completion
        county.text = ("POUNDING COMPLETE!");
        yield return new WaitForSeconds(offsets[offsets.Length - 1] + 0.3f);
        if (resultQuality < 0.50)
        {
            county.text = ("You have failed to Phorge your item!");
        }
        else
        {
            county.text = ("Your item's quality was: " + (int)(resultQuality * 100) + "%");
        }
        resultQuality = 0;
        // Flash results
        Countdown.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        // Close menus and destroy hit icon prefabs
        Countdown.gameObject.SetActive(false);
        hammer.SetActive(false);
        //added stopGame() so that the player is automatically booted from the anvil once they are done
        //Also added stopGame() because previously the player could keep clicking and stretch the ingot into infinity
        stopGame();
        ClearIngots();
        foreach (GameObject hitIcon in hitIcons)
            Destroy(hitIcon);
        hitIcons = new List<GameObject>();
    }

    // Sub-subroutine lol. Basically just animates and logics the hit icons for the minigame
    // Takes a reference to the icon, a float of the time in seconds the icon should travel for, and the id/index of the icon
     IEnumerator HitIconMover(GameObject icon, float duration, int id)
    {
        //float startTime = Time.time;        // UNUSED

        //Every tick, check and adjust icon position
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float midpoint = 0.5f;
            float progress = t / duration;
            // If icon is within acceptable hit range, and has not been acted on already, allow players to click and score
            // Turn everything yellow to indicate readiness.
            if (progress >= midpoint - forgivenessWindow && progress <= midpoint + forgivenessWindow)
            {
                if (currentStriker == id && !hasMissed && !hasScored)
                {
                    canStillScore = true;
                    timingIndicator.color = Color.yellow;
                    (hitIcons[currentStriker].GetComponentAtIndex(1) as SpriteRenderer).color = Color.yellow;
                    NowText.text = "NOW!";
                    NowText.color = Color.yellow;
                    NowText.enabled = true;
                }
            }
            // If icon has left acceptable hit range, and has not been acted on already, remove ability to score
            // Turn everything red to indicate miss.
            if (canStillScore && (progress > midpoint + forgivenessWindow) && currentStriker == id)
            {
                canStillScore = false;
                print("MISS! (No swing)");
                damage += 2f / (float)offsets.Length;
                timingIndicator.color = Color.red;
                (hitIcons[currentStriker].GetComponentAtIndex(1) as SpriteRenderer).color = Color.red;
                NowText.color = Color.red;
                NowText.text = "MISS!";
            }
                // If icon has moved far enough to the right, shift internal focus (currentStriker) to the next one coming & reset action flags
                // Turn everything back to neutral state to indicate fresh start
                if (progress > midpoint + forgivenessWindow + transitionWindow)
                {
                    if (currentStriker == id)
                    {
                        currentStriker++;
                        hasMissed = false;
                        if (id != offsets.Length - 1)
                            hasScored = false;
                        timingIndicator.color = Color.white;
                        NowText.enabled = false;
                    }
                }

                // Actually move icons with time
                icon.transform.localPosition = Vector3.Lerp(HIT_ICON_START_POS, HIT_ICON_END_POS, t / duration);
                yield return 0;
            }
            icon.transform.localPosition = HIT_ICON_END_POS;
        }

    // Ingot animator function
    public void changeIngotAppearance()
    {
        templateIngot.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, squish);
        templateIngot.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, damage);
    }

    // Updates all Anvil Menu quantities using setCount(int)
    public void refreshQuantities()
    {
        for (int i = 0; i < Player_Inventory.numMaterials; i++)
        {
            setCount(i);
        }
    }

    // Dirtily updates currently selected ingot count + on UI
    public void setCount()
    {
        var matName = Player_Inventory.materialNames[currentIngot];
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("materialQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == (matName + " Quant"))
            {
                textComp.text = (playerInventory.getMaterial(currentIngot).getQuantity() + "");
            }
        }
        //return materials[matIndex].getQuantity();
    }

    // Dirtiliy updates specific ingot count + on UI
    public void setCount(int matIndex)
    {
        var matName = Player_Inventory.materialNames[matIndex];
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("materialQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == (matName + " Quant"))
            {
                textComp.text = (playerInventory.getMaterial(matIndex).getQuantity() + "");
            }
        }
        //return materials[matIndex].getQuantity();
    }

    // Resets some crucial flags
    public void stopGame()
    {
        anvilStarted = false;
        canStillScore = false;
        hasMissed = false;
        hasScored = false;
        currentIngot = -1;
        resultQuality = 0;
        AnvilRhythmGUI.enabled = false;
    }
}
