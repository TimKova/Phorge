using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground")]
    public float playerHeight;
    public LayerMask whatisGround;
    public bool grounded;

    [Header("Jump")]
    public Vector3 jump;
    public float jumpForce = 2.0f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    bool jumpInput;

    Vector3 moveDirection;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jump = new Vector3(0, 2f, 0);

    }

    private void FixedUpdate()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, whatisGround);
        MyInput();

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else rb.drag = 0f;

        if (grounded && jumpInput)
        {
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            grounded = false;
        }
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
    }
}
