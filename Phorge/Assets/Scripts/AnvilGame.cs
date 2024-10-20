using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;



public class AnvilGame : MonoBehaviour
{
    const float INGOT_SCALE = 2.5f;
    private readonly Vector3 ANVIL_TOP = new Vector3(-15.321f, 0.9684f, -11.472f);
    public const string MATERIAL_QUANTITY_TAG = "materialQuantity";
    public readonly string[] READY_PHRASES = { "Ready", "Set", "Phorge!" };
    public float[] offsets = { 2.5f, 2.5f, 2.5f, 1f, 1f, 1f };

    public GameObject player_manager;
    public Canvas AnvilMenu;
    public Canvas Countdown;
    //private GameObject forShowHammer;


    private string cur_state;
    private string cur_task;
    private bool anvilStarted;
    private bool canStoreTime;
    private float playerHammerTime;
    private float hammerTimeStart;
    public float resultQuality;

    public GameObject hammer;
    Animator hammerAnimator;
    public GameObject ingotPrefab;
    float damage;
    float squish;
    public GameObject anvil;

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
        canStoreTime = false;
        currentIngot = -1;
        resultQuality = 0;
        playerInventory.listMerchantGoods();
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
                SwingHammer();
                changeIngotAppearance();
            }
        }
        //if (cur_state == "free_move")
        //{
        //    ClearIngots();
        //}
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ClearIngots();
            hammer.SetActive(false);
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
        setCount(currentIngot);
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (canStoreTime)
            {
                playerHammerTime = Time.time;
                canStoreTime = false;
                float timeDif = Mathf.Abs(playerHammerTime - hammerTimeStart);
                print("You were " + timeDif + " seconds off");
                if (timeDif < 1)
                {
                    print("Hit!");
                    resultQuality += 1f / (float)offsets.Length;
                    damage = resultQuality * 10;
                    print("Item quality = " + resultQuality);
                }
                else
                {
                    print("MISS! (Womp womp)");
                }
            }
            hammerAnimator.SetBool("HammerSwing", true);
            hammerAnimator.speed = 1.5f;
            squish += 10;
        }
    }

    IEnumerator RunMinigame(float[] offsetList)
    {
        
        hammer.SetActive(true);
        Countdown.gameObject.SetActive(true);
        anvilStarted = true;
        var county = Countdown.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        for (int c = 0; c < 3; c++)
        {
            print(c + "...");
            print(READY_PHRASES[c]);
            county.text = READY_PHRASES[c];
            yield return new WaitForSeconds(1);
            //print(Time.time);
        }
        Countdown.gameObject.SetActive(false);
        int tempy = 0;
        foreach (float offset in offsetList)
        {
            county.text = ("HIT " + tempy++);
            yield return new WaitForSeconds(offset);
            Countdown.gameObject.SetActive(true);
            canStoreTime = true;
            hammerTimeStart = Time.time;
            yield return new WaitForSeconds(1);
            Countdown.gameObject.SetActive(false);
        }
        county.text = ("POUNDING COMPLETE!");
        yield return new WaitForSeconds(1);
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
    }

    public void changeIngotAppearance()
    {
        templateIngot.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, squish);
        templateIngot.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(1, damage);
    }

    public void setCount(int matIndex)
    {
        var matName = Player_Inventory.materialNames[matIndex];
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
}
