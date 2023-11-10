using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InteractionData", menuName ="InteractionSystem/Data")]

public class InteractableData : ScriptableObject
{
    private InteractableBase m_interactable;
    public InteractableBase Interactable
    {
        get => m_interactable;
        set => m_interactable = value;
    }

    public void Interact()
    {
        Interactable.OnInteract();
        ResetData();
    }

    public bool IsSameInteractable(InteractableBase _newInteractable)
    {
        return _newInteractable == Interactable;
    }

    public void ResetData()
    {
        m_interactable = null;
    }

    public bool IsEmpty() => m_interactable == null;
}
