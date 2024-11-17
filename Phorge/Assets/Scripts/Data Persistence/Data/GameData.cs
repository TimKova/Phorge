using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float money;
    public List<IngotMaterial> ingots;
    public List<OreMaterial> ores;
    public List<Weapon> weapons;

    public GameData()
    {
        this.money = 500;
        this.ingots = new List<IngotMaterial>();
        this.ores = new List<OreMaterial>();
        this.weapons = new List<Weapon>();
    }

}
