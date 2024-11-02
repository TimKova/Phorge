using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;



public class FurnaceGame : MonoBehaviour
{
    const float INGOT_SCALE = 2.5f;
    private readonly Vector3 FURNACE_TOP = new Vector3(-12.243f, 0.906f, -14.466f);
    public const string MATERIAL_QUANTITY_TAG = "materialQuantity";
    public readonly Vector3 BELLOWS_START_POSITION = new Vector3(-1.383f, 0.178f, 0.627f);
    public readonly Quaternion BELLOWS_START_ROTATION = new Quaternion(0.424148679f, -0.636672974f, -0.357055634f, -0.535963356f);
    public readonly Vector3 BELLOWS_END_POSITION = new Vector3(0.799f, 0.855f, 1.992f);
    public readonly Quaternion BELLOWS_END_ROTATION = new Quaternion(0.0241820589f, -0.960221767f, -0.00242961245f, 0.278178722f);

    public GameObject player_manager;
    public Canvas FurnaceMenu;

    public Slider TemperatureSlider;
    public GameObject Blower;
    public GameObject WholeBlower;
    public GameObject flameEffect;
    Vector3 flameScale;
    float maxFlameMagnitude;
    float minFlameMagnitude;

    float blowerPressCoefficient;
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
        blowerPressCoefficient = 0f;
        maxFlameMagnitude = Vector3.Magnitude(new Vector3(1.57f, 1.5f, 1.75f));
        minFlameMagnitude = flameScale.magnitude;
    }

    // Update is called once per frame
    void Update()
    {

        cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();
        if (cur_state == "task_int")
        {
            cur_task = player_manager.GetComponent<Player_Manager>().get_cur_task();
            if (cur_task == "Furnace")
            {
                WholeBlower.transform.localPosition = BELLOWS_END_POSITION;
                WholeBlower.transform.localRotation = BELLOWS_END_ROTATION;
                TemperatureSlider.gameObject.SetActive(true);
                Blow();
            }
        }
        if (cur_state == "free_move")
        {
            ClearPrefabs();
            WholeBlower.transform.localPosition = BELLOWS_START_POSITION;
            WholeBlower.transform.localRotation = BELLOWS_START_ROTATION;
            TemperatureSlider.gameObject.SetActive(false);
        }
        Blower.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, blowerPressCoefficient);
        flameEffect.transform.localScale = flameScale;
        setTempSlider();
    }

    public void SpawnIngot(int ingotType)
    {
        print("FURN");
        // Tried with both uppercase and lowercase ingot types. I just don't know how we're getting the input,
        // It doesn't appear that the inventory field is updating, even though I think it logically should?
        ClearPrefabs();
        if (ingotType < Player_Inventory.materialNames.Length)
        {
            currentIngot = ingotType;
            var mat = playerInventory.getIngot(ingotType);
            if (mat == null || mat.getQuantity() <= 0)
            {
                print("Not enough of that Material!");
                return;
            }
            string ingotName = mat.getName();
            GameObject templateIngot = Instantiate(ingotPrefab, FURNACE_TOP, Quaternion.identity);
            templateIngot.transform.localScale = new Vector3(INGOT_SCALE, INGOT_SCALE, INGOT_SCALE);
            Material ingotMat = Resources.Load(ingotName) as Material;
            templateIngot.GetComponent<Renderer>().material = ingotMat;
            templateIngot.tag = "instancedPrefab";
        }
    }

    public void ConfirmMaterial()
    {
        if (currentIngot < 0)
            return;
        if (playerInventory.getIngot(currentIngot).getQuantity() <= 0)
        {
            ClearPrefabs();
            print("Not enough of that Material!");
            return;
        }
        var mat = playerInventory.getIngot(currentIngot);
        print(mat.spend() + " " + mat.getName() + " ingots left");
        setCount(currentIngot);
        print("why");
        FurnaceMenu.enabled = false;
    }

    public void ClearPrefabs()
    {
        foreach (GameObject preff in GameObject.FindGameObjectsWithTag("instancedPrefab"))
        {
            Destroy(preff);
        }
    }

    //public void SwingHammer()
    //{
    //    if (Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        hammerAnimator.SetBool("HammerSwing", true);
    //    }
    //}

    bool pressable = false;//Makes sure that the player can only press the blow 1 time per click regardless of how far they push it down
    public void Blow()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetKey(KeyCode.Mouse0) && blowerPressCoefficient != 100f && pressable)
        {
            flameScale += new Vector3(0.003f, 0.003f, 0.003f);
            blowerPressCoefficient += 1f;
            if (blowerPressCoefficient >= 100f)
            {
                pressable = false;
            }
        }
        else
        {
            pressable = false;
            flameScale -= new Vector3(0.0006f, 0.0006f, 0.0006f);
            blowerPressCoefficient -= 0.5f;
            if (blowerPressCoefficient <= 0f)
            {
                pressable = true;
            }
        }
        blowerPressCoefficient = Mathf.Clamp(blowerPressCoefficient, 0, 100);
        flameScale.x = Mathf.Clamp(flameScale.x, 0.78f, 1.75f);
        flameScale.y = Mathf.Clamp(flameScale.y, 1f, 1.5f);
        flameScale.z = Mathf.Clamp(flameScale.z, 0.21f, 1.75f);
    }

    public void setTempSlider()
    {
        float heatAsPercentage = 0f;
        float range = maxFlameMagnitude - minFlameMagnitude;
        float curFlameMagnitude = flameScale.magnitude;
        heatAsPercentage = (curFlameMagnitude - minFlameMagnitude) / range;

        TemperatureSlider.value = heatAsPercentage;
    }

    public void refreshQuantities()
    {
        for (int i = 0; i < Player_Inventory.numMaterials; i++)
        {
            setCount(i);
        }
    }

    public void setCount(int matIndex)
    {
        var matName = Player_Inventory.materialNames[matIndex];
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("materialQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == (matName + " Quant"))
            {
                textComp.text = (playerInventory.getIngot(currentIngot).getQuantity() + "");
            }
        }
        //return materials[matIndex].getQuantity();
    }

    public void demoMe()
    {
        print("FURNACE PLEASE");
    }
}
