using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class PlayerMovement : MonoBehaviour
    {
        Vector3 movement;
        Rigidbody _rb;
        [SerializeField] float speed = 5f;
        [SerializeField] float turnRate = 360;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
        {
            TakeInput();
            Look();
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
            if(movement != Vector3.zero)
            {
                var relative = (transform.position + movement) - transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnRate * Time.deltaTime);
            }
        }
    }