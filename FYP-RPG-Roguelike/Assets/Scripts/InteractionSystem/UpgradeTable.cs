using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTable : InteractableBase
{
    public override void OnInteract()
    {
        base.OnInteract();
        GameObject.Find("SkillTreeUI").SetActive(true);
    }
}
