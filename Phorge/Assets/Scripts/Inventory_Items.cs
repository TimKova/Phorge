using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

[System.Serializable]
public class Inventory_Item : IDataPersistence
{
    private string name;
    private int quantity;
    private float quality;
    private string qualityName;
    private float price;
    private bool unlocked;
    private Sprite icon;
    private GameObject prefab;

    //Contructors----------------------------------------------------------------
    public Inventory_Item()
    {
        this.name = "";
        this.quantity = 0;
        this.price = 0f;
        this.quality = 0.6f;
        this.qualityName = getQualityModifier(this.quality);
        this.unlocked = true;
    }
    public Inventory_Item(string name)
    {
        this.name = name;
        this.quantity = 0;
        this.price = 0f;
        this.quality = 0.6f;
        this.qualityName = getQualityModifier(this.quality);
        this.unlocked = true;
    }
    public Inventory_Item(string name, float quality)
    {
        this.quality = quality;
        this.name = $"{this.getQualityModifier()} {name}";
        this.quantity = 1;
        this.quality = 0.6f;
        this.qualityName = getQualityModifier(this.quality);
        this.unlocked = true;
    }
    public Inventory_Item(string name, int quantity)
    {
        this.name = name;
        this.quantity = quantity;
        this.price = 0f;
        this.quality = 0.6f;
        this.qualityName = getQualityModifier(this.quality);
        this.unlocked = true;
    }
    public Inventory_Item(string name, int quantity, float price)
    {
        this.name = name;
        this.quantity = quantity;
        this.price = price;
        this.quality = 0.6f;
        this.qualityName = getQualityModifier(this.quality);
        this.unlocked = true;
    }
    public Inventory_Item(string name, int quantity, float price, bool unlocked)
    {
        this.name = name;
        this.quantity = quantity;
        this.price = price;
        this.quality = 0.6f;
        this.qualityName = getQualityModifier(this.quality);
        this.unlocked = unlocked;
    }

    public string getQualityModifier()
    {
        if (this.quality > 1f)
            return "Supernatural";
        if (this.quality >= 0.9f)
            return "Flawless";
        if (this.quality >= 0.8f)
            return "Fine";
        if (this.quality >= 0.7f)
            return "Good";
        if (this.quality >= 0.6f)
            return "Basic";
        return "Poor";
    }

    public static string getQualityModifier(float quality)
    {
        if (quality > 1f)
            return "Supernatural";
        if (quality >= 0.9f)
            return "Flawless";
        if (quality >= 0.8f)
            return "Fine";
        if (quality >= 0.7f)
            return "Good";
        if (quality >= 0.6f)
            return "Basic";
        return "Poor";
    }

    public override bool Equals(object obj)
    {
        if (obj is Inventory_Item other)
        {
            return this.name == other.name && this.qualityName == other.qualityName;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.name, this.qualityName);
    }

    public int CompareTo(Inventory_Item other)
    {
        if (other == null) return 1;

        int nameComp = this.name.CompareTo(other.name);
        if (nameComp != 0)
            return nameComp;

        return string.Compare(this.qualityName, other.qualityName, StringComparison.Ordinal);
    }

    //Data Persistence------------------------------------------------------------

    public void LoadData(GameData data)
    {
        data.unlockedItems.TryGetValue(name, out Inventory_Item item);
        if (item.isUnlocked())
        {
            setName(item.name);
            setPrice(item.price);
            setQuantity(item.quantity);
            setUnlocked(true);
        }
    }
    public void SaveData(ref GameData data)
    {
        if (data.unlockedItems.ContainsKey(name))
        {
            data.unlockedItems.Remove(name);
        }
        data.unlockedItems.Add(name, this);
    }
    //Getters---------------------------------------------------------------------
    public string getName() { return this.name; }
    public int getQuantity() { return this.quantity; }
    public float getPrice() { return this.price; }
    public float getQuality() { return this.quality; }
    public string getQualityName() { return this.qualityName; }
    public bool isUnlocked() { return this.unlocked; }

    //Setters----------------------------------------------------------------------
    public void setName(string name) { this.name = name; }
    public void setQuantity(int quantity) { this.quantity = quantity; }
    public void setPrice(float price) { this.price = price; }
    public void setQuality(float quality) { this.quality = quality; this.qualityName = getQualityModifier(); }
    public void setQualityName(string qualityName) { this.qualityName = qualityName; }
    public void setUnlocked(bool unlocked) { this.unlocked = unlocked; }

    //Methods----------------------------------------------------------------------
    public float genListPrice(List<Inventory_Item> items)
    {
        float listPrice = 0;
        int stock = 0;

        foreach (Inventory_Item item in items)
        {
            if (item.name.Equals(name))
                stock += 1;
        }//counts how many of that object are in the inventory

        if (stock > 1)//only adjust the price if there's more than 1 of the object. The more you make, the less they are worth.
        {
            float priceReduct = 0.2f;

            listPrice = price * (stock * priceReduct);
        }
        else
        {
            listPrice = price;
        }

        return listPrice;
    }
    public int spend() { this.quantity--; return getQuantity(); }
    public int spend(int amount) { this.quantity -= amount; return getQuantity(); }

    public int gain() { this.quantity += 1; return getQuantity(); }
    public int gain(int amount) { this.quantity += amount; return getQuantity(); }

    public string HowMany() { return $"{this.getQualityModifier()} {this.name}: {this.quantity}"; }
    public override string ToString() { return $"{this.name}"; }
}

public class IngotMaterial : Inventory_Item
{
    // CHANGE - This shouldn't necessarily be durability, I think the better route would be to
    // have a unique array of time offsets for each material. That way, each one gets its own
    // unique rhythm. This wouldn't be hard at all, the longest bit would be just us
    // making and testing each rhythm on our own.

    public float durability;//This can be used to make the anvil game longer or shorter for different ores

    //Constructors----------------------------------------------------------------------
    //This constructor notation with the :base() is what C# uses to call the parent
    //constructor instead of having to set all the values in the child constructor
    public IngotMaterial(string name) : base(name)
    {
        this.durability = 0f;
    }

    public IngotMaterial(string name, int quantity) : base(name, quantity)
    {
        this.durability = 0f;
    }

    public IngotMaterial(string name, int quantity, float price) : base(name, quantity, price)
    {
        this.durability = 0f;
    }

    public IngotMaterial(string name, int quantity, float price, bool unlocked) : base(name, quantity, price, unlocked)
    {
        this.durability = 0f;
    }
    public IngotMaterial(string name, int quantity, float price, bool unlocked, float durability) : base(name, quantity, price, unlocked)
    {
        this.durability = durability;
    }

    //Getters--------------------------------------------------------------------------------
    public float getDurability() { return this.durability; }
    //Setters--------------------------------------------------------------------------------
    public float setDurability(float durability) { this.durability = durability; return getDurability(); }
    public override string ToString() { return $"{this.getQualityName()} {this.getName()} Ingot"; }

}

public class OreMaterial : Inventory_Item
{
    // CHANGE - Melting point is good, but this might be better as 3 variables
    // Zone size determines how tall the ideal zone is for each ore
    // Zone height determines how high the bar is from the bottom
    // Melting Time determines how long the player must keep the ore in the zone

    private float barSize;
    float barHeight;
    float barDuration;
    //should be between 0-1 to match the scale of the furnace game

    //Constructors----------------------------------------------------------------------------------------
    public OreMaterial()
    {
        this.barSize = 0.25f;
        this.barHeight = 0.5f;
        this.barDuration = 4;
    }
    public OreMaterial(float barSize, float barHeight, float barDuration)
    {
        this.barSize = barSize;
        this.barHeight = barHeight;
        this.barDuration = barDuration;
    }
    public OreMaterial(float barSize, float barHeight, float barDuration, string name) : base(name)
    {
        this.barSize = barSize;
        this.barHeight = barHeight;
        this.barDuration = barDuration;
    }
    public OreMaterial(float barSize, float barHeight, float barDuration, string name, int quantity) : base(name, quantity)
    {
        this.barSize = barSize;
        this.barHeight = barHeight;
        this.barDuration = barDuration;
    }
    public OreMaterial(float barSize, float barHeight, float barDuration, string name, int quantity, float price) : base(name, quantity, price)
    {
        this.barSize = barSize;
        this.barHeight = barHeight;
        this.barDuration = barDuration;
    }
    public OreMaterial(float barSize, float barHeight, float barDuration, string name, int quantity, float price, bool unlocked) : base(name, quantity, price, unlocked)
    {
        this.barSize = barSize;
        this.barHeight = barHeight;
        this.barDuration = barDuration;
    }
    //Getters--------------------------------------------------------------------------------------------
    public float[] getFurnaceParameters()
    {

        return new float[] { this.barSize, this.barHeight, this.barDuration };
    }
    //Setters--------------------------------------------------------------------------------------------
    public float[] setFurnaceParameters(float barSize, float barHeight, float barDuration)
    {
        this.barSize = barSize;
        this.barHeight = barHeight;
        this.barDuration = barDuration;
        return getFurnaceParameters();
    }
}

public class Weapon : Inventory_Item
{
    //private float quality;//quality is based on the anvil and sharpening games and helps determine the selling price of the weapon
    private string material;

    //Constructors---------------------------------------------------------------------------------------------
    public Weapon()
    {
        //this.quality = 0;
    }
    public Weapon(float quality, string material)
    {
        this.setQuality(quality);
        this.material = material;
    }
    public Weapon(float quality, string material, string name) : base(name)
    {
        this.setQuality(quality);
        this.material = material;
    }
    public Weapon(float quality, string material, string name, int quantity) : base(name, quantity)
    {
        this.setQuality(quality);
        this.material = material;
    }
    public Weapon(float quality, string material, string name, int quantity, float price) : base(name, quantity, price)
    {
        this.setQuality(quality);
        this.material = material;
    }
    public Weapon(float quality, string material, string name, int quantity, float price, bool unlocked) : base(name, quantity, price, unlocked)
    {
        this.setQuality(quality);
        this.material = material;
    }

    public override bool Equals(object obj)
    {
        if (obj is Weapon other)
        {
            Debug.Log("This Quality = " + this.getQuality());
            Debug.Log("Other Quality = " + other.getQuality());
            Debug.Log("Validity Check = " + (this.getName() == other.getName() && this.getQuality() > other.getQuality() && this.getMaterial() == other.getMaterial()));
            return (this.getName() == other.getName() && this.getQuality() > other.getQuality() && this.getMaterial() == other.getMaterial());
        }
        return false;
    }

    public override int GetHashCode()
    {
        Debug.Log("hashole");

        return HashCode.Combine(this.getName(), this.getQualityName(), this.getMaterial());
    }

    public int CompareTo(Weapon other)
    {
        if (other == null) return 1;

        int nameComp = this.getName().CompareTo(other.getName());
        if (nameComp != 0)
            return nameComp;
        int matComp = this.getMaterial().CompareTo(other.getMaterial());
        if (matComp != 0)
            return matComp;
        Debug.Log("Fuckhead");
        return Mathf.CeilToInt((other.getQuality() - this.getQuality()) * 10);
    }

    //Setters-----------------------------------------------------------------------------------------------
    public override string ToString() { return $"{this.getQualityName()} {this.getMaterial()} {this.getName()}"; }
    public string setMaterial(string material) { this.material = material; return this.material; }

    public string getMaterial() { return this.material; }

}