using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamSway : MonoBehaviour
{
    private readonly float swayAmnt = 0.1f;
    public GameObject cam;

    // Update is called once per frame
    void Update()
    {
        cam.transform.eulerAngles = new Vector3(
            cam.transform.eulerAngles.x,
            cam.transform.eulerAngles.y,
            cam.transform.eulerAngles.z + Mathf.Sin(Time.time) / 180
        );
    }
}
