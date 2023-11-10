using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Equpiment", menuName ="Items/Equipment")]
[Serializable]
public class Equipment : ConsumableItem
{

    public EquipSlot slot;
    public float ad;
    public float armor;

    public void Awake()
    {
        statModifiers = new Dictionary<string, float>
        {
            { "AttackDamage", ad },
            { "Armor", armor }
        };

        Debug.Log("Stats on " + slot + statModifiers.ContainsKey("AttackDamage") + "and" + statModifiers.ContainsKey("Armor"));
    }

    public override void onUse()
    {
        Debug.Log("Equipment"+name+"Was Equipped in Slot:"+slot);
    }


}

public enum EquipSlot
{
    Head, Body, Leg, Ring, RightHand, LeftHand
}
