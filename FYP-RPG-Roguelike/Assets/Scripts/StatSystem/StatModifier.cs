using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StatModifier
{
    public string statName;
    public float value;

    public StatModifier(string statName, float value)
    {
        this.statName = statName;
        this.value = value;
    }

    public void Apply(StatSystem statSystem)
    {
        statSystem.AddStat(statName, value);
    }
}
