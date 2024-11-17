using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Inventory : MonoBehaviour, IDataPersistence
{
    public const bool LOCKED = false;
    public const bool UNLOCKED = true;

    //the following 3 lists store all the materials that the player has in their inventory.
    //it's important to remember that one instance of each object stores the quantity
    //that the player has of the object, so there shouldn't be any repeat items in these lists
    public List<OreMaterial> ores = new List<OreMaterial>();
    public List<IngotMaterial> ingots = new List<IngotMaterial>();
    public List<Weapon> weapons = new List<Weapon>();

    public static readonly string[] materialNames = { "Copper", "Bronze", "Iron", "Silver", "Gold", "Uranium" };
    public float[] materialBasePrices = { 1f, 2f, 3f, 4f, 5f, 6f };
    public bool[] materialsUnlocked = { UNLOCKED, LOCKED, LOCKED, LOCKED, UNLOCKED, UNLOCKED };
    public static readonly int numMaterials = materialNames.Length;

    public static readonly string[] weaponNames = { "Sword", "Shield", "Bow", "Arrow", "Dagger", "Hatchet", "Hammer", "Pitchfork" };
    public bool[] schematicsUnlocked = { UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED, UNLOCKED };
    public static readonly int numSchematics = weaponNames.Length;



    public static bool[] merchantGoods = new bool[materialNames.Length];
    public AnvilGame AnvilTask;

    public Canvas moneyUI;
    public static float money;
    //public Canvas AnvilMenu;

    // Start is called before the first frame update
    void Start()
    {

        for (int c = 0; c < materialNames.Length; c++)
        {
            OreMaterial orrey = new OreMaterial(0.17f/numMaterials*(numMaterials-c+0.5f), 0.5f+ 1f/numMaterials*c/2f, 15f, materialNames[c]+" Ore", 7-c, materialBasePrices[c], UNLOCKED);
            ores.Add(orrey);
            IngotMaterial mat = new IngotMaterial(materialNames[c], c + 2, materialBasePrices[c], materialsUnlocked[c]);
            // Save me ig?
            ingots.Add(mat);
            //merchantGoods[c] = mat.isUnlocked();
            //print(materialNames[c]);
            setCount(c);
        }
        money = 500;
        for (int w = 0; w < numSchematics; w++)
        {
            Weapon wep = new Weapon(weaponNames[w], 1f);
            weapons.Add(wep);
        }
        foreach (Weapon weppy in weapons)
        {
            print(weppy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (game)
        updateMoneyUI();
    }

    //GameData Methods------------------------------------------
    public void LoadData(GameData data)
    {
        money = data.money;
        ores = data.ores;
        ingots = data.ingots;
        weapons = data.weapons;

    }
    public void SaveData(ref GameData data)
    {
        data.money = money;
        data.ores = ores;
        data.ingots = ingots;
        data.weapons = weapons;
    }

    public IngotMaterial getIngot(int matIndex)
    {
        return ingots[matIndex];
    }

    public int useIngot(int matIndex)
    {
        return ingots[matIndex].spend();
    }

    public int useIngot(int matIndex, int quant)
    {
        return ingots[matIndex].spend(quant);
    }

    public int purchaseIngot(int matIndex)
    {
        return ingots[matIndex].gain();
    }

    public int purchaseIngot(int matIndex, int quant)
    {
        return ingots[matIndex].gain(quant);
    }

    public OreMaterial getOre(int matIndex)
    {
        return ores[matIndex];
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

    public int setCount(int matIndex)
    {
        var matName = ingots[matIndex].getName();
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("materialQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == (matName + " Quant"))
            {
                textComp.text = (ingots[matIndex].getQuantity() + "");
            }
        }
        return ingots[matIndex].getQuantity();
    }

    //void 
}