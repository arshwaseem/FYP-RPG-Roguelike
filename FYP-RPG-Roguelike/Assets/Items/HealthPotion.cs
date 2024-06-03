using MoreMountains.InventoryEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MoreMountains.InventoryEngine
{

    [CreateAssetMenu(fileName = "HealthPotion", menuName = "MoreMountains/InventoryEngine/HealthPotion", order = 1)]
    public class HealthPotion : InventoryItem
    {
        public int HealAmount;

        public override bool Use(string playerID)
        {
            var HpRestored = 0;

            if (BattleManager.Instance)
            {
                HpRestored = (int)(BattleManager.Instance.FriendlyCharacters[0].characterData.currHealth + HealAmount);
                BattleManager.Instance.FriendlyCharacters[0].characterData.Heal(HpRestored);
                return true;
            }
            else
            {
                HpRestored = (int)(PlayerManager.Instance.playerStats.currentHP + HealAmount);
                if(HpRestored > PlayerManager.Instance.playerStats.maxHP)
                {
                    PlayerManager.Instance.playerStats.currentHP = PlayerManager.Instance.playerStats.maxHP;
                    PlayerManager.Instance.updateUI();
                    return true;
                }
                else
                {
                    PlayerManager.Instance.playerStats.currentHP = HpRestored;
                    PlayerManager.Instance.updateUI();
                    return true;
                }
            }

        }
    }
}

