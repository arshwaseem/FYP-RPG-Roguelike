using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager Instance;
    public StatManagementSystem playerStatSystem;
    public Dictionary<string, float> statList;
    public Equipment[] playerEquipment;
    public bool managerRunning;
    public string PlayerName = "Cal Kestis";

    private void Awake()
    {
        Instance = this;
        playerEquipment = new Equipment[System.Enum.GetValues(typeof(Equipment.EquipSlot)).Length];
        playerStatSystem = ScriptableObject.CreateInstance<StatManagementSystem>();
    }
    #endregion

    private void Start()
    {
        InitializePlayerCharacter();
        playerStatSystem.statList = statList;
        Debug.Log("Player Stats");
        foreach(KeyValuePair<string,float> stat in  statList)
        {
            Debug.Log(stat.Key+":"+stat.Value);
        }
        managerRunning = true;
    }

    public void InitializePlayerCharacter()
    {
        statList = new Dictionary<string, float> { { "BaseHealth", 100 }, {"BaseMana", 100 }, { "Strength", 3 }, 
            {"Intelligence",1 }, {"Tenacity", 4 }, {"BaseStatusResist",2 }, {"BaseArmor", 3}, {"BaseAttackDamage", 15}};

        var add_stats = new Dictionary<string, float> {{"TrueHealth", statList["BaseHealth"] + (statList["BaseHealth"]*statList["Strength"]/100) },
            {"TrueMana", statList["BaseMana"] + (statList["BaseMana"]*statList["Intelligence"]/100) },
            { "TrueAttackDamage", statList["BaseAttackDamage"] + (statList["BaseAttackDamage"]*getHighestStat(statList)/100) },
            {"TrueStatusResist", statList["BaseStatusResist"]+(statList["BaseStatusResist"]*statList["Tenacity"]/100) },
            {"TrueArmor", statList["BaseArmor"] } };

        foreach(KeyValuePair<string,float> stat in add_stats)
        {
            statList.Add(stat.Key, stat.Value);
        }
    }

    public float getHighestStat(Dictionary<string, float> statList)
    {
        return new float[3] { statList["Strength"], statList["Intelligence"], statList["Tenacity"] }.Max();
    }

    public bool managerStarted()
    {
        return managerRunning;
    }

}
