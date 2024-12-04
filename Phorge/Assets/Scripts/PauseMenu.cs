using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    bool paused;

    public GameObject playerManager;
    public Canvas pauseCanvas;

    [SerializeField]
    private Image controls;

    private void Start()
    {
        paused = false;
        controls.gameObject.SetActive(false);
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
            controls.color = new Color(controls.color.r, controls.color.g, controls.color.b, 0);
            pauseCanvas.enabled =false;
            controls.gameObject.SetActive(false);
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
        controls.color = new Color(controls.color.r, controls.color.g, controls.color.b, 1);
        controls.gameObject.SetActive (true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
