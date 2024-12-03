using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class Player_Inventory : MonoBehaviour, IDataPersistence
{
    public const bool LOCKED = false;
    public const bool UNLOCKED = true;
    public readonly Vector3 MENU_TLC = new Vector3(-800f, 275f, 9f);
    public const float buttonOffset = 176f;

    //the following 3 lists store all the materials that the player has in their inventory.
    //it's important to remember that one instance of each object stores the quantity
    //that the player has of the object, so there shouldn't be any repeat items in these lists
    public List<Inventory_Item> ores = new List<Inventory_Item>();
    public List<Inventory_Item> ingots = new List<Inventory_Item>();
    public List<Inventory_Item> weapons = new List<Inventory_Item>();

    public static readonly string[] materialNames = { "Copper", "Bronze", "Iron", "Silver", "Gold", "Uranium" };
    public float[] materialBasePrices = { 1f, 2f, 3f, 4f, 5f, 6f };

    public bool[] materialsUnlocked = { UNLOCKED, LOCKED, LOCKED, LOCKED, UNLOCKED, UNLOCKED };
    public static readonly int numMaterials = materialNames.Length;

    public static readonly string[] weaponNames = { "Sword", "Shield", "Crossbow", "Dagger", "Axe", "Hammer" };
    public bool[] schematicsUnlocked = { UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED };
    public static readonly int numSchematics = weaponNames.Length;

    public static List<Inventory_Item>[] megaInventory;

    public GameObject player_manager;
    private string cur_state;

    public static bool[] merchantGoods = new bool[materialNames.Length];
    public AnvilGame AnvilTask;

    public Canvas moneyUI;
    public static float money;

    public GameObject quantityButtonPrefab;
    public Sprite whitebox;

    public Canvas InventoryMenu;
    private List<GameObject> InventoryScreens;
    private List<GameObject> InventoryTabs;

    public List<GameObject> buttons = new List<GameObject>();

    private int tempy = 0;
    private float qually = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

        for (int c = 0; c < materialNames.Length; c++)
        {
            OreMaterial orrey = new OreMaterial(0.17f / numMaterials * (numMaterials - c + 0.5f), 0.5f + 1f / numMaterials * c / 2f, 15f, materialNames[c] + " Ore", 0, materialBasePrices[c], UNLOCKED);
            ores.Add(orrey);
            IngotMaterial mat = new IngotMaterial(materialNames[c], 1, materialBasePrices[c], UNLOCKED);
            // Save me ig?
            ingots.Add(mat);
            //merchantGoods[c] = mat.isUnlocked();
            print(materialNames[c]);
        }
        //foreach (Inventory_Item ore in ores)
        //    print(ore);

        //foreach (Inventory_Item ingot in ingots)
        //    print(ingot);

        money = 500;
        for (int w = 0; w < numSchematics; w++)
        {
            Weapon wep = new Weapon(1f, "Plain", weaponNames[w]);
            //weapons.Add(wep);
        }
        foreach (Weapon weppy in weapons)
        {
            print(weppy);
        }

        megaInventory = new List<Inventory_Item>[4];
        megaInventory[0] = ores;
        megaInventory[1] = ingots;
        megaInventory[2] = ingots;
        megaInventory[3] = weapons;


        InventoryScreens = new List<GameObject>();
        InventoryTabs = new List<GameObject>();
        for (int c = 0; c < InventoryMenu.transform.GetChild(0).childCount; c++)
        {
            InventoryScreens.Add(InventoryMenu.transform.GetChild(0).transform.GetChild(c).gameObject);
            print($"InventoryScreens Count: {InventoryScreens.Count}");
            //print(megaInventory[0][1].getName());
            for (int n = 0; n < megaInventory[c].Count; n++)
            {
                makeButton(InventoryScreens[c], megaInventory[c][n], n);
            }
        }
        totalRefresh();
        //print($"IventoryScreens.Count = {InventoryScreens.Count}");
        //for (int c = 0; c < InventoryMenu.transform.GetChild(1).childCount; c++)
        //{
        //    InventoryTabs.Add(InventoryMenu.transform.GetChild(c).transform.GetChild(c).gameObject);

        //}
    }

    // Update is called once per frame
    void Update()
    {
        updateMoneyUI();
        cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();
        if (cur_state == "inventory")
        {
            print("booyah");
            totalRefresh();
        }
        //if (Input.GetKeyDown("p"))
        //{
        //    var newGuy = new Weapon(0.7f, "Iron", "Sword", 1, 1f, true);
        //    //newGuy.setQuality(qually);
        //    gainWeapon(newGuy, qually);
        //    //print("fuckmetal");
        //    //print($"Length of weapons = {weapons.Count}");
        //    //if (weapons.Contains(newGuy))
        //    //{
        //    //    print("Already there bb");
        //    //    print($"Newguy quality = {newGuy.getQuality()}, modifier = {newGuy.getQualityName()}");
        //    //    weapons[tempy - 1].gain();
        //    //    totalRefresh();
        //    qually += 0.05f;
        //    //}
        //    //else
        //    //{
        //    //    print("NEWBALL");
        //    //    weapons.Add(newGuy);
        //    //    makeButton(InventoryScreens[3], weapons[tempy], tempy++);
        //    //}
        //}

    }

    //void DrawInventory()

    //GameData Methods------------------------------------------
    public void LoadData(GameData data)
    {
        //money = data.money;
        //money += 100;
    }
    public void SaveData(ref GameData data)
    {
        //data.money = money;
    }

    public IngotMaterial getIngot(int matIndex)
    {
        return ingots[matIndex] as IngotMaterial;
    }

    public int useIngot(int matIndex)
    {
        return ingots[matIndex].spend();
    }

    public int useIngot(int matIndex, int quant)
    {
        return ingots[matIndex].spend(quant);
    }

    public int gainIngot(IngotMaterial ingot, float quality)
    {
        ingot.setQuality(quality);
        if (ingots.Contains(ingot))
        {
            //print("Already there bb");
            //print($"Newguy quality = {newGuy.getQuality()}, modifier = {newGuy.getQualityName()}");
            ingots[ingots.IndexOf(ingot)].gain();
            totalRefresh();
        }
        else
        {
            print("NEWBALL");
            ingots.Add(ingot);
            makeButton(InventoryScreens[1], ingot, ingots.Count - 1);
        }
        return 0;
    }

    public int gainWeapon(Weapon weapon)
    {
        if (weapons.Contains(weapon))
        {
            //print("Already there bb");
            //print($"Newguy quality = {newGuy.getQuality()}, modifier = {newGuy.getQualityName()}");
            weapons[weapons.IndexOf(weapon)].gain();
            totalRefresh();
        }
        else
        {
            weapons.Add(weapon);
            makeButton(InventoryScreens[3], weapon, weapons.Count - 1);
        }
        return 0;
    }

    public bool loseWeapon(Weapon weapon)
    {
        int hasWeapon = weapons.IndexOf(weapon);
        if (hasWeapon < 1 )
        {
            print("Not enough buster");
            //print($"Newguy quality = {newGuy.getQuality()}, modifier = {newGuy.getQualityName()}");
            return false;
        }
        else
        {
            //weapons.Add(weapon);
            weapons[weapons.IndexOf(weapon)].spend();
            //makeButton(InventoryScreens[3], weapon, weapons.Count - 1);
            totalRefresh();
        }
        return true;
    }

    public int gainIngot(int matIndex)
    {
        if (ingots.Contains(getIngot(matIndex)))
            return 0;
        else
            return ingots[matIndex].gain();
    }

    public int gainIngot(int matIndex, int quant)
    {
        return ingots[matIndex].gain(quant);
    }

    public OreMaterial getOre(int matIndex)
    {
        return ores[matIndex] as OreMaterial;
    }

    public int useOre(int matIndex)
    {
        return ores[matIndex].spend();
    }

    public int useOre(int matIndex, int quant)
    {
        return ores[matIndex].spend(quant);
    }

    public int purchaseOre(int matIndex)
    {
        return ores[matIndex].gain();
    }

    public int purchaseOre(int matIndex, int quant)
    {
        return ores[matIndex].gain(quant);
    }

    public void goToScreen(int screenIndex)
    {
        print($"ScreenIndex: {screenIndex}");
        for (int c = 0; c < InventoryScreens.Count; c++)
        {
            print("wow" + c);
            InventoryScreens[c].SetActive(false);
        }
        InventoryScreens[screenIndex].SetActive(true);
    }

    public void listMerchantGoods()
    {
        for (int c = 0; c < numMaterials; c++)
        {
            if (merchantGoods[c])
                print("Merchant sells" + ingots[c].getName() + " for $" + ingots[c].getPrice());
        }
    }

    public void updateMoneyUI()
    {
        var moneyText = moneyUI.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        moneyText.text = "$" + money.ToString();
    }

    public int setCount(List<Inventory_Item> arr, int matIndex)
    {
        var matName = arr[matIndex].ToString();
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("materialQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == (matName + " Quant"))
            {
                textComp.text = (arr[matIndex].getQuantity() + "");
            }
        }
        return arr[matIndex].getQuantity();
    }

    public void totalRefresh()
    {
        foreach (List<Inventory_Item> arr in megaInventory)
        {
            for (int c = 0; c < arr.Count; c++)
                setCount(arr, c);
        }
    }

    public void makeButton(GameObject parentMenu, Inventory_Item self, int id)
    {
        string name = self.getName();

        GameObject templateButton = Instantiate(quantityButtonPrefab, Vector3.zero, Quaternion.identity);
        templateButton.transform.SetParent(parentMenu.transform);
        templateButton.transform.localScale = new Vector3(1, 1, 1);
        templateButton.transform.localPosition = new Vector3(MENU_TLC.x + buttonOffset * (id % 10), MENU_TLC.y - buttonOffset * (id / 10), MENU_TLC.z);
        templateButton.transform.localRotation = Quaternion.identity;

        (templateButton.GetComponentAtIndex(3) as Button).onClick.AddListener(delegate { print("Fuck"); });

        var icon = Resources.Load<Texture2D>(name.Replace(" ", "") + "Icon");
        (templateButton.transform.GetChild(0).gameObject.GetComponentAtIndex(2) as Image).sprite = Sprite.Create(icon, new Rect(0.0f, 0.0f, icon.width, icon.height), new Vector2(0.5f, 0.5f), 100.0f);

        var title = (templateButton.transform.GetChild(1).gameObject.GetComponentAtIndex(2) as TextMeshProUGUI);
        title.text = self.ToString();

        var currentAmount = (templateButton.transform.GetChild(2).gameObject.GetComponentAtIndex(2) as TextMeshProUGUI);
        currentAmount.gameObject.tag = "materialQuantity";
        currentAmount.gameObject.name = self.ToString() + " Quant";
        currentAmount.text = self.getQuantity() + "";

        if (!self.isUnlocked())
        {
            (templateButton.GetComponentAtIndex(3) as Button).interactable = false;
            (templateButton.GetComponentAtIndex(2) as Image).sprite = whitebox;
            currentAmount.text = "";
            return;
        }
        //buttons.Add(templateButton);

        //return templateButton;
    }


    //void 
}