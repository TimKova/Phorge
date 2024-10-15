using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilGame : MonoBehaviour
{
    public GameObject player_manager;
    public Canvas AnvilMenu;
    private string cur_state;
    public GameObject hammer;
    Animator hammerAnimator;
    public GameObject ingotPrefab;
    public GameObject anvil;
    const float ingotScale = 2.5f;
    private readonly Vector3 anvilTop = new Vector3(-15.321f, 0.9684f, -11.472f);
    private static int currentIngot;
    //private readonly Color RED = new Color(1f, 0f, 0f, 1f);

    private Player_Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        hammerAnimator = hammer.GetComponent<Animator>();
        playerInventory = player_manager.GetComponent<Player_Inventory>();
        
        currentIngot = -1;
    }

    // Update is called once per frame
    void Update()
    {
        cur_state = player_manager.GetComponent<Player_Manager>().get_cur_state();

        //if (cur_state == "free_move")
        //{
        //    ClearIngots();
        //}
        if (Input.GetKeyDown(KeyCode.F))
        {
            ClearIngots();
        }
    }

    public void SpawnIngot(int ingotType)
    {
        // Tried with both uppercase and lowercase ingot types. I just don't know how we're getting the input,
        // It doesn't appear that the inventory field is updating, even though I think it logically should?
        ClearIngots();
        if (ingotType < playerInventory.materialNames.Length)
        {
            var mat = playerInventory.getMaterial(ingotType);
            if (mat == null || mat.getQuantity() <= 0)
            {
                print("Not enough of that Material!");
                return;
            }
            string ingotName = playerInventory.getMaterial(ingotType).name;
            GameObject templateIngot = Instantiate(ingotPrefab, anvilTop, Quaternion.identity);
            templateIngot.transform.localScale = new Vector3(ingotScale, ingotScale, ingotScale);
            Material ingotMat = Resources.Load(ingotName) as Material;
            templateIngot.GetComponent<Renderer>().material = ingotMat;
            templateIngot.tag = "ingotPrefab";
            currentIngot = ingotType;
        }
    }

    public void ConfirmMaterial()
    {
        if (currentIngot < 0)
            return;
        if (playerInventory.getMaterial(currentIngot).getQuantity() <= 0)
        {
            ClearIngots();
            print("Not enough of that Material!");
            return;
        }
        var mat = playerInventory.getMaterial(currentIngot);
        print(mat.spend() + " " + mat.getName() + " ingots left");
        AnvilMenu.enabled = false;
    }

    public void ClearIngots()
    {
        foreach (GameObject ingot in GameObject.FindGameObjectsWithTag("ingotPrefab"))
        {
            Destroy(ingot);
        }
    }

    public void SwingHammer()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            hammerAnimator.SetBool("HammerSwing", true);
        }
    }
}
