using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class FurnaceGame : MonoBehaviour
{
    const float INGOT_SCALE = 2.5f;
    private readonly Vector3 FURNACE_TOP = new Vector3(-12.243f, 0.906f, -14.466f);
    public const string MATERIAL_QUANTITY_TAG = "materialQuantity";

    public GameObject player_manager;
    public Canvas FurnaceMenu;
    public GameObject flameEffect;
    Vector3 flameScale;
    float originalFlameMagnitude;

    private string cur_state;
    private string cur_task;

    Animator hammerAnimator;
    public GameObject ingotPrefab;
    public GameObject furnace;

    private static int currentIngot;
    //private readonly Color RED = new Color(1f, 0f, 0f, 1f);

    private Player_Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        //hammerAnimator = hammer.GetComponent<Animator>();
        playerInventory = player_manager.GetComponent<Player_Inventory>();
        originalFlameMagnitude = flameEffect.transform.localScale.magnitude;
        flameScale = flameEffect.transform.localScale;
        currentIngot = -1;
    }

    // Update is called once per frame
    void Update()
    {
        //SwingHammer();
        //cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();
        //if(cur_state == "task_int")
        //{
        //    cur_task = player_manager.GetComponent<Player_Manager>().get_cur_task();
        //    if(cur_task == "Anvil")
        //    {
        //        SwingHammer();
        //    }
        //}
        //if (cur_state == "free_move")
        //{
        //    ClearIngots();
        //}
        Blow();
        flameEffect.transform.localScale = flameScale;
        if (Input.GetKeyDown(KeyCode.F))
        {
            ClearIngots();
        }
    }

    public void SpawnIngot(int ingotType)
    {
            print("FURN");
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
            GameObject templateIngot = Instantiate(ingotPrefab, FURNACE_TOP, Quaternion.identity);
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
        print("why");
        FurnaceMenu.enabled = false;
    }

    public void ClearIngots()
    {
        foreach (GameObject ingot in GameObject.FindGameObjectsWithTag("ingotPrefab"))
        {
            Destroy(ingot);
        }
    }

    //public void SwingHammer()
    //{
    //    if (Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        hammerAnimator.SetBool("HammerSwing", true);
    //    }
    //}

    public void Blow()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            flameScale += new Vector3(0.005f, 0.005f, 0.005f);
        }
        else
        {
            flameScale -= new Vector3(0.005f, 0.005f, 0.005f);
        }
        flameScale.x = Mathf.Clamp(flameScale.x, 0.78f, 1.75f);
        flameScale.y = Mathf.Clamp(flameScale.y, 1f, 1.5f);
        flameScale.z = Mathf.Clamp(flameScale.z, 0.21f, 1.75f);
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

    public void demoMe()
    {
        print("FURNACE PLEASE");
    }
}
