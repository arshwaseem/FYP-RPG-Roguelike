using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConsumableItem : ScriptableObject
{
    #region Vars
    public string c_name;

    public string description;

    public Sprite icon;

    public Dictionary<string, float> statModifiers;

    public string Tier;

    #endregion


    public virtual void onUse()
    {
        Debug.Log("Item was Consumed");
    }

}
