using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject fader;
    bool faded = false;
    void Update()
    {
        faded = fader.GetComponent<FadeMenu>().faded;

        if (faded)
        {
            PlayGame();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("PhorgeGame");
    }
    public void QuitGame() 
    {
        Application.Quit();
    }
}
