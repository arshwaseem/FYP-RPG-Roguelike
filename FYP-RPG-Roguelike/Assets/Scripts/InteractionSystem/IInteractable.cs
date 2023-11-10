using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    float HoldDuration
    {
        get;
    }

    bool IsHold
    {
        get;
    }

    bool MultipleUse
    {
        get;
    }

    bool IsInteractable
    {
        get;
    }

    void OnInteract();
}
