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

[System.Serializable]
public class CombatCharacterData
{
    public CharacterUIData charUi = new CharacterUIData();
    public Dictionary<string, float> charStats;
    public AbilityData basicAttack;
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

    public bool isAlive
    {
        get
        {
            return characterState != CharState.Dead;
        }
    }

    public CombatCharacterData(string name, Dictionary<string, float> stats, CharTeam team)
    {
        this.charName = name;
        charStats = stats;
        maxHealth = charStats["TrueHealth"];
        currHealth = maxHealth;

        maxMana = charStats["TrueMana"];
        currMana = maxMana;

        limitPoints = 100f;
        currentLimit = 0;

        characterTeam = team;

        speedLimit = 5;
        createdSuccessfully = true;
    }

    public void SaveCharacterState()
    {
        savedState = characterState;
    }

    void onReadyDefault()
    {
        if(characterTeam == CharTeam.Friendly) { 
        foreach (var item in CombatUIManager.Instance.ActionWindow.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            item.interactable = true;
        }
            BattleManager.Instance.currentCharacter = this.PlayerCont;
        }
    }

    void OnAttackQueueDefault()
    {
        currSpeed = 0;
    }

    public void ResetActionsContainer()
    {
        foreach (var item in CombatUIManager.Instance.ActionWindow.GetComponentsInChildren<UnityEngine.UI.Button>())
        {
            item.interactable = false;
        }
    }

    public void Init()
    {
        if (characterTeam == CharTeam.Friendly)
        {
            charUi.InitUI((int)this.maxHealth, (int)this.currHealth, (int)this.maxMana, (int)this.currMana, (int)this.charStats["Strength"], (int)this.charStats["Intelligence"], (int)this.charStats["Tenacity"], limitPoints, currentLimit, speedLimit);
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
        if (!isAlive || characterState == CharState.TryingAttack)
        {
            Debug.Log("Character is dead or trying to attack, coroutine aborted.");
            PlayerCont.ClearAttackQueue();
            yield break;
        }
            

        characterState = CharState.TryingAttack;

        onAttackQueue.Invoke();

        yield return new WaitUntil(() => { Debug.Log("Inside Wait until: " + targetData.isAttackable); return targetData.isAttackable; });

        if (!targetData.isAlive)
        {
            Debug.Log("Target is dead, coroutine aborted.");
            yield break;
        }

        characterState = CharState.Attacking;

        targetData.SaveCharacterState();
        targetData.characterState = CharState.Attacked;

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
        if (currHealth <= 0)
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

                yield return new WaitUntil(()=> characterState != CharState.TryingAttack);

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

            //AnimEventsHere
            if(characterState != CharState.Dead)
            {
                characterState = CharState.Idle;
            }
            yield return new WaitUntil(() => characterState == CharState.Idle);
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
