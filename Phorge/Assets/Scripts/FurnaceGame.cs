using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;



public class FurnaceGame : MonoBehaviour
{
    const float ORE_SCALE = 5f;
    private readonly Vector3 FURNACE_TOP = new Vector3(-12.22f, 0.91f, -13.95f);
    public const string MATERIAL_QUANTITY_TAG = "oreQuantity";
    public readonly Vector3 BELLOWS_START_POSITION = new Vector3(-1.383f, 0.178f, 0.627f);
    public readonly Quaternion BELLOWS_START_ROTATION = new Quaternion(0.424148679f, -0.636672974f, -0.357055634f, -0.535963356f);
    public readonly Vector3 BELLOWS_END_POSITION = new Vector3(0.799f, 0.855f, 1.992f);
    public readonly Quaternion BELLOWS_END_ROTATION = new Quaternion(0.0241820589f, -0.960221767f, -0.00242961245f, 0.278178722f);
    public readonly Vector3 MENU_TLC = new Vector3(-800f, 320f, 9f);
    const int SIZE = 0;
    const int HEIGHT = 1;
    const int DURATION = 2;
    const float FURNACE_PROGRESS_STRENGTH = 0.0005f;
    const float BLOWER_SUCK_POWER = 0.5f; //2f;
    public readonly string[] READY_PHRASES = { "Ready", "Set", "Phorge!" };     // Countdown phrases for minigame

    // X position of the range = (+/-80 - rangeHeight). - is high, + is low
    // % occupied bounds = (160 - range height))/160 +- range height/2;
    const float sliderHeight = 160f;
    const float maxSliderOffset = sliderHeight / 2;
    const float sliderWidth = 10f;

    public const float buttonOffset = 176f;

    public GameObject player_manager;
    public Canvas FurnaceMenu;
    public Canvas Countdown;

    public Slider TemperatureSlider;
    public Slider ProgressSlider;
    public Slider TimeSlider;
    public Canvas SliderCanvas;
    public GameObject TemperatureRangePrefab;

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
    public GameObject orePrefab;
    public GameObject furnace;

    public GameObject oreButtonPrefab;  // The prefab for a dynamic menu button
    public List<GameObject> buttons = new List<GameObject>();

    bool gameStarted;
    public float resultQuality;
    public float[] gameParams;
    GameObject currentRange;
    readonly Color RANGE_OPAQUE = new Color(0f, 1f, 0f, 0.6f);
    readonly Color RANGE_TRANSPARENT = new Color(0f, 1f, 0f, 0.2f);


    private static int currentOre;
    //private readonly Color RED = new Color(1f, 0f, 0f, 1f);

    //private Inventory_Item Inventory_Item;
    private Player_Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        //hammerAnimator = hammer.GetComponent<Animator>();
        playerInventory = player_manager.GetComponent<Player_Inventory>();
        originalFlameMagnitude = flameEffect.transform.localScale.magnitude;
        flameScale = flameEffect.transform.localScale;
        maxFlameMagnitude = Vector3.Magnitude(new Vector3(1.57f, 1.5f, 1.75f));
        minFlameMagnitude = flameScale.magnitude;

        currentOre = -1;
        blowerPressCoefficient = 0f;
        resultQuality = 0f;

        gameStarted = false;
        SliderCanvas.enabled = false;
        Countdown.gameObject.SetActive(false);

        for (int c = 0; c < Player_Inventory.numMaterials; c++)
        {
            string name = Player_Inventory.materialNames[c];
            GameObject templateButton = Instantiate(oreButtonPrefab, Vector3.zero, Quaternion.identity);
            templateButton.transform.SetParent(FurnaceMenu.gameObject.transform.GetChild(0));
            templateButton.transform.localScale = new Vector3(1, 1, 1);
            templateButton.transform.localPosition = new Vector3(MENU_TLC.x + buttonOffset * (c % 10), MENU_TLC.y - buttonOffset * (c / 10), MENU_TLC.z);
            templateButton.transform.localRotation = Quaternion.identity;

            int localIndex = c;
            (templateButton.GetComponentAtIndex(3) as Button).onClick.AddListener(delegate { SpawnOre(localIndex); });

            var icon = Resources.Load<Texture2D>(name + "OreIcon");
            (templateButton.transform.GetChild(0).gameObject.GetComponentAtIndex(2) as Image).sprite = Sprite.Create(icon, new Rect(0.0f, 0.0f, icon.width, icon.height), new Vector2(0.5f, 0.5f), 100.0f);
            templateButton.transform.GetChild(0).localScale = new Vector3(0.9f, 0.9f, 0.9f);

            var title = (templateButton.transform.GetChild(1).gameObject.GetComponentAtIndex(2) as TextMeshProUGUI);
            title.text = name;

            var currentAmount = (templateButton.transform.GetChild(2).gameObject.GetComponentAtIndex(2) as TextMeshProUGUI);
            currentAmount.gameObject.tag = "oreQuantity";
            currentAmount.gameObject.name = name + " Quant";
            //print(playerInventory.ores.Count + "OLOLOL");
            currentAmount.text = "0";// playerInventory.ores[c].getQuantity() + "";

            buttons.Add(templateButton);
            //Material ingotMat = Resources.Load(ingotName) as Material;
            //templateIngot.GetComponent<Renderer>().material = ingotMat;
            //templateIngot.tag = "instancedPrefab";
        }

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
                refreshQuantities();
                WholeBlower.transform.localPosition = BELLOWS_END_POSITION;
                WholeBlower.transform.localRotation = BELLOWS_END_ROTATION;
                Blow();
                if (gameStarted && sliderIsInBounds())
                {
                    if (currentRange != null)
                    {
                        (currentRange.GetComponentAtIndex(2) as Image).color = RANGE_TRANSPARENT;
                        ProgressSlider.value += FURNACE_PROGRESS_STRENGTH;
                    }
                }
                else
                {
                    if (currentRange != null)
                        (currentRange.GetComponentAtIndex(2) as Image).color = RANGE_OPAQUE;
                }
            }
        }
        if (cur_state == "free_move")
        {
            gameStarted = false;
            ClearPrefabs();
            WholeBlower.transform.localPosition = BELLOWS_START_POSITION;
            WholeBlower.transform.localRotation = BELLOWS_START_ROTATION;
            SliderCanvas.enabled = false;
            stopGame();
        }
        Blower.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, blowerPressCoefficient);
        flameEffect.transform.localScale = flameScale;
        setTempSlider();
    }

    public void SpawnOre(int oreType)
    {
        ClearPrefabs();
        if (oreType < Player_Inventory.materialNames.Length && oreType >= 0)
        {
            currentOre = oreType;
            var mat = playerInventory.getOre(oreType);
            if (mat == null || mat.getQuantity() <= 0)
            {
                print("Not enough of that Material!");
                return;
            }
            string orename = mat.getName();
            GameObject templateOre = Instantiate(orePrefab, FURNACE_TOP, Quaternion.Euler(0, 135, 0));
            templateOre.transform.localScale = new Vector3(ORE_SCALE, ORE_SCALE, ORE_SCALE);
            Material oreMat = Resources.Load(orename) as Material;
            templateOre.GetComponent<Renderer>().material = oreMat;
            templateOre.tag = "instancedPrefab";
        }
    }

    public void ConfirmMaterial()
    {
        if (currentOre < 0)
            return;
        if (playerInventory.getOre(currentOre).getQuantity() <= 0)
        {
            ClearPrefabs();
            print("Not enough of that Material!");
            return;
        }
        var mat = playerInventory.getOre(currentOre);
        print(mat.spend() + " " + mat.getName() + " ore nodes left");
        setCount(currentOre);
        gameParams = mat.getFurnaceParameters();
        FurnaceMenu.enabled = false;
        SliderCanvas.enabled = true;
        ProgressSlider.value = 0f;
        StartCoroutine(RunMinigame(gameParams[SIZE], gameParams[HEIGHT], gameParams[DURATION]));
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
        //Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetKey(KeyCode.Mouse0) && blowerPressCoefficient != 100f && pressable && gameStarted)
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
            blowerPressCoefficient -= BLOWER_SUCK_POWER;
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
        heatAsPercentage = (maxFlameMagnitude - curFlameMagnitude) / range;

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
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("oreQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == (matName + " Quant"))
            {
                textComp.text = (playerInventory.getOre(matIndex).getQuantity() + "");
                break;
            }
        }
        //return materials[matIndex].getQuantity();
    }

    IEnumerator RunMinigame(float size, float height, float duration)
    {
        var county = Countdown.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        Countdown.gameObject.SetActive(true);
        for (int c = 0; c < 3; c++)
        {
            county.text = READY_PHRASES[c];
            yield return new WaitForSeconds(1);
        }
        Countdown.gameObject.SetActive(false);

        gameStarted = true;
        print($"Size: {size}, Height: {height}, Duration: {duration}sec");
        currentRange = Instantiate(TemperatureRangePrefab, Vector3.zero, Quaternion.identity, TemperatureSlider.gameObject.transform);
        currentRange.transform.localScale = new Vector3(1, size, 1);
        currentRange.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        (currentRange.transform as RectTransform).anchoredPosition3D = new Vector3((80f - 160f * height), 0f, 0f);
        currentRange.tag = "instancedPrefab";
        print(TemperatureSlider.value);
        print($"Lower = {(gameParams[HEIGHT] - gameParams[SIZE] / 2)}, Higher = {(gameParams[HEIGHT] + gameParams[SIZE] / 2)}");
        //if (TemperatureSlider.value >=  && TemperatureSlider.value <= )
        //    print("GOOOD");
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            if (ProgressSlider.value >= 1)
                break;
            TimeSlider.value = 1 - t / duration;
            yield return 0;
        }
        if (ProgressSlider.value < 0.5)
            county.text = "YOU FAILED TO PHORGE THE INGOT";
        else
        {
            print($"Result qual: {ProgressSlider.value}");
            county.text = $"Succesfully Phorged a {Inventory_Item.getQualityModifier(ProgressSlider.value)} {Player_Inventory.materialNames[currentOre]} Ingot!";
        }
        //StartCoroutine(startTimer(duration));
        stopGame();
        Countdown.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        // Close menus and destroy hit icon prefabs
        Countdown.gameObject.SetActive(false);
    }

    bool sliderIsInBounds()
    {
        float currentBarHeight = gameParams[HEIGHT];
        float currentBarSize = gameParams[SIZE];
        return (1 - TemperatureSlider.value >= (currentBarHeight - currentBarSize / 2) && 1 - TemperatureSlider.value <= (currentBarHeight + currentBarSize / 2));
    }

    public void stopGame()
    {
        ClearPrefabs();
        gameStarted = false;
        currentOre = -1;
        FurnaceMenu.enabled = false;
        SliderCanvas.enabled = false;
    }

    public void demoMe()
    {
        print("FURNACE PLEASE");
    }
}
