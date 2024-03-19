using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityData
{
    public string ability_name = "defaultAbility";
    public string ability_description = "Placeholder";

    public int abilityValue = 10;
    public int manaCost = 1;

    public AbilityType type = AbilityType.Melee;
    public AbilityOutput output = AbilityOutput.Damage;


    //TODO: VFX
    public GameObject particleFX;


}
public enum AbilityType
{
    Ranged, Melee
}

public enum AbilityOutput
{
    Damage,Heal,Boost
}