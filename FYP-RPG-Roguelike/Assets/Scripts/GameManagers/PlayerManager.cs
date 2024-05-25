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
    public playerState _playerState;
    public static PlayerManager Instance;
    public bool managerRunning;
    public string PlayerName = "Cal Kestis";
    public Stats playerStats = new Stats();
    public List<GameObject> EnemiesInTrigger = new List<GameObject>();
    public float xpEarned=0;
    public bool justFinishedFight = false;
    public string previousScene;
    public Vector3 positionInPrevScene;
    public string deftext;
    public static GameObject RoamingUI;
    public static GameObject CombatUI;
    public List<Item> Inventory;
    public bool isInteractRange;
    public Scene _activelevel;
    public TriggerCombat currentTrigger;

    public void ChangePlayerState(playerState newState)
    {
        switch (newState)
        {
            case playerState.Explore:
                break;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            playerStats.name = PlayerName;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        managerRunning = true;
        _activelevel = SceneManager.GetActiveScene();
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
        RoamUIManager.Instance.showAlertWindow("You have leveled up to Level "+playerStats.playerLvl);
    }

    public void addExp()
    {
        Debug.Log("Player Has Earend XP " + this.xpEarned);
        if ((this.playerStats.currentXP + this.xpEarned) > this.playerStats.xpThreshHold)
        {
            float leftoverXP = this.playerStats.currentXP - this.playerStats.xpThreshHold;
            LvlUp();
            addExp();
        }
        else
        {
            this.playerStats.currentXP += this.xpEarned;
            RoamUIManager.Instance.showAlertWindow("Earned "+this.xpEarned+" XP, You need " + (this.playerStats.lvlthresholds[this.playerStats.playerLvl] - (int)this.xpEarned) + " XP to level up");
            this.xpEarned = 0;
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
    public float maxHP = 1000f;
    public float currentHP = 1000f;
    public float baseHP = 10f;
    public float maxMana = 100f;
    public float currentMana = 50f;
    public float baseMana = 10f;
    public float xpThreshHold = 10;
    public float currentXP = 0f;
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

public enum playerState {Explore,inBattle,wonBattle,lostBattle}
