using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestScript : MonoBehaviour
{
    public Player_Inventory pi;
    public DialogueScript ds;
    public NavScript nav;

    public int readyToLeave;
    public State_Manager sm;
    // Start is called before the first frame update
    void Start()
    {
        readyToLeave = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (Weapon weppy in weapons)
        //{
        //    if (weppy.INSERTFIELDHERE == ds.itemName && weppy.INSERTFIELDHERE == ds.itemMetal)
        //    {
        //        //DECREMENT WEPPY

        //    }
        //}

    }
    public void updateVars()
    {
        if (!pi.loseWeapon(ds.currentRequest))
        {
            print("Not quite bucko");
            print("I was looking for a " + ds.currentRequest.ToString());
            return;
        }
        else
        {
            print("Gucci");
            nav.readyToLeaveLocal = 1;
            Player_Inventory.money += Mathf.CeilToInt(ds.currentRequest.getPrice());
            if (nav.gameObject.name == "QuestGiver1")
            {
                sm.swapQ1toggle = true;
                switch (sm.activeFaction1)
                {
                    case "beast":
                        sm.beastRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[3].value = sm.beastRep;
                        break;
                    case "elf":
                        sm.elfRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[2].value = sm.elfRep;
                        break;
                    case "knight":
                        sm.knightRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[0].value = sm.knightRep;
                        break;
                    case "thief":
                        sm.thiefRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[1].value = sm.thiefRep;
                        break;
                }
            }
            if (nav.gameObject.name == "QuestGiver2")
            {
                sm.swapQ2toggle = true;
                switch (sm.activeFaction2)
                {
                    case "beast":
                        sm.beastRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[3].value = sm.beastRep;
                        break;
                    case "elf":
                        sm.elfRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[2].value = sm.elfRep;
                        break;
                    case "knight":
                        sm.knightRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[0].value = sm.knightRep;
                        break;
                    case "thief":
                        sm.thiefRep += sm.BIG_FACTION_CHANGE;
                        sm.sliders[1].value = sm.thiefRep;
                        break;
                }
            }
        }

        print("Bugtest");
    }

    public void sendAway()
    {
        nav.readyToLeaveLocal = 1;
        if (nav.gameObject.name == "QuestGiver1")
        {
            sm.swapQ1toggle = true;
            switch (sm.activeFaction1)
            {
                case "beast":
                    sm.beastRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[3].value = sm.beastRep;
                    break;
                case "elf":
                    sm.elfRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[2].value = sm.elfRep;
                    break;
                case "knight":
                    sm.knightRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[0].value = sm.knightRep;
                    break;
                case "thief":
                    sm.thiefRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[1].value = sm.thiefRep;
                    break;
            }
        }
        if (nav.gameObject.name == "QuestGiver2")
        {
            sm.swapQ2toggle = true;
            switch (sm.activeFaction2)
            {
                case "beast":
                    sm.beastRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[3].value = sm.beastRep;
                    break;
                case "elf":
                    sm.elfRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[2].value = sm.elfRep;
                    break;
                case "knight":
                    sm.knightRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[0].value = sm.knightRep;
                    break;
                case "thief":
                    sm.thiefRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[1].value = sm.thiefRep;
                    break;
            }
        }
        print("Hasta La ByeBye");
    }

    public void sendAway(int questGiverID)
    {
        nav.readyToLeaveLocal = 1;
        if (questGiverID == 1)
        {
            sm.swapQ1toggle = true;
            switch (sm.activeFaction1)
            {
                case "beast":
                    sm.beastRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[3].value = sm.beastRep;
                    break;
                case "elf":
                    sm.elfRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[2].value = sm.elfRep;
                    break;
                case "knight":
                    sm.knightRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[0].value = sm.knightRep;
                    break;
                case "thief":
                    sm.thiefRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[1].value = sm.thiefRep;
                    break;
            }
        }
        if (questGiverID == 2)
        {
            sm.swapQ2toggle = true;
            switch (sm.activeFaction2)
            {
                case "beast":
                    sm.beastRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[3].value = sm.beastRep;
                    break;
                case "elf":
                    sm.elfRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[2].value = sm.elfRep;
                    break;
                case "knight":
                    sm.knightRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[0].value = sm.knightRep;
                    break;
                case "thief":
                    sm.thiefRep -= sm.SMALL_FACTION_CHANGE;
                    sm.sliders[1].value = sm.thiefRep;
                    break;
            }
        }
        print("Hasta La ByeBye");
    }
}
