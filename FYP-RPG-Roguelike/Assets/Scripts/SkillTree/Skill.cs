using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="SkillTreeSkill", menuName ="SkillTree/SkillTreeSkill")]

public class Skill : ScriptableObject
{
    public bool isParentSkill;
    public string skillName;
    public string skillDesc;
    public Sprite skillImg;
    public Sprite skillEnable;
    public float skillValue;
    public string skillAffector;

}
