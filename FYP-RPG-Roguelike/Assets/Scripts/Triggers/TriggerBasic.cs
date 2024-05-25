using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBasic : MonoBehaviour
{
    public BoxCollider TriggerArea;

    public void Start()
    {
        TriggerArea = this.gameObject.GetComponent<BoxCollider>();
    }

    public virtual void TriggerInvoke()
    {
        Debug.Log("Trigger Invoked");
    }

    public void OnTriggerEnter(Collider p)
    {
        TriggerInvoke();
    }
}
