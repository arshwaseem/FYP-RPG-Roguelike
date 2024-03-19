using DungeonArchitect.Themeing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Inventory/Equipment")]

public class Equpiment : Item
{
    public EquipSlot _equipslot;

    public override void onUse()
    {
        Debug.Log(name + "used");
        EqupimentManager.Instance.Equip(this);
            if(Statname == "Dmg")
            {
                PlayerManager.Instance.playerStats.trueDamage += Statvalue;
            }
            if(Statname == "Armor")
            {
                PlayerManager.Instance.playerStats.Armor += Statvalue;
            }
        }
}

public enum EquipSlot {Head, Body, Leg, Feet, Ring, RHand, LHand}
