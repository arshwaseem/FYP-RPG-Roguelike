using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManagementSystem : ScriptableObject
{
    public Dictionary<string, float> statList;

    public float getStat(string name)
    {
        return statList[name];
    }

    public void ApplyStatModifier(string name, float value)
    {
        if (statList.ContainsKey(name))
        {
            statList[name] += value;
        }
    }

    public void RemoveStatModifier(string name, float value)
    {
        if (statList.ContainsKey(name))
        {
            statList[name] -= value;
        }
    }
}
