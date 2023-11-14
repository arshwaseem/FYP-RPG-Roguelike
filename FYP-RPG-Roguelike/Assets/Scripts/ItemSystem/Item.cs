using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Item",menuName ="Items/Item")]
public class Item : ScriptableObject
{
    #region variables
    public string Name;
    public string Description;
    public Sprite Icon;
    public List<EntityStat> statModifiers = new List<EntityStat>();
    #endregion

    public virtual void onItemUse()
    {
        Debug.Log("Consumable " + Name + "was used");
    }
}
