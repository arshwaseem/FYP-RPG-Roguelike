using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    public static InventoryUI Instance;
    public Button HeadIcon;
    public Button BodyIcon;
    public Button LegIcon;
    public Button RHandIcon;
    public Button LHandIcon;
    public Button RingIcon;
    public GameObject itemsList;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    public void onInventoryOpen()
    {

    }

    public void clearEquipmentIcons()
    {
        HeadIcon.image.sprite = null;
        HeadIcon.image.color = Color.clear;
        BodyIcon.image.sprite = null;
        BodyIcon.image.color = Color.clear;
        LegIcon.image.sprite = null;
        LegIcon.image.color = Color.clear;
        RHandIcon.image.sprite = null;
        RHandIcon.image.color = Color.clear;
        LHandIcon.image.sprite = null;
        LHandIcon.image.color = Color.clear;
        RingIcon.image.sprite = null;
        RingIcon.image.color = Color.clear;
    }

    public void UpdateEquipmentUI(Equpiment newItem)
    {
        if(newItem._equipslot == EquipSlot.Head)
        {
            HeadIcon.image.sprite = newItem._icon;
            HeadIcon.image.color = Color.white;
        }
        if (newItem._equipslot == EquipSlot.Body)
        {
            BodyIcon.image.sprite = newItem._icon;
            BodyIcon.image.color = Color.white;
        }
        if (newItem._equipslot == EquipSlot.Leg)
        {
            LegIcon.image.sprite = newItem._icon;
            LegIcon.image.color = Color.white;
        }
        if (newItem._equipslot == EquipSlot.Ring)
        {
            RingIcon.image.sprite = newItem._icon;
            RingIcon.image.color = Color.white;
        }
        if (newItem._equipslot == EquipSlot.RHand)
        {
            RHandIcon.image.sprite = newItem._icon;
            RHandIcon.image.color = Color.white;
        }
        if (newItem._equipslot == EquipSlot.LHand)
        {
            LHandIcon.image.sprite = newItem._icon;
            LHandIcon.image.color = Color.white;
        }
    }


}
