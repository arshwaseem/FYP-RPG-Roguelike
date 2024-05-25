using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Item itemInfo;
    public Image itemImage;
    // Start is called before the first frame update
    void Start()
    {
        InitInvSlot(itemInfo);
    }

    public void InitInvSlot(Item item)
    {
        itemInfo = item;
        itemImage.sprite = item._icon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        itemInfo.onUse();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData e)
    {

    }
}
