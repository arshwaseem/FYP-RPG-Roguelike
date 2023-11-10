using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StatSystem
{
    public Dictionary<string, float> stats;

    public void AddStat(string statName, float value)
    {
        if (stats.ContainsKey(statName))
        {
            stats[statName] += value;
        }
        else
        {
            stats[statName] = value;
        }
    }

    public float GetStat(string statName)
    {
        return stats.ContainsKey(statName) ? stats[statName] : 0f;
    }
}
