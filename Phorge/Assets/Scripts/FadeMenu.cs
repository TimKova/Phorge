using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeMenu : MonoBehaviour
{
    public GameObject fader;

    public Material material;

    private bool fading = false;
    private float alpha = 0;
    public bool faded = false;


    // Start is called before the first frame update
    void Start()
    {
        alpha = 0;
    }

    private void Update()
    {
        print("faded:" + faded);
        if (fading)
        {
            alpha += 0.5f * Time.deltaTime;
            material.color = new Color(0, 0, 0, alpha);
            if (alpha > 1f)
            {
                faded = true;
            }
        }
        else
        {
            material.color = new Color(0, 0, 0, 0);
        }
    }

    public void FadeAway()
    {
        fading = true;
    }
}
