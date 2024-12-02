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
            pauseCanvas.GetComponent<CanvasGroup>().alpha = 1.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            pauseCanvas.GetComponent<CanvasGroup>().alpha = 0.0f;
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
