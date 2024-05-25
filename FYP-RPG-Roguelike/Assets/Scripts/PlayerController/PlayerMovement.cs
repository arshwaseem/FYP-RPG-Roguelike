using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 movement;
    private Rigidbody _rb;
    private Animator playerAnim;
    public float speed = 5f;
    public float originalSpeed = 5f;
    public float turnRate = 360f;
    public GameObject skillTreeUI;
    [SerializeField] private Camera playerCam;
    public bool isRunning = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerAnim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        TakeInput();
        Look();
        UpdateAnimator();
        HandleUIInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void TakeInput()
    {
        // Get input for horizontal and vertical movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate movement direction based on input
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Adjust movement direction based on camera orientation
        if (playerCam != null)
        {
            Vector3 camForward = playerCam.transform.forward;
            Vector3 camRight = playerCam.transform.right;
            camForward.y = 0f; // Ensure y component is zero for flat movement
            camRight.y = 0f;
            movement = (camForward * verticalInput + camRight * horizontalInput).normalized;
        }
        else
        {
            movement = direction;
        }
    }

    private void Move()
    {
        if (movement != Vector3.zero)
        {
            Vector3 newPosition = transform.position + movement * speed * Time.deltaTime;
            _rb.MovePosition(newPosition);
        }
    }

    private void Look()
    {
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnRate * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {
        float speedPercent = movement.magnitude;
        if (speedPercent < 0.01f)
        {
            speedPercent = 0f;
            playerAnim.SetFloat("speedPercent", speedPercent);
        }
        else
        {
            playerAnim.SetFloat("speedPercent", speedPercent, 0.1f, Time.deltaTime);
            playerAnim.SetBool("isRunning", isRunning);
        }
    }

    private void HandleUIInputs()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            skillTreeUI.SetActive(!skillTreeUI.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameObject alertUI = GameObject.Find("AlertUI");
            if (alertUI != null)
            {
                Transform alert = alertUI.transform.Find("Alert");
                if (alert != null)
                {
                    alert.gameObject.SetActive(!alert.gameObject.activeSelf);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
            speed = speed * 1.5f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
            speed = originalSpeed;
        }
    }
}
