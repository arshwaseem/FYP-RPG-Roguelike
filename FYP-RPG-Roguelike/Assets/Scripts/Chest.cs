using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public BoxCollider collider;
    public bool isInteractable=false;
    public GameObject DamageTrigger;
    public List<Item> Items;

    private void Start()
    {
        collider = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("in contact");
        RoamUIManager.Instance.showAlertWindow("Press E to open this chest");
        isInteractable = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInteractable == true) { 
            RoamUIManager.Instance.showAlertWindow("Picked Up: Health Potion, Mana Potion, Helmet, Sword");
            DamageTrigger.SetActive(true);
            }

            foreach (Item item in Items)
            {
                PlayerManager.Instance.Inventory.Add(item);
            }
        }
    }

}
