using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory_Item
{
    private string name;
    private int quantity;
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
        this.unlocked = false;
    }
    public Inventory_Item(string name)
    {
        this.name = name;
        this.quantity = 0;
        this.price = 0f;
        this.unlocked = false;
    }
    public Inventory_Item(string name, int quantity)
    {
        this.name = name;
        this.quantity = quantity;
        this.price = 0f;
        this.unlocked = false;
    }
    public Inventory_Item(string name, int quantity, float price)
    {
        this.name = name;
        this.quantity = quantity;
        this.price = price;
        this.unlocked = false;
    }
    public Inventory_Item(string name, int quantity, float price, bool unlocked)
    {
        this.name = name;
        this.quantity = quantity;
        this.price = price;
        this.unlocked = unlocked;
    }

    //public void Init()
    //{
    //    this.name = "";
    //    this.quantity = 0;
    //    this.price = 0f;
    //    this.unlocked = false;
    //}
    //public void Init(string name)
    //{
    //    this.name = name;
    //    this.quantity = 0;
    //    this.price = 0f;
    //    this.unlocked = false;
    //}
    //public void Init(string name, int quantity)
    //{
    //    this.name = name;
    //    this.quantity = quantity;
    //    this.price = 0f;
    //    this.unlocked = false;
    //}
    //public void Init(string name, int quantity, float price)
    //{
    //    this.name = name;
    //    this.quantity = quantity;
    //    this.price = price;
    //    this.unlocked = false;
    //}
    //public void Init(string name, int quantity, float price, bool unlocked)
    //{
    //    this.name = name;
    //    this.quantity = quantity;
    //    this.price = price;
    //    this.unlocked = unlocked;
    //}

    //Getters---------------------------------------------------------------------
    public string getName() { return this.name; }
    public int getQuantity() { return this.quantity; }
    public float getPrice() { return this.price; }
    public bool isUnlocked() { return this.unlocked; }

    //Setters----------------------------------------------------------------------
    public void setName(string name) { this.name = name; }
    public void setQuantity(int quantity) { this.quantity = quantity; }
    public void setPrice(float price) { this.price = price; }
    public void setUnlocked(bool unlocked) { this.unlocked = unlocked; }

    //Methods----------------------------------------------------------------------
    public int spend() { this.quantity--; return getQuantity(); }
    public int spend(int amount) { this.quantity -= amount; return getQuantity(); }

    public int gain() { this.quantity += 1; return getQuantity(); }
    public int gain(int amount) { this.quantity += amount; return getQuantity(); }

    public override string ToString() { return this.name + ": " + this.quantity; }

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
    private float quality;//quality is based on the anvil and sharpening games and helps determine the selling price of the weapon
    private string material;

    //Constructors---------------------------------------------------------------------------------------------
    public Weapon()
    {
        this.quality = 0;
    }
    public Weapon(float quality)
    {
        this.quality = quality;
    }
    public Weapon(string name, float quality) : base(name)
    {
        this.quality = quality;
    }
    public Weapon(string name, int quantity, float quality) : base(name, quantity)
    {
        this.quality = quality;
    }
    public Weapon(string name, int quantity, float price, float quality) : base(name, quantity, price)
    {
        this.quality = quality;
    }
    public Weapon(string name, int quantity, float price, bool unlocked, float quality) : base(name, quantity, price, unlocked)
    {
        this.quality = quality;
    }
    //Getters-----------------------------------------------------------------------------------------------
    public float GetQuality() { return this.quality; }
    //Setters-----------------------------------------------------------------------------------------------
    public float SetQuality(float quality) { this.quality = quality; return GetQuality(); }
}