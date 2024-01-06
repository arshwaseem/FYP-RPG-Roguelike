using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CombatCharacterController : MonoBehaviour
{
    public Animator charAnimator;

    public CombatCharacterData characterData;
    public WaitUntil isDataReady;

    public Coroutine CharacterBaseLoop;
    public Coroutine attackQueue = null;
    public Coroutine enemyAttackBehavior;

    Minimax minimax;

    public void Awake()
    {
        characterData.PlayerCont = this;
        charAnimator = GetComponent<Animator>();
    }
    public void Start()
    {
        minimax = new Minimax(this);
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

    public void OnMouseDown()
    {
        if (characterData.characterTeam == CharTeam.Friendly)
            return;

        Debug.Log(gameObject.name + "was clicked");
        BattleManager.Instance.SelectCharacterTarget(characterData);
    }

    public void AttackAnimation()
    {
        charAnimator.Play("Attack",0);
    }

    public void LimitBurstAnimation()
    {
        charAnimator.Play("Limit", 0);
    }

    public void spawnParticleOnTarget(CombatCharacterController charCont, GameObject particle)
    {
        GameObject tmpParticle = Instantiate(particle);
        tmpParticle.transform.position = charCont.transform.position;

        ParticleSystem particleComponent = tmpParticle.GetComponent<ParticleSystem>();

        Destroy(tmpParticle, particleComponent.main.duration);
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
        Debug.Log("enemy behavior started");
        if (characterData.characterTeam == CharTeam.Enemy)
        {
            Debug.Log("Enemy " + characterData.charName + " Time is " + characterData.currSpeed);
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


                AbilityData bestMove = minimax.ChooseBestMove();
                yield return characterData.QueueAttack(bestMove);

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
        /*if (characterData.characterTeam == CharTeam.Friendly)
        {
            characterData.targetData = targetData.characterData;
        }*/
        CharacterBaseLoop = StartCoroutine(characterData.CharacterLoop());
        enemyAttackBehavior = StartCoroutine(AttackRandomFriendly());

    }

}
