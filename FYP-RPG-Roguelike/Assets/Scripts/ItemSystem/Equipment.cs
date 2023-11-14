using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Equipment",menuName ="Items/Equipment")]
public class Equipment : Item
{
    EquipSlot slot;
    public override void onItemUse()
    {
        Equip();
        Debug.Log("Equipment " + Name + "was used");
    }

    public void Equip()
    {
        Debug.Log(this.Name + "was Equipped onto slot" + slot);
    }

    public enum EquipSlot
    {
        Head,Body,Leg,RightHand,LeftHand,Ring
    }
}
