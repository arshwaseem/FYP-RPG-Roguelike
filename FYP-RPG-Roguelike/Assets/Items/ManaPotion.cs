using MoreMountains.InventoryEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MoreMountains.InventoryEngine
{

    [CreateAssetMenu(fileName = "ManaPotion", menuName = "MoreMountains/InventoryEngine/ManaPotion", order = 1)]
    public class ManaPotion : InventoryItem
    {
        public int HealAmount;

        public override bool Use(string playerID)
        {
            var ManaRestored = 0;

            if (BattleManager.Instance)
            {
                ManaRestored = (int)(BattleManager.Instance.FriendlyCharacters[0].characterData.currMana + HealAmount);
                if(ManaRestored > BattleManager.Instance.FriendlyCharacters[0].characterData.maxMana)
                {
                    BattleManager.Instance.FriendlyCharacters[0].characterData.currMana = BattleManager.Instance.FriendlyCharacters[0].characterData.maxMana;
                    BattleManager.Instance.FriendlyCharacters[0].characterData.charUi.UpdateManaBar(BattleManager.Instance.FriendlyCharacters[0].characterData.currMana);
                    return true;
                }
                else
                {
                    BattleManager.Instance.FriendlyCharacters[0].characterData.currMana = ManaRestored;
                    BattleManager.Instance.FriendlyCharacters[0].characterData.charUi.UpdateManaBar(BattleManager.Instance.FriendlyCharacters[0].characterData.currMana);
                    return true;
                }
                
            }
            else
            {
                ManaRestored = (int)(PlayerManager.Instance.playerStats.currentMana + HealAmount);
                if (ManaRestored > PlayerManager.Instance.playerStats.maxMana)
                {
                    PlayerManager.Instance.playerStats.currentMana = PlayerManager.Instance.playerStats.maxMana;
                    PlayerManager.Instance.updateUI();
                    return true;
                }
                else
                {
                    PlayerManager.Instance.playerStats.currentMana = ManaRestored;
                    PlayerManager.Instance.updateUI();
                    return true;
                }
            }

        }

        public override bool Pick(string playerID)
        {
            _targetEquipmentInventory.AddItem(this, 1);
            return true;
        }




    }
}

