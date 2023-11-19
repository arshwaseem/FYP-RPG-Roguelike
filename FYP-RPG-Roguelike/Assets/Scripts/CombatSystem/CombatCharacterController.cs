using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CombatCharacterController : MonoBehaviour
{
    public CombatCharacterData characterData;
    public CombatCharacterController targetData;
    public WaitUntil isDataReady;

    public Coroutine CharacterBaseLoop;
    public Coroutine attackQueue = null;
    public Coroutine enemyAttackBehavior;

    public void Start()
    {
        StartCoroutine(combatInitializer());
    }

    public void ClearAttackQueue()
    {
        if (attackQueue != null)
        {
            StopCoroutine(attackQueue);
            attackQueue = null;
        }
    }

    public void StopAll()
    {
        if (CharacterBaseLoop != null)
        {
            StopCoroutine(CharacterBaseLoop);
        }
        if (attackQueue != null)
        {
            StopCoroutine(attackQueue);
        }
        if (enemyAttackBehavior != null)
        {
            StopCoroutine(enemyAttackBehavior);
        }
        CharacterBaseLoop = null;
        attackQueue = null;
        enemyAttackBehavior = null;
        characterData.characterState = CharState.Finished;
    }

    public IEnumerator AttackRandomFriendly()
    {
        if (characterData.characterTeam == CharTeam.Enemy)
        {
            while (characterData.isAlive)
            {

                yield return new WaitUntil(() => characterData.isReadyForAction);


                    while (characterData.targetData == null || characterData.targetData.characterState == CharState.Dead)
                    {
                        if (BattleManager.Instance.isFriendlyAlive)
                        {
                            characterData.targetData = BattleManager.Instance.RandomFriendly.characterData;
                            yield return null;
                        }
                        else
                        {
                            yield break;
                        }

                    yield return null;
                    }



                yield return characterData.QueueAttack(characterData.basicAttack);

                yield return null;
            }

        }
    }
    public IEnumerator combatInitializer()
    {
        if (characterData.characterTeam == CharTeam.Friendly)
        {
            yield return new WaitUntil(PlayerManager.Instance.managerStarted);
            characterData = new CombatCharacterData(PlayerManager.Instance.PlayerName, PlayerManager.Instance.statList, CharTeam.Friendly);
            characterData.PlayerCont = this;
        }
        yield return new WaitUntil(() => characterData.createdSuccessfully);
        characterData.Init();
        if (characterData.characterTeam == CharTeam.Friendly)
        {
            characterData.targetData = targetData.characterData;
        }
        CharacterBaseLoop = StartCoroutine(characterData.CharacterLoop());
        enemyAttackBehavior = StartCoroutine(AttackRandomFriendly());

    }

}
