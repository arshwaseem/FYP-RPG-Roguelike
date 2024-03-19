using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 movement;
    public Rigidbody _rb;
    public Animator playerAnim;
    public float speed = 5f;
    public float turnRate = 360;
    public GameObject skillTreeUI;
    [SerializeField] Camera playerCam;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        TakeInput();
        Look();
        UpdateAnimator();
        if (Input.GetKeyDown(KeyCode.P))
        {
            skillTreeUI.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject.Find("AlertUI").transform.Find("Alert").gameObject.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        Move();
    }
    void TakeInput()
    {
        // Get input for horizontal movement (A/D or left/right arrow keys)
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Get input for vertical movement (W/S or up/down arrow keys)
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Combine inputs to calculate movement direction
        movement = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Adjust movement direction based on camera orientation
        Vector3 camForward = playerCam.transform.forward;
        Vector3 camRight = playerCam.transform.right;
        camForward.y = 0f; // Make sure y component is zeroed out for flat movement
        camRight.y = 0f;
        movement = horizontalInput * camRight + verticalInput * camForward;
    }


    void Move()
    {
        _rb.MovePosition(transform.position + movement * speed * Time.deltaTime);
    }

    void Look()
    {
        if (movement != Vector3.zero)
        {
            // Remove any unnecessary rotation adjustments
            var rot = Quaternion.LookRotation(movement, Vector3.up);

            // Rotate towards the desired direction smoothly
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnRate * Time.deltaTime);
        }
    }

    void UpdateAnimator()
    {
        float pS = movement.magnitude;
        playerAnim.SetFloat("speedPercent", pS, .1f, Time.deltaTime);
    }
}
