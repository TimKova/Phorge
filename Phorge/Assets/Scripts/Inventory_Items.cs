using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory_Item : MonoBehaviour, IDataPersistence
{
    private new string name;
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
    public string getName(){return this.name;}
    public int getQuantity(){ return this.quantity;}
    public float getPrice() { return this.price; }
    public bool isUnlocked() { return this.unlocked;}

    //Setters----------------------------------------------------------------------
    public void setName(string name) {this.name = name;}
    public void setQuantity(int quantity) {this.quantity = quantity;}
    public void setPrice(float price) {this.price = price;}
    public void setUnlocked(bool unlocked) { this.unlocked = unlocked;}

    //Methods----------------------------------------------------------------------
    public int spend() {  this.quantity--; return getQuantity(); }
    public int spend(int amount) {  this.quantity -= amount; return getQuantity(); }
    
    public int gain() { this.quantity += 1; return getQuantity(); }
    public int gain(int amount) {  this.quantity += amount; return getQuantity(); }

    public override string ToString() { return this.name;}

}

public class IngotMaterial : Inventory_Item
{
    public float durability;//This can be used to make the anvil game longer or shorter for different ores

    //Constructors----------------------------------------------------------------------
    //This constructor notation with the :base() is what C# uses to call the parent
    //constructor instead of having to set all the values in the child constructor
    public IngotMaterial(string name):base(name) 
    {
        this.durability = 0f;
    }

    public IngotMaterial(string name, int quantity):base(name, quantity)
    {
        this.durability = 0f;
    }

    public IngotMaterial(string name, int quantity, float price):base(name, quantity, price)
    {
        this.durability = 0f;
    }

    public IngotMaterial(string name, int quantity, float price, bool unlocked):base(name, quantity, price, unlocked)
    {
        this.durability = 0f;
    }
    public IngotMaterial(string name, int quantity, float price, bool unlocked, float durability):base(name, quantity, price, unlocked)
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
    private float meltingPoint;//should be between 0-1 to match the scale of the furnace game

    //Constructors----------------------------------------------------------------------------------------
    public OreMaterial()
    { 
        this.meltingPoint = 0.5f; 
    }
    public OreMaterial(float meltingPoint)
    {
        this.meltingPoint = meltingPoint;
    }
    public OreMaterial(string name, float meltingPoint):base(name)
    {
        this.meltingPoint = meltingPoint;
    }
    public OreMaterial(string name, int quantity, float meltingPoint):base(name, quantity)
    {
        this.meltingPoint = meltingPoint;
    }
    public OreMaterial(string name, int quantity, float price, float meltingPoint):base(name, quantity, price)
    {
        this.meltingPoint = meltingPoint;
    }
    public OreMaterial(string name, int quantity, float price, bool unlocked, float meltingPoint):base(name, quantity, price, unlocked)
    {
        this.meltingPoint = meltingPoint;
    }
    //Getters--------------------------------------------------------------------------------------------
    public float getMeltingPoint() { return this.meltingPoint; }
    //Setters--------------------------------------------------------------------------------------------
    public float setMeltingPoint(float meltingPoint) { this.meltingPoint = meltingPoint; return getMeltingPoint(); }
}

public class Weapon : Inventory_Item
{
    private float quality;//quality is based on the anvil and sharpening games and helps determine the selling price of the weapon

    //Constructors---------------------------------------------------------------------------------------------
    public Weapon()
    {
        this.quality = 0;
    }
    public Weapon(float quality) 
    { 
        this.quality = quality;
    }
    public Weapon(string name, float quality):base (name)
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