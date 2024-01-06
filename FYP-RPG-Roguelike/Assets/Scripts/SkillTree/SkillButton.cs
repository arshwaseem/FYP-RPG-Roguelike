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
            Debug.Log("Skill can be unlocked proceeding");
            if (!isUnlocked)
            {
                var stats = PlayerManager.Instance.statList;

                if (children != null)
                {
                    foreach (var child in children)
                    {
                        child.canBeUnlocked = true;
                    }
                }

                isUnlocked = true;

                if (skillAffector == "BaseHealth")
                {
                    stats["TrueHealth"] -= stats["BaseHealth"];
                    stats["BaseHealth"] += skillValue;
                    stats["TrueHealth"] += stats["BaseHealth"];
                }
                if (skillAffector == "BaseMana")
                {
                    stats["TrueMana"] += skillValue;
                }
                if (skillAffector == "Strength")
                {
                    stats[skillAffector] += skillValue;
                    stats["TrueHealth"] = stats["BaseHealth"] + (stats["BaseHealth"] * stats["Strength"] / 100);
                }
                if (skillAffector == "Intelligence")
                {
                    stats[skillAffector] += skillValue;
                    stats["TrueMana"] = stats["BaseMana"] + (stats["BaseMana"] * stats["Strength"] / 100);
                }
                if (skillAffector == "Tenacity")
                {
                    stats[skillAffector] += skillValue;
                    stats["TrueStatusResist"] = stats["BaseStatusResist"] + (stats["BaseStatusResist"] * stats["Tenacity"] / 100);
                }
                if (skillAffector == "Strength" || skillAffector == "Intelligence" || skillAffector == "Tenacity")
                {
                    if (HighestStat().Key == skillAffector)
                    {
                        stats["TrueAttackDamage"] = stats["BaseAttackDamage"] + (stats["BaseAttackDamage"] * (HighestStat().Value / 100));
                    }
                }
                if (skillAffector == "BaseAttackDamage")
                {
                    stats["TrueAttackDamage"] -= stats["BaseAttackDamage"];
                    stats["BaseAttackDamage"] = +skillValue;
                    stats["TrueAttackDamage"] += stats["BaseAttackDamage"];
                }
                if (skillAffector == "BaseArmor")
                {
                    stats["BaseArmor"] += skillValue;
                    stats["TrueArmor"] = stats["BaseArmor"];
                }
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
        var stats = new Dictionary<string, float> { {"Strength", PlayerManager.Instance.statList["Strength"] }, { "Intelligence", PlayerManager.Instance.statList["Intelligence"] },
        {"Tenacity", PlayerManager.Instance.statList["Tenacity"] }};
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
