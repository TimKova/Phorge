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
    public Camera playerCam;
    public Camera npcCam;
    public Camera npcCam2;
    public Camera anvilCam;
    public Camera furnaceCam;
    public Camera finishCam;
    //public GameObject merchant;
    //[SerializeField] public List<GameObject> questGivers;
    string taskName;
    public bool hasRelevantMenuOpened;

    //private readonly Vector3 furnacePosition = new Vector3(0f, 1.243f, 2.885f);
    //private readonly Vector3 anvilPosition = new Vector3Vector3(0.234f, 0.69f, 0f);

    public Canvas AnvilMenu;
    public Canvas FurnaceMenu;
    public Canvas MerchantMenu;
    private AnvilGame anvilGame;
    private Merchant merchantManager;


    public Transform orientation;

    float xRotation;
    float yRotation;

    string cur_state;
    private string npcName;

    // Start is called before the first frame update
    void Start()
    {
        npcCam2.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        hasRelevantMenuOpened = false;
        anvilGame = anvil_task.GetComponent<AnvilGame>();
        merchantManager = MerchantMenu.GetComponent<Merchant>();
    }

    // Update is called once per frame
    void Update()
    {
        cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();
        taskName = player_manager.GetComponent<Player_Manager>().get_cur_task();
        npcName = player_manager.GetComponent<Player_Manager>().get_cur_npc();
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
            NPCMove();
        }//end if-else
    }

    public void FreeMove()
    {
        SnapToPlayer();
        CloseMenus();
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
            playerCam.enabled = false;
            anvilCam.enabled = true;
            if (!hasRelevantMenuOpened)
            {
                anvilGame.refreshQuantities();
                //added this here so that anytime you open the menu the cursor appears, but once the game starts the cursor goes away
                //It's kinda scuffed rn but we can make it better if/when we refactor the code
                Cursor.lockState = CursorLockMode.Confined;
                AnvilMenu.enabled = true;
                hasRelevantMenuOpened = true;
            }
        }

        else if (taskName == "Furnace")
        {
            playerCam.enabled = false;
            furnaceCam.enabled = true;
            if (!hasRelevantMenuOpened)
            {
                //furnaceGame.refreshQuantities();
                //added this here so that anytime you open the menu the cursor appears, but once the game starts the cursor goes away
                //It's kinda scuffed rn but we can make it better if/when we refactor the code
                Cursor.lockState = CursorLockMode.Confined;
                FurnaceMenu.enabled = true;
                hasRelevantMenuOpened = true;
            }          
        }
    }

    void NPCMove()
    {
        //print(npcName);
        if (npcName == "MrItemMan")
        {
            playerCam.enabled = false;
            npcCam.enabled = true;
            MerchantMenu.enabled = true;
            if (!hasRelevantMenuOpened)
            {
                merchantManager.refreshQuantities();
                hasRelevantMenuOpened = true;
            }
        }
        else if (npcName == "QuestGiver1")
        {
            playerCam.enabled = false;
            npcCam2.enabled = true;
            //MerchantMenu.enabled = true;
            //if (!hasRelevantMenuOpened)
            //{
            //    merchantManager.refreshQuantities();
            //    hasRelevantMenuOpened = true;
            //}
        }

    }   

    public void SnapToPlayer()
    {
        playerCam.enabled = true;
        anvilCam.enabled = false;
        furnaceCam.enabled = false;
        npcCam.enabled = false;
        npcCam2.enabled = false; 
        hasRelevantMenuOpened = false;
        
    }

    public void CloseMenus()
    {
        AnvilMenu.enabled = false;
        FurnaceMenu.enabled = false;
        MerchantMenu.enabled = false;
    }

}
