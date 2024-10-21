using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;



public class AnvilGame : MonoBehaviour
{
    // Constants
    //---------------------------------------------------------------------------------------------
    
    // Positions
        private readonly Vector3 ANVIL_TOP = new Vector3(-15.321f, 0.9684f, -11.472f);  // World position (I think) for top of anvil
        public readonly Vector3 HIT_ICON_START_POS = new Vector3(-1436f, -456f, 61f);   // Local position for where hit icons start relative to rhythm game canvas
        public readonly Vector3 HIT_ICON_END_POS = new Vector3(1460f, -456f, 61f);      // Same as above, but for end
        //public const float HIT_ICON_CHECK_X = 0f;
        //public const float HIT_ICON_DONE_X = 1460f;
    
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


    public GameObject player_manager;
    public Canvas AnvilRhythmGUI;
    public Canvas AnvilMenu;
    public Canvas Countdown;
    private Image timingIndicator;
    private TextMeshProUGUI NowText;
    //private GameObject forShowHammer;

    private string cur_state;
    private string cur_task;
    private bool anvilStarted;
    private bool canStillScore;
    private bool hasMissed;
    private bool hasScored;
    private float playerHammerTime;
    private float hammerTimeStart;
    public float resultQuality;
    private int currentStriker;

    public GameObject hammer;
    Animator hammerAnimator;
    public GameObject ingotPrefab;
    float damage;
    float squish;
    public GameObject anvil;
    public GameObject hitIconPrefab;
    private List<GameObject> hitIcons;

    private static int currentIngot;
    //private readonly Color RED = new Color(1f, 0f, 0f, 1f);

    private Player_Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        hammerAnimator = hammer.GetComponent<Animator>();
        //forShowHammer = GameObject.FindGameObjectsWithTag('')
        playerInventory = player_manager.GetComponent<Player_Inventory>();
        Countdown.gameObject.SetActive(false);
        anvilStarted = false;
        canStillScore = false;
        hasMissed = false;
        hasScored = false;
        currentIngot = -1;
        resultQuality = 0;
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
            cur_task = player_manager.GetComponent<Player_Manager>().get_cur_task();
            if (anvilStarted)
            {
                //progress = Mathf.Clamp01(progress);

                SwingHammer();
                changeIngotAppearance();
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ClearIngots();
            hammer.SetActive(false);
            stopGame();
        }
    }

    GameObject templateIngot;
    public void SpawnIngot(int ingotType)
    {
        // Tried with both uppercase and lowercase ingot types. I just don't know how we're getting the input,
        // It doesn't appear that the inventory field is updating, even though I think it logically should?
        ClearIngots();
        if (ingotType < Player_Inventory.materialNames.Length)
        {
            currentIngot = ingotType;
            var mat = playerInventory.getMaterial(ingotType);
            if (mat == null || mat.getQuantity() <= 0)
            {
                print("Not enough of that Material!");
                return;
            }
            string ingotName = playerInventory.getMaterial(ingotType).name;
            templateIngot = Instantiate(ingotPrefab, ANVIL_TOP, Quaternion.identity);
            templateIngot.transform.localScale = new Vector3(INGOT_SCALE, INGOT_SCALE, INGOT_SCALE);
            Material ingotMat = Resources.Load(ingotName) as Material;
            templateIngot.GetComponent<Renderer>().material = ingotMat;
            templateIngot.tag = "ingotPrefab";
        }
    }

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
        var mat = playerInventory.getMaterial(currentIngot);
        print(mat.spend() + " " + mat.getName() + " ingots left");
        setCount();
        AnvilMenu.enabled = false;
        StartCoroutine(RunMinigame(offsets));
    }

    public void ClearIngots()
    {
        foreach (GameObject ingot in GameObject.FindGameObjectsWithTag("ingotPrefab"))
        {
            Destroy(ingot);
        }
    }

    public void SwingHammer()
    {
        hammer.transform.parent.position = new Vector3(hammer.transform.position.x, hammer.transform.position.y, (Input.mousePosition.x / 2560f * 2) - 12.319f);
        if (Input.GetKeyDown(KeyCode.Mouse0) && anvilStarted)
        {
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
            else
            {
                if (!hasScored)
                {
                    canStillScore = false;
                    hasMissed = true;
                    print("MISS! (Bad Swing)");
                    damage += 2f / (float)offsets.Length;
                    timingIndicator.color = Color.red;
                    (hitIcons[currentStriker].GetComponentAtIndex(1) as SpriteRenderer).color = Color.red;
                    NowText.color = Color.red;
                    NowText.text = "MISS!";
                    NowText.enabled = true;
                }
            }
            hammerAnimator.SetBool("HammerSwing", true);
            hammerAnimator.speed = 1.5f;
            squish = Mathf.Clamp(squish, 0f, 100f);
            damage = Mathf.Clamp(damage, 0f, 100f);
        }
    }

    IEnumerator RunMinigame(float[] offsetList)
    {
        AnvilRhythmGUI.enabled = true;
        hammer.SetActive(true);
        Countdown.gameObject.SetActive(true);
        anvilStarted = true;
        var county = Countdown.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        for (int c = 0; c < 3; c++)
        {
            county.text = READY_PHRASES[c];
            yield return new WaitForSeconds(1);
        }
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
        county.text = ("POUNDING COMPLETE!");
        yield return new WaitForSeconds(offsets[offsets.Length-1]+0.3f);
        if (resultQuality < 0.50)
        {
            county.text = ("You have failed to Phorge your item!");
        }
        else
        {
            county.text = ("Your item's quality was: " + (int)(resultQuality * 100) + "%");
        }
        resultQuality = 0;
        squish = 0;
        damage = 0;
        Countdown.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        Countdown.gameObject.SetActive(false);
        hammer.SetActive(false);
        foreach (GameObject hitIcon in hitIcons)
            Destroy(hitIcon);
        hitIcons = new List<GameObject>();
    }

    IEnumerator HitIconMover(GameObject icon, float duration, int id)
    {
        float startTime = Time.time;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float midpoint = 0.5f;
            float progress = t / duration;
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

            icon.transform.localPosition = Vector3.Lerp(HIT_ICON_START_POS, HIT_ICON_END_POS, t / duration);
            yield return 0;
        }
        icon.transform.localPosition = HIT_ICON_END_POS;
    }

    public void changeIngotAppearance()
    {
        templateIngot.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, squish);
        templateIngot.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, damage);
    }

    public void refreshQuantities()
    {
        for (int i = 0; i < Player_Inventory.numMaterials; i++)
        {
            setCount(i);
        }
    }

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
