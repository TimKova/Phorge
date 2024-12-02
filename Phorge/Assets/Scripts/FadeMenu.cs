using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeMenu : MonoBehaviour
{
    public GameObject fader;

    public Color currentColor;

    private bool fading = false;
    private float alpha = 0;
    public bool faded = false;


    // Start is called before the first frame update
    void Start()
    {
        currentColor = GetComponent<Renderer>().material.color;
    }

    private void Update()
    {
        print("faded:" + faded);
        if (fading)
        {
            Debug.Log("fading");
            currentColor.a += 0.5f * Time.unscaledDeltaTime;
            if (alpha > 1f)
            {
                faded = true;
            }
        }
        else
        {
            currentColor.a = 0f;
        }
    }

    public void FadeAway()
    {
        fading = true;
    }
}
