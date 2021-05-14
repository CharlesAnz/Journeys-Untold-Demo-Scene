using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Stat
{
    public string statName;

    [SerializeField]
    private float baseValue;

    //list of modifiers for the stat
    private List<float> mods = new List<float>();

    //returns base stat value + or - all mods in list
    public float GetValue()
    {
        float finalValue = baseValue;
        mods.ForEach(x => finalValue += x);
        return finalValue;
    }

    //adds new mod to stat list
    public void AddModifier(float modifier)
    {
        if (modifier != 0)
            mods.Add(modifier);
    }

    //removes mod from stat list
    public void RemoveModifier(float modifier)
    {
        if (modifier != 0)
            mods.Remove(modifier);
    }

    public List<float> GetMods()
    {
        return mods;
    }
}
