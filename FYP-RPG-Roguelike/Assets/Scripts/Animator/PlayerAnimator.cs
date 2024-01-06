using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{

    Animator charAnimator;
    PlayerMovement playSpeed;
    float animSpeed = .1f;
    // Start is called before the first frame update
    void Start()
    {
        playSpeed = GetComponent<PlayerMovement>();
        charAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float speedPercent = playSpeed.movement.magnitude / playSpeed.speed;
        charAnimator.SetFloat("speedPercent", speedPercent, animSpeed, Time.deltaTime);
    }
}
