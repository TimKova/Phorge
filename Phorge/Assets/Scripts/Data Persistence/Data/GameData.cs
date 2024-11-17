using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float money;
    public Vector3 playerPosition;
    public Dictionary<string, Inventory_Item> unlockedItems;

    public GameData()
    {
        this.money = 0;
        playerPosition = new Vector3(-12, 3, 10);
        unlockedItems = new Dictionary<string, Inventory_Item>();
    }

}
