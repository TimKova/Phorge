using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    bool paused;

    public GameObject playerManager;
    public Canvas pauseCanvas;

    private void Start()
    {
        paused = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;



            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log("Clicked object: " + hit.collider.gameObject.name); // Print the name of the clicked object

            }

        }
        if (paused)
        {
            pauseCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            pauseCanvas.enabled =false;
        }

        paused = playerManager.GetComponentInChildren<State_Manager>().pause;
    }

    public void ResumeGame()
    {
        playerManager.GetComponentInChildren<State_Manager>().pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void ShowControls()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
