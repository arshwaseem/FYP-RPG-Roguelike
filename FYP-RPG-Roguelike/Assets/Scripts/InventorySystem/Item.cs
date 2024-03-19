using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]

public class Item : ScriptableObject
{

    public string itemName;
    public string itemDesc;
    public Sprite _icon;
    public string Statname;
    public float Statvalue;

    public virtual void onUse()
    {
            if (Statname == "Health")
            {
                if(PlayerManager.Instance.playerStats.currentHP + Statvalue > PlayerManager.Instance.playerStats.maxHP)
                {
                    PlayerManager.Instance.playerStats.currentHP = PlayerManager.Instance.playerStats.maxHP;
                    RoamUIManager.Instance.UpdateHealth(PlayerManager.Instance.playerStats.currentHP);
                }
                else
                {
                PlayerManager.Instance.playerStats.currentHP += Statvalue;
                    RoamUIManager.Instance.UpdateHealth(PlayerManager.Instance.playerStats.currentHP);
            }
            }

            if (Statname == "Mana")
            {
                if (PlayerManager.Instance.playerStats.currentMana + Statvalue > PlayerManager.Instance.playerStats.maxMana)
                {
                    PlayerManager.Instance.playerStats.currentMana = PlayerManager.Instance.playerStats.maxMana;
                RoamUIManager.Instance.UpdateMana(PlayerManager.Instance.playerStats.currentMana);
            }
                else
                {
                    PlayerManager.Instance.playerStats.currentMana += Statvalue;
                RoamUIManager.Instance.UpdateMana(PlayerManager.Instance.playerStats.currentMana);

            }
            }
    }
}
