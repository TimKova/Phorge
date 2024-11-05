using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public bool isOccupied;
    // Start is called before the first frame update
    void Start()
    {
        isOccupied = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            //print("Trigger Enter");
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        new WaitForSeconds(2);
        print("Exited");
        if (other.gameObject.tag == "NPC")
        {
            isOccupied = false;
        }
    }
}
