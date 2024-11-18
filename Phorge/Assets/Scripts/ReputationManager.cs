using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reputation
{
    //string is the name of the faction, int should be the reputation level from 0-100
    private Dictionary<string, int> reputations;

    public Reputation()
    {
        reputations = new Dictionary<string, int>();
        reputations.Add("Knights", 50);
        reputations.Add("Thieves", 50);
        reputations.Add("Elves", 50);
        reputations.Add("Beasts", 50);
    }

    //repHit takes value number of reputation points away from key
    int repHit(string key, int value)
    {
        reputations[key] -= value;
        return reputations[key];
    }//end repHit

    //repHit gives value number of reputation points to key
    int repGain(string key, int value)
    {
        reputations[key] += value;
        return reputations[key];
    }//end repGain

}
