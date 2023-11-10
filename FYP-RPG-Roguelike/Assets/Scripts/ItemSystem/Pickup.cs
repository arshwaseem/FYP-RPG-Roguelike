using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pickup : InteractableBase
{
    public Equipment p_item;

    public void Start()
    {
        p_item.Awake();
    }

    public override void OnInteract()
    {
        base.OnInteract();
        PlayerManager.playerManager.Equip(p_item);
    }

}
