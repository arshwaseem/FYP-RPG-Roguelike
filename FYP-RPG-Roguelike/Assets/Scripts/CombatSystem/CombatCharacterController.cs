using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;


[System.Serializable]
public class CombatCharacterController : MonoBehaviour
{
    public Animator charAnimator;

    public CombatCharacterData characterData;
    public WaitUntil isDataReady;

    public Coroutine CharacterBaseLoop;
    public Coroutine attackQueue = null;
    public Coroutine enemyAttackBehavior;

    public CombatCharacterController cachedtarget;
    public AbilityData lastAbilityUsed;

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

    public void spawnParticleOnTarget()
    {

        if (lastAbilityUsed.particleFX != null)
        {
            GameObject tmpParticle = Instantiate(lastAbilityUsed.particleFX);
            tmpParticle.transform.position = cachedtarget.transform.position;

            ParticleSystem particleComponent = tmpParticle.GetComponent<ParticleSystem>();

            Destroy(tmpParticle, particleComponent.main.duration);
        }
        
    }

    public void AttackingTarger()
    {
        characterData.characterState = CharState.Attacking;
        characterData.targetData.SaveCharacterState();
        characterData.targetData.characterState = CharState.Attacked;
    }

    public void AttackedTarget()
    {
        characterData.characterState = CharState.Idle;
    }

    public void spawnParticleFX()
    {
        
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
        if (characterData.characterTeam == CharTeam.Enemy && characterData.isAlive)
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
                    if(characterData.isAlive && characterData.targetData.isAlive && characterData.isReadyForAction && characterData.targetData.isAttackable)
                {
                    AbilityData bestMove = minimax.ChooseBestMove();
                    yield return characterData.QueueAttack(bestMove);
                }


                yield return null;
            }

        }
    }
    public IEnumerator combatInitializer(){
        if (characterData.characterTeam == CharTeam.Friendly)
        {
            yield return new WaitUntil(PlayerManager.Instance.managerStarted);
            characterData.charName = PlayerManager.Instance.playerStats.name;
            characterData.maxHealth = PlayerManager.Instance.playerStats.maxHP;
            characterData.currHealth = PlayerManager.Instance.playerStats.currentHP;
            characterData.maxMana = PlayerManager.Instance.playerStats.maxMana;
            characterData.currMana = PlayerManager.Instance.playerStats.currentMana;
            characterData.armor = PlayerManager.Instance.playerStats.Armor;
            characterData.statusResist = PlayerManager.Instance.playerStats.statusResist;
            characterData.spellAmp = PlayerManager.Instance.playerStats.spellAmp;
            characterData.PlayerCont = this;
        }
        //yield return new WaitUntil(() => characterData.createdSuccessfully);
        characterData.Init();
        /*if (characterData.characterTeam == CharTeam.Friendly)
        {
            characterData.targetData = targetData.characterData;
        }*/
        CharacterBaseLoop = StartCoroutine(characterData.CharacterLoop());
        enemyAttackBehavior = StartCoroutine(AttackRandomFriendly());

    }

}
