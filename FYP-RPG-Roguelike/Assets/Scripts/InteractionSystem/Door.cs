using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : InteractableBase
{

    float maxHeight = 2.05f;
    bool isOpen = false;
    GameObject door;
    Collider doorCollider;
    Vector3 startPos;

    private void Start()
    {
        door = GameObject.Find("Door");
        doorCollider = door.GetComponent<Collider>();
        startPos = door.transform.position;
    }

    public override void OnInteract()
    {
        base.OnInteract();
        if (isOpen == false)
        {
            isOpen = !isOpen;
            Debug.Log("Door: " + isOpen);
            doorCollider.enabled = false;
            door.transform.position += new Vector3(0, 3f ,0);
        }
        else if (isOpen == true)
        {
            isOpen = !isOpen;
            Debug.Log("Door: " + isOpen);
            doorCollider.enabled = true;
            door.transform.position = startPos;
        }
    }
}
