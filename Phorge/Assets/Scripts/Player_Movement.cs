using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Player Manager")]
    public GameObject player_manager;
    public string cur_state;
    public string taskName;

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

    [Header("Tasks")]
    public GameObject anvil_task;
    public GameObject furnace_task;

    public GameObject MrItemMan;
    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    bool jumpInput;
    bool leftMouseClick;

    Animator animator;
    Vector3 moveDirection;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        print("TEST-------------------------------");
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
        //print(cur_state);
        if (cur_state == "free_move")
        {
            FreeMove();
        }
        else if (cur_state == "task_int")
        {
            TaskMove();
        }
        else if (cur_state == "npc_int")
        {

        }//end if-else
    }
    private void TaskMove()
    {
        taskName = player_manager.GetComponent<Player_Manager>().get_cur_task();
        animator.SetFloat("Vertical", 0);
        animator.SetFloat("Horizontal", 0);
        if (taskName == "Anvil")
        {
            transform.position = new Vector3(anvil_task.transform.position.x + 2f, transform.position.y, anvil_task.transform.position.z);
            transform.LookAt(anvil_task.transform.position);

        }
        else if (taskName == "Furnace")
        {
            transform.position = new Vector3(-12.255f, transform.position.y, -11.847f);
            transform.LookAt(furnace_task.transform.position);
        }
    }
    private void NPCMove()
    {
        
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
