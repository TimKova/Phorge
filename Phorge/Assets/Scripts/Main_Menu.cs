using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{

    public void PlayGame() 
    {
        SceneManager.LoadScene("PhorgeGame");
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
