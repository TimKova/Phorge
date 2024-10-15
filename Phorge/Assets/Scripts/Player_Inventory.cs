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
        public float value;
        public bool unlocked;

        public override string ToString() { return name; }

        public IngotMaterial(string name)
        {
            this.name = name;
            this.quantity = 0;
            this.value = 0;
            this.unlocked = false;
        }

        public IngotMaterial(string name, int quantity)
        {
            this.name = name;
            this.quantity = quantity;
            this.value = 0;
            this.unlocked = false;
        }

        public string getName() { return this.name; }
        public int getQuantity() { return this.quantity; }
        public float getValue() { return this.value; }
        public bool isLocked() { return this.unlocked; }

        public void setName(string name) { this.name = name; }
        public void setQuantity(int quantity) { this.quantity = quantity; }
        public void setValue(float value) { this.value = value; }
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

    public List<IngotMaterial> materials = new List<IngotMaterial>();
    public readonly string[] materialNames = { "Copper", "Bronze", "Iron", "Silver", "Gold", "Uranium" };

    public AnvilGame AnvilTask;
    //public Canvas AnvilMenu;

    // Start is called before the first frame update
    void Start()
    {
        for (int c = 0; c < materialNames.Length; c++)
        {
            materials.Add(new IngotMaterial(materialNames[c], c + 2));
            print(materialNames[c]);
            setCount(c);
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