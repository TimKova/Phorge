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
        Cursor.lockState = CursorLockMode.Locked;
        playerManager.GetComponentInChildren<State_Manager>().pause = false;
        paused = false;
        Time.timeScale = 1;
    }

    public void ShowControls()
    {

    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
