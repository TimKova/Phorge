using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public GameObject playermodel;
    public GameObject camera;
    public float rotationSpeed;

    bool canInteract;
    bool isInteracting;

    // Start is called before the first frame update
    void Start()
    {
        canInteract = false;
        isInteracting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (Input.GetKeyDown("e"))  // can be e or any key
            {
                isInteracting = true;

            }
        }

        if (isInteracting)
        {
            playerLock();
            if (Input.GetKeyDown("q"))
            {
                isInteracting = false;
            }
        }
    }

    void playerLock()
    {
        Vector3 newPos = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
        playermodel.transform.position = newPos;
        playermodel.GetComponent<Movement>().enabled = false;
        camera.GetComponent<Camera_Controller>().enabled = false;

        Vector3 lookDir = this.transform.position - camera.transform.position;
        Quaternion q = Quaternion.LookRotation(lookDir);

        //Either of the below methods will work, but we're getting issues with the camera seeing inside the player model.
        //camera.transform.rotation = q;
        camera.transform.LookAt(this.transform);

        //I don't think this method is correct, but it might be more correct if fixed
        //transform.rotation = Quaternion.RotateTowards(playermodel.transform.rotation, q, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other) // to see when the player enters the collider
    {
        if (other.gameObject.tag == "Player") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            canInteract = true;
        }
    }
}
