using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour

{
    [SerializeField] public InteractionInputData interactioninputdata;
    // Start is called before the first frame update
    void Start()
    {
        interactioninputdata.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        GetInteractionInputData();
    }

    void GetInteractionInputData()
    {
        interactioninputdata.InteractPressed = Input.GetKeyDown(KeyCode.F);
        interactioninputdata.InteractReleased = Input.GetKeyUp(KeyCode.F);
    }
}
