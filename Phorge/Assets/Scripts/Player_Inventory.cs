using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Inventory : MonoBehaviour
{

    public class IngotMaterial
    {
        public int quantity;
        public string name;
        public float price;
        public bool unlocked;

        public override string ToString() { return name; }

        public IngotMaterial(string name)
        {
            this.name = name;
            this.quantity = 0;
            this.price = 0;
            this.unlocked = true;
        }

        public IngotMaterial(string name, int quantity)
        {
            this.name = name;
            this.quantity = quantity;
            this.price = 0;
            this.unlocked = true;
        }

        public IngotMaterial(string name, int quantity, float price)
        {
            this.name = name;
            this.quantity = quantity;
            this.price = price;
            this.unlocked = true;
        }

        public IngotMaterial(string name, int quantity, float price, bool unlocked)
        {
            this.name = name;
            this.quantity = quantity;
            this.price = price;
            this.unlocked = unlocked;
        }


        public string getName() { return this.name; }
        public int getQuantity() { return this.quantity; }
        public float getPrice() { return this.price; }
        public bool isUnlocked() { return this.unlocked; }

        public void setName(string name) { this.name = name; }
        public void setQuantity(int quantity) { this.quantity = quantity; }
        public void setPrice(float value) { this.price = value; }
        public void unlock() { this.unlocked = true; }

        public int spend()
        {
            this.quantity--;
            return this.quantity;
        }
        public int spend(int quant)
        {
            this.quantity -= quant;
            return this.quantity;
        }

        public int gain()
        {
            this.quantity++;
            return this.quantity;
        }
        public int gain(int quant)
        {
            this.quantity += quant;
            return this.quantity;
        }
    }
    public const bool LOCKED = false;
    public const bool UNLOCKED = true;

    public List<IngotMaterial> materials = new List<IngotMaterial>();
    public static readonly string[] materialNames = { "Copper", "Bronze", "Iron", "Silver", "Gold", "Uranium" };
    public float[] materialBasePrices = { 1f, 2f, 3f, 4f, 5f, 6f };
    public bool[] materialsUnlocked = { UNLOCKED, LOCKED, LOCKED, LOCKED, UNLOCKED, UNLOCKED };
    public static readonly int numMaterials = materialNames.Length;
    public static bool[] merchantGoods = new bool[materialNames.Length];
    public AnvilGame AnvilTask;
    public static float money;
    //public Canvas AnvilMenu;

    // Start is called before the first frame update
    void Start()
    {
        for (int c = 0; c < materialNames.Length; c++)
        {
            IngotMaterial mat = new IngotMaterial(materialNames[c], c + 2, materialBasePrices[c], materialsUnlocked[c]);
            // Save me ig?
            materials.Add(mat);
            merchantGoods[c] = mat.isUnlocked();
            print(materialNames[c]);
            setCount(c);
            money = 500;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (game)
    }

    public IngotMaterial getMaterial(int matIndex)
    {
        return materials[matIndex];
    }

    public int useMaterial(int matIndex)
    {
        return materials[matIndex].spend();
    }

    public int useMaterial(int matIndex, int quant)
    {
        return materials[matIndex].spend(quant);
    }

    public int purchaseMaterial(int matIndex)
    {
        return materials[matIndex].gain();
    }

    public int purchaseMaterial(int matIndex, int quant)
    {
        return materials[matIndex].gain(quant);
    }

    public void listMerchantGoods()
    {
        for(int c = 0;c<numMaterials; c++)
        {
            if (merchantGoods[c])
                print("Merchant sells" + materials[c].getName() + " for $" + materials[c].getPrice());
        }
    }

    public int setCount(int matIndex)
    {
        var matName = materials[matIndex].getName();
        foreach (GameObject quant in GameObject.FindGameObjectsWithTag("materialQuantity"))
        {
            var textComp = quant.GetComponent<TextMeshProUGUI>();
            if (textComp.name == ( matName + " Quant")){
               textComp.text = (materials[matIndex].getQuantity() + "");
            }
        }
        return materials[matIndex].getQuantity();
    }

    //void 
}