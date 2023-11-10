using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]public class InteractionController : MonoBehaviour
{
    #region Vars
    [Header("Data")]
    public InteractionInputData interactionInputData;
    public InteractableData interactableData;

    private bool isInteracting = false;
    private float Timer = 0f;

    [Header("Ray Settings")]

    public float RayDistance;
    public float RaySphereRadius;

    public LayerMask InteractableLayer;

    private GameObject player;
    #endregion

    #region BaseMethods
    void Awake()
    {
        player = GameObject.Find("PlayerPlaceHolder");
    }

    void Update()
    {
        CheckForInteractable();
        CheckForInteractableInput();
    }
    #endregion

    #region CustomMethods
    void CheckForInteractable()
    {
        Ray _ray = new Ray(player.transform.position, player.transform.forward);

        RaycastHit _hitInfo;

        bool isRayHitting = Physics.SphereCast(_ray, RaySphereRadius, out _hitInfo, RayDistance, InteractableLayer);

        if (isRayHitting)
        {
            InteractableBase _interactable = _hitInfo.transform.GetComponent<InteractableBase>();

            if (_interactable != null)
            {
                if (interactableData.IsEmpty())
                {
                    interactableData.Interactable = _interactable;
                }
                else
                {
                    if (!interactableData.IsSameInteractable(_interactable))
                    {
                        interactableData.Interactable = _interactable;
                    }
                    else
                    {

                    }
                }
            }
        }
        else
        {
            interactableData.ResetData();
        }

        Debug.DrawRay(_ray.origin, _ray.direction * RayDistance, isRayHitting ? Color.green : Color.red);
    }

    void CheckForInteractableInput()
    {
        if (interactableData.IsEmpty())
        {
            return;
        }

        if (interactionInputData.InteractPressed)
        {
            isInteracting = true;
            Timer = 0f;
        }

        if (interactionInputData.InteractReleased)
        {
            isInteracting = false;
            Timer = 0f;
        }

        if (isInteracting)
        {
            if (!interactableData.Interactable.isInteractable)
            {
                return;
            }
            if (interactableData.Interactable.isHold)
            {
                Timer += Time.deltaTime;
                if(Timer >= interactableData.Interactable.HoldDuration)
                {
                    interactableData.Interact();
                    isInteracting = false;
                }
            }
            else
            {
                interactableData.Interact();
                isInteracting = false;
            }
        }
    }
    #endregion
}
