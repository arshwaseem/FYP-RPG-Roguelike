using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.AI;

[System.Serializable]
public class CombatCharacterData
{
    public CharacterUIData charUi = new CharacterUIData();
    public AbilityData basicAttack;
    public AbilityData limitBurst;
    public List<AbilityData> charAbilities;

    public string charName;

    public float maxHealth;
    public float currHealth;

    public float maxMana;
    public float currMana;

    public float limitPoints;
    public float currentLimit;

    public float speedLimit = 5;
    public float currSpeed = 0;

    public float armor;

    public float xpDrop;

    public float statusResist;

    public float spellAmp;

    public CharState characterState;
    public CharTeam characterTeam;
    CharState savedState;

    public CombatCharacterData targetData;
    public CombatCharacterController PlayerCont;

    

    public UnityEvent onAttack;
    public UnityEvent onAttackQueue;
    public UnityEvent onWasAttacked;
    public UnityEvent onJustReady;
    public UnityEvent onCharacterDied = new UnityEvent();

    public bool playerjustAttacked;

    public bool createdSuccessfully;

    public bool initStatus;
    public bool uiStatus;

    public bool isSimulated;

    public bool CanQueueAttack
    {
        get
        {
            return characterState == CharState.TryingAttack || PlayerCont.attackQueue == null;
        }
    }
    public bool isAttackable
    {
        get
        {
            return characterState == CharState.Idle || characterState == CharState.Ready;
        }
    }

    public bool isReadyForAction
    {
        get
        {
            return currSpeed >= speedLimit;
        }
    }

    public bool isReadyForLB
    {
        get
        {
            return (currentLimit >= limitPoints);
        }
    }

    public bool isAlive
    {
        get
        {
            return characterState != CharState.Dead;
        }
    }

    public void SaveCharacterState()
    {
        savedState = characterState;
    }

    void onReadyDefault()
    {
        SelectCharacter();
    }

    void OnAttackQueueDefault()
    {
        currSpeed = 0;

        if(characterTeam == CharTeam.Friendly)
        charUi.UpdateTimeBar(currSpeed);
    }

    public void ResetActionsContainer()
    {
        foreach (var item in CombatUIManager.Instance.ActionWindow.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            item.interactable = false;
        }
    }

    public void SelectCharacter()
    {
        if (!isReadyForAction)
            return;

        CombatUIManager.Instance.ActionWindow.SetActive(true);
        foreach (var item in CombatUIManager.Instance.ActionWindow.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            item.interactable = true;
        }

        foreach (var item in GameObject.FindObjectsOfType<CombatCharacterController>())
        {
            if (item.characterData.characterTeam == CharTeam.Enemy)
                item.characterData.ResetUINameText();
        }
        BattleManager.Instance.currentCharacter = PlayerCont;
    }



    public void ResetUINameText()
    {
        return;
    }

    public void Init()
    {
        if (characterTeam == CharTeam.Friendly)
        {
            charUi.InitUI((int)this.maxHealth, (int)this.currHealth, (int)this.maxMana, (int)this.currMana, (int)PlayerManager.Instance.playerStats.Str, (int)PlayerManager.Instance.playerStats.Int, (int)PlayerManager.Instance.playerStats.Ten, limitPoints, currentLimit, speedLimit);
            ResetActionsContainer();
            onJustReady.AddListener(onReadyDefault);
        }
        onAttack.AddListener(characterAttackDefault);
        onWasAttacked.AddListener(characterAttackedDefault);
        onAttackQueue.AddListener(OnAttackQueueDefault);
        onCharacterDied.AddListener(OnDeathDefault);

        initStatus = true;

        characterState = CharState.Idle;
    }

    public void characterAttackDefault()
    {
        currSpeed = 0f;
    }

    public void characterAttackedDefault()
    {
        //Debug.Log(charName + " was attacked");
        if(characterState != CharState.Dead)
        {
            characterState = savedState;
        }
    }

    public void OnDeathDefault()
    {
        BattleManager.Instance.checkEncounterStatus();
        characterState = CharState.Dead;
    }

    public void increaseLimit(float amount)
    {
        if (characterTeam == CharTeam.Friendly)
        {
            currentLimit += amount;
            currentLimit = Mathf.Clamp(currentLimit, 0, limitPoints);
            charUi.UpdateLimitBar(currentLimit);
        }
    }


    public IEnumerator QueueAttack(AbilityData atk)
    {


        if (!isAlive || characterState==CharState.TryingAttack || characterState == CharState.Finished || targetData == null)
        {
            PlayerCont.ClearAttackQueue();
            yield break;
        }
            

        characterState = CharState.TryingAttack;

        onAttackQueue.Invoke();

        yield return new WaitUntil(() => targetData.isAttackable);

        if (!targetData.isAlive)
            yield break;


        PlayerCont.cachedtarget = targetData.PlayerCont;
        PlayerCont.lastAbilityUsed = atk;

        if (atk != limitBurst)
        {
            PlayerCont.AttackAnimation();
        }
        else
        {
            PlayerCont.LimitBurstAnimation();
        }

        //spawnParticleFX(atk, targetData.PlayerCont);
        yield return new WaitUntil(() => characterState == CharState.Attacking);

        
        if (CombatUIManager.Instance != null)
        {
            CombatUIManager.Instance.setAbilityText(atk.ability_name, characterTeam);
        }
        else
        {
            Debug.LogError("CMUI is null");
        }

        Debug.Log("Attacked with " + atk.ability_name + " to " + targetData.charName);

        //TempAttack
        switch (atk.output)
        {
            case AbilityOutput.Damage:
                targetData.Damage(atk.abilityValue);
                this.currMana = this.currMana - atk.manaCost;
                break;
            case AbilityOutput.Heal:
                Heal(atk.abilityValue);
                this.currMana = this.currMana - atk.manaCost;
                break;
        }
        if (characterTeam == CharTeam.Friendly)
        {
            increaseLimit(5);
        }
        PlayerCont.ClearAttackQueue();
    }

    public void Heal(int healAmount)
    {
        currHealth = Mathf.Clamp(currHealth + healAmount, 0, maxHealth);
        if (characterTeam == CharTeam.Friendly)
        {
            charUi.UpdateHealthBar(currHealth, maxHealth);
        }
    }

    public void Damage(int damageAmount)
    {
        currHealth -= damageAmount;
        if (currHealth <= 0 && characterState != CharState.Dead)
        {
            currHealth = 0;
            characterState = CharState.Dead;
            onCharacterDied.Invoke();
        }
        if (characterTeam == CharTeam.Friendly)
        {
            increaseLimit(5);
            charUi.UpdateHealthBar(currHealth, maxHealth);
        }
        onWasAttacked.Invoke();
    }

    public IEnumerator CharacterLoop()
    {
        while (characterState != CharState.Dead)
        {
            while (currSpeed < speedLimit)
            {

                yield return new WaitUntil(()=> characterState != CharState.TryingAttack && characterState != CharState.Attacking);

                currSpeed += Time.deltaTime;
                if (characterTeam == CharTeam.Friendly)
                {
                    charUi.UpdateTimeBar(currSpeed);
                }
                if (characterState != CharState.Attacked || characterState != CharState.Dead)
                {
                    characterState = CharState.Idle;
                }

                yield return null;
            }

            currSpeed = speedLimit;
            characterState = CharState.Ready;
            onJustReady.Invoke();

            yield return new WaitUntil(() => characterState == CharState.Attacking);
            yield return new WaitUntil(() => characterState == CharState.Idle);
            if(characterTeam == CharTeam.Friendly)
            ResetActionsContainer();
        }
    }

    [System.Serializable]
    public class CharacterUIData
    {

        public InformationUI playerCombatUI;

        public void InitUI(int maxHealth, int currHealth, int maxMana, int currMana, int Str, int Int, int Ten, float maxLim, float currLim, float speedLim)
        {

            playerCombatUI = CombatUIManager.Instance.defaultUI;
            //Health
            playerCombatUI.healthSlider.maxValue = maxHealth;
            UpdateHealthBar(currHealth, maxHealth);
            //Mana
            playerCombatUI.manaSlider.maxValue = maxMana;
            UpdateManaBar(currMana);
            //Stats
            playerCombatUI.Strength.GetComponent<TextMeshProUGUI>().text = "Str : " + Str.ToString();
            playerCombatUI.Inteliigence.GetComponent<TextMeshProUGUI>().text = "Int : " + Int.ToString();
            playerCombatUI.Tenacity.GetComponent<TextMeshProUGUI>().text = "Ten : " + Ten.ToString();
            //Equipment

            //Limit
            playerCombatUI.limitSlider.maxValue = maxLim;
            playerCombatUI.limitSlider.minValue = 0;
            playerCombatUI.limitSlider.value = 0;
            //Time
            playerCombatUI.timeSlider.maxValue = speedLim;
            playerCombatUI.timeSlider.value = 0;

        }

        public void UpdateTimeBar(float currProg)
        {
            playerCombatUI.timeSlider.value = currProg;
        }

        public void UpdateLimitBar(float currProg)
        {
            playerCombatUI.limitSlider.value = currProg;
        }

        public void UpdateHealthBar(float currAmount, float maxAmount)
        {
            playerCombatUI.healthSlider.value = currAmount;
            playerCombatUI.healthText.GetComponent<TextMeshProUGUI>().text = (currAmount.ToString() + "/" + maxAmount.ToString());
        }

        public void UpdateManaBar(float currAmount)
        {
            playerCombatUI.manaSlider.value = currAmount;
            playerCombatUI.manaText.GetComponent<TextMeshProUGUI>().text = currAmount.ToString();
        }
    }
}
    public enum CharTeam
    {
        Friendly,
        Enemy
    }
    public enum CharState
    {
        Idle,
        Attacking,
        Attacked,
        Dead,
        Ready,
        Loading,
        TryingAttack,
        Finished
    }
