using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    //Code is based on video by Dave/GameDevelopment

    public float sensX;
    public float sensY;
    public GameObject player_manager;
    public GameObject anvil_task;
    public GameObject furnace_task;
    string taskName;

    public Transform orientation;

    float xRotation;
    float yRotation;

    string cur_state;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();
        taskName = player_manager.GetComponent<Player_Manager>().get_cur_task();

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

    void FreeMove()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        //transform.position = new Vector3 (orientation.position.x, orientation.position.y + camYDiff, orientation.position.z);
    }

    void TaskMove()
    {
        if (taskName == "Anvil")
        {
            transform.LookAt(anvil_task.transform.position);
        }
        else if (taskName == "Furnace")
        {
            transform.LookAt(furnace_task.transform.position);
        }

    }
}
