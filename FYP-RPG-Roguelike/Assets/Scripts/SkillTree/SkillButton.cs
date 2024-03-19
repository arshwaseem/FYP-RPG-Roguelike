using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public Skill skill;
    public float skillValue;
    public string skillAffector;
    public List<SkillButton> children;
    public bool isUnlocked;
    public bool canBeUnlocked;


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = skill.skillImg;
        skillValue = skill.skillValue;
        skillAffector = skill.skillAffector;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void unlockSkill()
    {
        Debug.Log("SkillUnlockCalled");
        if (canBeUnlocked == true)
        {
            if (PlayerManager.Instance.playerStats.skillPoints >= skill.skillCost)
            {
                Debug.Log("Skill can be unlocked proceeding");
                if (!isUnlocked)
                {
                    var stats = PlayerManager.Instance.playerStats;

                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            child.canBeUnlocked = true;
                        }
                    }

                    isUnlocked = true;
                    PlayerManager.Instance.playerStats.skillPoints -= skill.skillCost;

                    if (skillAffector == "BaseHealth")
                    {
                        stats.maxHP -= stats.baseHP;
                        stats.baseHP += skillValue;
                        stats.maxHP += stats.baseHP;
                    }
                    if (skillAffector == "BaseMana")
                    {
                        stats.maxMana -=stats.baseMana;
                        stats.baseMana += skillValue;
                        stats.maxMana += stats.baseMana;
                    }
                    if (skillAffector == "Strength")
                    {
                        stats.Str += skillValue;
                        stats.maxHP = stats.baseHP + (stats.baseHP * stats.Str / 100);
                    }
                    if (skillAffector == "Intelligence")
                    {
                        stats.Int += skillValue;
                        stats.maxMana = stats.baseMana + (stats.baseMana * stats.Int / 100);
                    }
                    if (skillAffector == "Tenacity")
                    {
                        stats.Ten += skillValue;
                        stats.statusResist = stats.statusResist + (stats.statusResist * stats.Ten / 100);
                    }
                    if (skillAffector == "Strength" || skillAffector == "Intelligence" || skillAffector == "Tenacity")
                    {
                        if (HighestStat().Key == skillAffector)
                        {
                            stats.trueDamage = stats.baseDamage + (stats.baseDamage * (HighestStat().Value / 100));
                        }
                    }
                    if (skillAffector == "BaseAttackDamage")
                    {
                        stats.trueDamage -= stats.baseDamage;
                        stats.baseDamage = +skillValue;
                        stats.trueDamage += stats.baseDamage;
                    }
                    if (skillAffector == "BaseArmor")
                    {
                        stats.Armor += skillValue;
                    }
                }
            }
            else
            {
                GameObject AlertWindow = transform.root.Find("Alert").gameObject;
                string defText = AlertWindow.GetComponent<TextMeshPro>().text;
                AlertWindow.GetComponent<TextMeshPro>().SetText("Not Enough Skill Points");
                AlertWindow.SetActive(true);
                Invoke("hideAlert", 3f);
                GameObject.Find("Alert").GetComponent<TextMeshPro>().text = defText;
            }
        }
        else
        {
            Debug.Log("Skill cannot be unlocked so showing error");
            GameObject AlertWindow = transform.root.Find("Alert").gameObject;
            AlertWindow.SetActive(true);
            Invoke("hideAlert", 3f);
        }

    }

    public KeyValuePair<string,float> HighestStat()
    {
        var stats = new Dictionary<string, float> { {"Strength", PlayerManager.Instance.playerStats.Str }, { "Intelligence", PlayerManager.Instance.playerStats.Int },
        {"Tenacity", PlayerManager.Instance.playerStats.Ten }};
        return stats.OrderByDescending(kv => kv.Value).First();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("mouse entered on skill");
        Transform skillInfoTransform = transform.root.Find("InfoSkill");

        if (skillInfoTransform != null)
        {
            // Access the TextMeshPro components within SkillInfo
            TextMeshProUGUI text1 = skillInfoTransform.Find("SkillName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI text2 = skillInfoTransform.Find("SkillDesc").GetComponent<TextMeshProUGUI>();

            // Now you can use text1 and text2 as needed
            if (text1 != null && text2 != null)
            {
                text1.text = skill.skillName;
                text2.text = skill.skillDesc;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Transform skillInfoTransform = transform.root.Find("InfoSkill");

        if (skillInfoTransform != null)
        {
            // Access the TextMeshPro components within SkillInfo
            TextMeshProUGUI text1 = skillInfoTransform.Find("SkillName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI text2 = skillInfoTransform.Find("SkillDesc").GetComponent<TextMeshProUGUI>();

            // Now you can use text1 and text2 as needed
            if (text1 != null && text2 != null)
            {
                text1.text = "Skill Name";
                text2.text = "Description";
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        unlockSkill();
        if (isUnlocked)
        {
            GetComponent<Image>().sprite = skill.skillEnable;
            Debug.Log("Unlocked Skill " + skill.skillName);
        }

    }

    public void hideAlert()
    {
        GameObject AlertWindow = transform.root.Find("Alert").gameObject;
        AlertWindow.SetActive(false);
    }
}
