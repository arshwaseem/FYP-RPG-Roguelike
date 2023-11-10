using PlayerControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public StatSystem playerStats;
    Equipment[]  playerEquipment;

    public static PlayerManager playerManager;

    private void Awake()
    {
        if (playerManager == null)
        {
            Debug.Log("Awake called on"+gameObject.name);
            playerManager = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Start()
    {
        Debug.Log("Start called on Player Manager Script");
        playerStats = new StatSystem();
        int numSlots = System.Enum.GetNames(typeof(EquipSlot)).Length;
        playerEquipment = new Equipment[numSlots];
        playerStats.stats = new Dictionary<string, float>() { { "Health", 100 }, { "Mana", 500 }, { "AttackDamage", 12 }, { "Armor", 3 } };
        foreach(KeyValuePair<string,float> stat in playerStats.stats)
        {
            Debug.Log(stat.Key + ":" + stat.Value);
        }
    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.slot;
        if (playerEquipment[slotIndex] == null)
        {
            playerEquipment[slotIndex] = newItem;
            Debug.Log("Player Stats Before Picking Item:");
            Debug.Log("Damage: "+playerStats.GetStat("AttackDamage"));
            Debug.Log("Armor:" + playerStats.GetStat("Armor"));
            foreach(KeyValuePair<string,float> stat in newItem.statModifiers)
            {
                playerStats.AddStat(stat.Key,stat.Value);
            }
            //newItem.onUse();
            Debug.Log("Player Stats After Equip Item");
            Debug.Log("Damage: " + playerStats.GetStat("AttackDamage"));
            Debug.Log("Armor: " + playerStats.GetStat("Armor"));
        }
        else
        {
            Debug.Log("You have an item equipped this will be added to inventory");
        }
    }


}
