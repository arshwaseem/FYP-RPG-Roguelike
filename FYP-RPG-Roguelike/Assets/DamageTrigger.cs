using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerManager.Instance.playerStats.currentHP -= 50;
        RoamUIManager.Instance.showAlertWindow("Looks like you took damage, open the inventory from your bottom left to use the health potion and mana potion and equip your sword and helmet");
        RoamUIManager.Instance.UpdateHealth(PlayerManager.Instance.playerStats.currentHP);
    }
}
