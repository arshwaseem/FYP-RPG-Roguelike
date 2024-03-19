using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatUIManager : MonoBehaviour
{
    public static CombatUIManager Instance;

    public InformationUI defaultUI;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject ActionWindow;

    [Header("MagicWindowObjects")]
    public GameObject magicWindow;
    public GameObject magicContainer;
    public GameObject magicPrefab;
    public GameObject magicDescWindow;

    [Header("Ability Texts")]
    public GameObject AbilityText;
    public GameObject EnemyAbilityText;
    

    public void FillMagicWindow()
    {
        ClearMagicWindow();

        var data = BattleManager.Instance.currentCharacter.characterData.charAbilities;

        for (int i = 0; i < data.Count; i++)
        {
            GameObject tempMagicPrefab = Instantiate(magicPrefab);
            tempMagicPrefab.transform.SetParent(magicWindow.transform, false);
            MagicUI tempUI = tempMagicPrefab.GetComponent<MagicUI>();
            tempUI.abilityIndex = i;
            tempUI.Init(data[i].ability_name);
        }
    }

    public void setManaNeededUI(int manaCost, int currMana, string name, string desc)
    {
        var _Texts = magicDescWindow.GetComponentsInChildren<TextMeshProUGUI>();
        _Texts[0].text = name;
        _Texts[1].text = desc;
        _Texts[2].text = manaCost + "/" + currMana;
    }

    public void clearManaNeededUI()
    {
        foreach(var item in magicDescWindow.GetComponentsInChildren<TextMeshProUGUI>())
        {
            item.text = "Select Ability";
        }
    }

    public void HideMagicUI()
    {
        magicContainer.SetActive(false);
    }

    public void setAbilityText(string abName, CharTeam team)
    {
        switch (team)
        {
            case CharTeam.Friendly:
                AbilityText.GetComponent<TextMeshProUGUI>().SetText(abName);
                break;
            case CharTeam.Enemy:
                EnemyAbilityText.GetComponent<TextMeshProUGUI>().SetText(abName);
                break;
        }
    }

    public void ClearMagicWindow()
    {
        foreach(Transform item in magicWindow.transform)
        {
            Destroy(item.gameObject);
        }
    }
}