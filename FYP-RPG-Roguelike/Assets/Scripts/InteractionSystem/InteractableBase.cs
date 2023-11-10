using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class InteractableBase : MonoBehaviour, IInteractable
{
    #region Variables
    [Header("Interactable Settings")]

    public float holdDuration;

    public bool isHold;

    public bool multipleUse;

    public bool isInteractable;
    #endregion

    #region Properties
    public float HoldDuration => holdDuration;
    public bool IsHold => isHold;
    public bool MultipleUse => multipleUse;
    public bool IsInteractable => isInteractable;
    #endregion


    #region Methods
    public virtual void OnInteract()
    {
        Debug.Log("Interacted" + gameObject.name);
    }
    #endregion


}
