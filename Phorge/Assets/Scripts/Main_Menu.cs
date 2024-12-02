using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{

    [SerializeField]
    private SceneController sceneController;

    public void PlayGame()
    {
        sceneController.LoadScene("PhorgeGame");
    }
    public void QuitGame() 
    {
        Application.Quit();
    }
}
