using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public CombatCharacterController currentCharacter;

    public static BattleManager Instance;

    public List<CombatCharacterController> FriendlyCharacters = new List<CombatCharacterController>();

    public List<CombatCharacterController> EnemyCharacters = new List<CombatCharacterController>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        FriendlyCharacters = FindObjectsOfType<CombatCharacterController>().ToList().FindAll(x => x.characterData.characterTeam == CharTeam.Friendly);
        EnemyCharacters = FindObjectsOfType<CombatCharacterController>().ToList().FindAll(x => x.characterData.characterTeam == CharTeam.Enemy);
    }

    public void DoBasicAttack()
    {
        if (currentCharacter.characterData.isReadyForAction)
        {
            //Debug.Log("Is Ready for Action");
            if (currentCharacter.characterData.characterTeam == CharTeam.Friendly)
            {
                Debug.Log("Player performed an action");
                if (currentCharacter.characterData.targetData.isAttackable)
                {
                    if (currentCharacter.attackQueue == null)
                        currentCharacter.attackQueue = StartCoroutine(currentCharacter.characterData.QueueAttack(currentCharacter.characterData.basicAttack));
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
            Debug.Log("Victory");
            stopAllChars();
        }

        if(!isFriendlyAlive && isEnemyAlive)
        {
            Debug.Log("Defeat");
            stopAllChars();
        }
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

}
