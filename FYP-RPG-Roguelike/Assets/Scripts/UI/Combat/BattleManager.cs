using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using DungeonArchitect.Samples.ShooterGame;

public class BattleManager : MonoBehaviour
{
    public CombatCharacterController currentCharacter;

    public static BattleManager Instance;

    public List<CombatCharacterController> FriendlyCharacters = new List<CombatCharacterController>();

    public List<CombatCharacterController> EnemyCharacters = new List<CombatCharacterController>();

    public GameObject selectorGraphic;

    public TriggerCombat Trigger;

    private void Awake()
    {
        Instance = this;
        Trigger = PlayerManager.Instance.currentTrigger;
    }

    private void Start()
    {
        StartCoroutine(Init());
    }

    public IEnumerator Init()
    {
        yield return new WaitUntil(() => Trigger.spawnEnemies() == true);
        FriendlyCharacters = FindObjectsOfType<CombatCharacterController>().ToList().FindAll(x => x.characterData.characterTeam == CharTeam.Friendly);
        EnemyCharacters = FindObjectsOfType<CombatCharacterController>().ToList().FindAll(x => x.characterData.characterTeam == CharTeam.Enemy);
        Debug.Log("Enemy " + EnemyCharacters.First().characterData.charName + " was found");
        Time.timeScale = 1f;
        yield return null;
    }

    public void SelectCharacter (CombatCharacterData newChar)
    {
        setTargetGraphics(currentCharacter);
    }

    public void SelectCharacterTarget(CombatCharacterData tgt)
    {
        if(currentCharacter != null)
        {
            if (currentCharacter.characterData.isReadyForAction)
            {
                currentCharacter.characterData.targetData = tgt;
                setTargetGraphics(currentCharacter);
            }
        }
    }

    public void setTargetGraphics(CombatCharacterController chC)
    {
        if(chC == null)
        {
            return;
        }

        if(chC.characterData.targetData == null)
        {
            SelectorStatus(false);
        }
        else
        {
            SelectorStatus(true);
            var _chartgt = chC.characterData.targetData.PlayerCont;
            selectorGraphic.transform.position = new Vector3(_chartgt.transform.position.x, _chartgt.transform.position.y + 2, _chartgt.transform.position.z);
        }
    }

    public void SelectorStatus(bool status)
    {
        selectorGraphic.SetActive(status);
    }

    public void DoBasicAttack()
    {
        if (currentCharacter.characterData.isReadyForAction)
        {
            //Debug.Log("Is Ready for Action");
            if (currentCharacter.characterData.characterTeam == CharTeam.Friendly)
            {
                if(currentCharacter.characterData.targetData != null)
                {
                    Debug.Log("Plauyer did Action");
                    if (currentCharacter.characterData.CanQueueAttack)
                        currentCharacter.attackQueue = StartCoroutine(currentCharacter.characterData.QueueAttack(currentCharacter.characterData.basicAttack));
                }
                
            }
        }
        
    }

    public void DoLimitBurst()
    {
        if (currentCharacter.characterData.isReadyForLB)
        {
            //Debug.Log("Is Ready for Action");
            if (currentCharacter.characterData.characterTeam == CharTeam.Friendly)
            {
                if (currentCharacter.characterData.targetData != null)
                {
                    Debug.Log("Player performed an action");
                    if (currentCharacter.characterData.CanQueueAttack)
                    {
                        currentCharacter.attackQueue = StartCoroutine(currentCharacter.characterData.QueueAttack(currentCharacter.characterData.limitBurst));
                        currentCharacter.characterData.currentLimit = 0;
                        currentCharacter.characterData.charUi.UpdateLimitBar(0);
                    }
                }

            }
        }

    }

    public CombatCharacterController RandomFriendly
    {
        get
        {
            return FriendlyCharacters[Random.Range(0, FriendlyCharacters.Count)];
        }
    }

    public void checkEncounterStatus()
    {
        if(isFriendlyAlive && !isEnemyAlive)
        {
            stopAllChars();
            Debug.Log("Victory");
            StartCoroutine(endBattleVictory());
        }

        if(!isFriendlyAlive && isEnemyAlive)
        {
            stopAllChars();
            Debug.Log("Defeat");
            endBattleDefeat();
            unloadCS();
        }
    }

    public IEnumerator endBattleVictory()
    {
        RoamUIManager.Instance.showAlertWindow("You are victorious");
        yield return new WaitForSeconds(5);
        foreach (CombatCharacterController enemy in EnemyCharacters)
        {
            PlayerManager.Instance.xpEarned += enemy.characterData.xpDrop;
        }
        Debug.Log("You Earned " + PlayerManager.Instance.xpEarned);
        StartCoroutine(unloadCS());
        PlayerManager.Instance.addExp();
        yield return null;
    }

    public IEnumerator endBattleDefeat()
    {
        RoamUIManager.Instance.showAlertWindow("You have been defeated");
        yield return new WaitForSeconds(5);
        foreach (CombatCharacterController enemy in EnemyCharacters)
        {
            if(enemy.characterData.characterState == CharState.Dead)
            {
                PlayerManager.Instance.xpEarned += enemy.characterData.xpDrop;
            }    
        }
        Debug.Log("You Earned " + PlayerManager.Instance.xpEarned);
        PlayerManager.Instance.addExp();
        yield return null;
    }

    public IEnumerator unloadCS()
    {
        Trigger.ReactivateRoamObjects();
        Trigger.gameObject.SetActive(false);
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject en in Enemies)
        {
            Destroy(en);
        }
        AsyncOperation _unloadCS = SceneManager.UnloadSceneAsync("CombatScene", UnloadSceneOptions.None);
        yield return new WaitUntil(() => _unloadCS.isDone);

        
    }

    public void stopAllChars()
    {
        for (int i = 0; i < FriendlyCharacters.Count; i++)
        {
            FriendlyCharacters[i].StopAll();
        }
        for (int i = 0; i < EnemyCharacters.Count; i++)
        {
            EnemyCharacters[i].StopAll();
        }
    }

    public bool isFriendlyAlive
    {
        get
        {
            bool isFriendlyAlive = false;

            for (int i = 0; i < FriendlyCharacters.Count; i++)
            {
                if (FriendlyCharacters[i].characterData.isAlive)
                {
                    isFriendlyAlive = true;
                }
            }

            return isFriendlyAlive;
        }
    }

    public bool isEnemyAlive
    {
        get
        {
            bool enemyAlive = false;
            for (int i = 0; i < EnemyCharacters.Count; i++)
            {
                if (EnemyCharacters[i].characterData.isAlive)
                {
                    enemyAlive = true;
                }
            }

            return enemyAlive;
        }
    }

    public void DisplayAlertWindow(string result)
    {
        GameObject AlertP = GameObject.FindGameObjectWithTag("CombALC");
        AlertP.GetComponentInChildren<TextMeshProUGUI>().SetText(result);
        AlertP.SetActive(true);
    }
    public void hideAlertWindow()
    {
        GameObject AlertWindow = GameObject.FindGameObjectWithTag("CombALC"); ;
        AlertWindow.SetActive(false);
    }

}
