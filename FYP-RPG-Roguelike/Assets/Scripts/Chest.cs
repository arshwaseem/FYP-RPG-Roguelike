using MoreMountains.InventoryEngine;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{

    public BoxCollider _area;
    public List<InventoryItem> _items;
    public string _names = "";
    public GameObject _AlertWindow;

    private void OnTriggerEnter(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (InventoryItem item in _items)
            {
                item.Pick("PlayerPlaceHolder");
                _names += item.name + ", ";
            }

            _AlertWindow.GetComponent<TextMeshProUGUI>().text = "Picked Up Items " + _names + " Click on the inventory button to open and use Inventory Items";
        }
    }
}
