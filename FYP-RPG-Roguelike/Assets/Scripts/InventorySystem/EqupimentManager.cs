using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class EqupimentManager : MonoBehaviour
{
    public static EqupimentManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
           Instance = this;
        }
        
    }

    Equpiment[] currentEquipment;

    public delegate void onEquipmentChanged(Equpiment newItem, Equpiment oldItem);
    public onEquipmentChanged _onEquipmentChanged;

    private void Start()
    {
        int numSlots = System.Enum.GetNames(typeof(EquipSlot)).Length;
        currentEquipment = new Equpiment[numSlots];
    }

    public void Equip(Equpiment newItem)
    {
        currentEquipment[(int)newItem._equipslot] = newItem;

        Equpiment oldItem = null;
        //Add inventory Functionality Here

        if(_onEquipmentChanged != null)
        {
            _onEquipmentChanged.Invoke(newItem, oldItem);
        }

        InventoryUI.Instance.UpdateEquipmentUI(newItem);
    }

    public void UnEquip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equpiment oldItem = currentEquipment[slotIndex];
            currentEquipment[slotIndex] = null;
            
            if(oldItem.Statname == "Dmg")
            {
                PlayerManager.Instance.playerStats.trueDamage -= oldItem.Statvalue;
            }
            else if (oldItem.Statname == "Armor")
            {
                PlayerManager.Instance.playerStats.Armor -= oldItem.Statvalue;
            }

            if (_onEquipmentChanged != null)
            {
                _onEquipmentChanged.Invoke(null, oldItem);
            }
        }

        InventoryUI.Instance.UpdateEquipmentUI(null);

       
    }
}
