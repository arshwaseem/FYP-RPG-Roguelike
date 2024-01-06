using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerMovement : MonoBehaviour
{
    public Vector3 movement;
    Rigidbody _rb;
    public Animator playerAnim;
    [SerializeField] public float speed = 5f;
    [SerializeField] float turnRate = 360;
    public GameObject skillTreeUI;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        TakeInput();
        Look();
        updateAnimator();
        if (Input.GetKeyDown(KeyCode.P))
        {
            skillTreeUI.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void TakeInput()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }
    void Move()
    {
        _rb.MovePosition(transform.position + (transform.forward * movement.magnitude) * speed * Time.deltaTime);
    }

    void Look()
    {
        if (movement != Vector3.zero)
        {
            var relative = (transform.position + movement) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnRate * Time.deltaTime);
        }
    }
    void updateAnimator()
    {
        float pS = movement.magnitude;
        playerAnim.SetFloat("speedPercent", pS, .1f, Time.deltaTime);
    }
}