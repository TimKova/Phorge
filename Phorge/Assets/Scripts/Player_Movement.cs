using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Player Manager")]
    public GameObject player_manager;
    public string cur_state;

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

    Animator animator;
    Vector3 moveDirection;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jump = new Vector3(0, 2f, 0);
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, whatisGround);
        MyInput();
        SpeedControl();
        cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();
        if(cur_state == "free_move")
        {
            FreeMove();
        }else if(cur_state == "task_int")
        {

        }else if (cur_state == "npc_int")
        {

        }//end if-else
    }

    private void FreeMove()
    {
        //Ground Movement
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 horizontalMovement = orientation.right * horizontalInput;
        Vector3 verticalMovement = orientation.forward * verticalInput;
        rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);

        //Jumping
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

        //Animations
        if (verticalInput > 0f)
            animator.SetFloat("Vertical", 1f);
        else if (verticalInput < 0f)
            animator.SetFloat("Vertical", -1f);
        else
            animator.SetFloat("Vertical", 0f);

        if (horizontalInput > 0f)
            animator.SetFloat("Horizontal", 1f);
        else if (horizontalInput < 0f)
            animator.SetFloat("Horizontal", -1f);
        else
            animator.SetFloat("Horizontal", 0f);

    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

}