using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilGame : MonoBehaviour
{
    public GameObject player_manager;
    private string cur_state;

    public GameObject hammer;
    Animator hammerAnimator;
    public GameObject ingotPrefab;
    public GameObject anvil;
    const float ingotScale = 2.5f;
    private readonly Vector3 anvilTop = new Vector3(-15.321f, 0.9684f, -11.472f);
    private readonly Color RED = new Color(1f, 0f, 0f, 1f);
    // Start is called before the first frame update
    void Start()
    {
        hammerAnimator = hammer.GetComponent<Animator>();
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

    public void SpawnIngot(string ingotType)
    {
        GameObject templateIngot = Instantiate(ingotPrefab, anvilTop, Quaternion.identity);
        templateIngot.transform.localScale = new Vector3(ingotScale, ingotScale, ingotScale);
        Material ingotMat = Resources.Load(ingotType) as Material;
        templateIngot.GetComponent<Renderer>().material = ingotMat;
        templateIngot.tag = "ingotPrefab";
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
