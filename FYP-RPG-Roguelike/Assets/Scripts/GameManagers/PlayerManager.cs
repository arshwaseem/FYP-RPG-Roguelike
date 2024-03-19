using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager Instance;
    public bool managerRunning;
    public string PlayerName = "Cal Kestis";
    public Stats playerStats = new Stats();
    public List<GameObject> EnemiesInTrigger = new List<GameObject>();
    public float xpEarned;
    public bool justFinishedFight = false;
    public string previousScene;
    public Vector3 positionInPrevScene;
    public static GameObject lastFightTrigger { get; set; }
    public string deftext;
    public static GameObject RoamingUI;
    public static GameObject CombatUI;
    public List<Item> Inventory;
    public bool isInteractRange;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerStats.name = PlayerName;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public void fightEnded()
    {
        if (justFinishedFight)
        {
            if (xpEarned != null && xpEarned > 0)
            {
                addExp(xpEarned);
            }
            justFinishedFight = false;
        }
    }

    private void Start()
    {
        managerRunning = true;
    }

    public bool managerStarted()
    {
        return managerRunning;
    }

    public void LvlUp()
    {
        playerStats.playerLvl++;
        playerStats.currentXP = 0;
        playerStats.xpThreshHold = playerStats.lvlthresholds[playerStats.playerLvl];
        playerStats.skillPoints++;
        Invoke("hideAlert", 3f);
    }

    public void addExp(float earnedXP)
    {
        if ((playerStats.currentXP + earnedXP) > playerStats.xpThreshHold)
        {
            float leftoverXP = playerStats.currentXP - playerStats.xpThreshHold;
            LvlUp();
            addExp(leftoverXP);
        }
        else
        {
            playerStats.currentXP += earnedXP;
        }
    }

    public void AssignEnemGraphics()
    {
        var Enems = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < Enems.Length; i++)
        {
            Enems[i] = EnemiesInTrigger.ElementAt(i);
        }
    }

}

public class Stats
{
    public Dictionary<int, int> lvlthresholds = new Dictionary<int, int> { { 1, 100 }, { 2, 250 }, { 3, 400 }, { 4, 600 }, { 5, 850 }, { 6, 1150 }, { 7, 1500 }, { 8, 1900 }, { 9, 2350 }, { 10, 2850 } };
    public float maxHP = 100;
    public float currentHP = 100;
    public float baseHP = 10;
    public float maxMana = 100;
    public float currentMana = 50;
    public float baseMana = 10;
    public float xpThreshHold = 10;
    public float currentXP = 0;
    public int playerLvl = 1;
    public float Str = 3;
    public float Int = 3;
    public float Ten = 3;
    public float Armor = 2;
    public float statusResist = 3;
    public float spellAmp = 3;
    public string name = "Cal Kestis";
    public int skillPoints = 0;
    public float baseDamage = 4;
    public float trueDamage = 4;
}
