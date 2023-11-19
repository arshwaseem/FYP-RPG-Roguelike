using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int abilityIndex = 0;
    public TextMeshProUGUI abTextMesh;

    public void Init(string abilityName)
    {
        abTextMesh.text = abilityName;
    }

    public IEnumerator OnPointerDownCoroutine(AbilityData ability)
    {
        var temp2 = BattleManager.Instance.currentCharacter;
        if (temp2.attackQueue == null)
        {
            temp2.attackQueue = StartCoroutine(temp2.characterData.QueueAttack(ability));
            yield return temp2.attackQueue;  // Wait for the QueueAttack coroutine to finish
        }

        CombatUIManager.Instance.HideMagicUI();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        var temp = BattleManager.Instance.currentCharacter.characterData.charAbilities;



        for (int i = 0; i < temp.Count; i++)
        {
            if (abilityIndex == i)
            {
                StartCoroutine(OnPointerDownCoroutine(temp[i]));

            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var charData = BattleManager.Instance.currentCharacter.characterData;
        for (int i = 0; i < charData.charAbilities.Count; i++)
        {
            if (abilityIndex == i)
            {
                CombatUIManager.Instance.setManaNeededUI(charData.charAbilities[i].manaCost, (int)charData.currMana, charData.charAbilities[i].ability_name, charData.charAbilities[i].ability_description);
            }
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CombatUIManager.Instance.clearManaNeededUI();
    }
}
