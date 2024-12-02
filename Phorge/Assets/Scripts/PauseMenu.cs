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
    public GameObject pauseCanvas;

    private void Start()
    {
        paused = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (paused)
        {
            pauseCanvas.SetActive(true);
        }
        else
        {
            pauseCanvas.SetActive(false);
        }

        paused = playerManager.GetComponent<State_Manager>().pause;
    }

    public void ResumeGame()
    {
        paused = false;
    }

    public void ShowControls()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
