using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = ("InteractionInputData"), menuName = ("InteractionSystem/InputData"))]
public class InteractionInputData : ScriptableObject
{

    private bool isInteractPressed;

    private bool isInteractReleased;

    public bool InteractPressed
    {
        get => isInteractPressed;

        set => isInteractPressed = value;
    }

    public bool InteractReleased
    {
        get => isInteractReleased;
        set => isInteractReleased = value;
    }

    public void Reset()
    {
        isInteractPressed = false;
        isInteractReleased = false;
    }


}
