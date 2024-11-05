using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NavScript : MonoBehaviour
{
    public Transform counter1;
    public Transform counter2;
    private TrafficManager counter1Script;
    private TrafficManager counter2Script;
    private NavMeshAgent npc;
    [SerializeField] private List<Transform> waypoints;
    private int prevTarget;
    // Start is called before the first frame update
    void Start()
    {
        counter1Script = counter1.GetComponent<TrafficManager>();
        counter2Script = counter2.GetComponent<TrafficManager>();
        npc = GetComponent<NavMeshAgent>();
        // Controls Ambience NPC initial movement
        if (this.gameObject.tag == "AmbienceNPC")
        {
            int randomTarget = Random.Range(0, waypoints.Count);
            npc.destination = waypoints[randomTarget].position;
            prevTarget = randomTarget;
            StartCoroutine(AmbienceCoroutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (this.gameObject.tag == "AmbienceNPC" && npc.isStopped == true)
        //{
        //    int randomCounter = Random.Range(0, 1000);
        //    print("We are at exterior");
        //    if (randomCounter == 1)
        //    {
        //        print("We made it in");
        //        int randomTarget = Random.Range(0, waypoints.Count);
        //        npc.destination = waypoints[randomTarget].position;
        //        npc.isStopped = false;
        //    }
        //}

        
       if (this.gameObject.tag != "AmbienceNPC")
        {
            occupancyChecker();
        }
    }

    private void occupancyChecker()
    {
        if (!counter1Script.isOccupied)
        {
            npc.destination = counter1.position;
            if (npc.transform.position == counter1.position)
            {
                return;
            }
        }
        else if (!counter2Script.isOccupied)
        {
            npc.destination = counter2.position;
            if (npc.transform.position == counter2.position)
            {
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        new WaitForSeconds(2);
        if (other.gameObject.tag == "Waypoint")
        {
            //print("Trigger Enter");
            npc.isStopped = true;
        }
    }
    IEnumerator AmbienceCoroutine()
    {
        while(true)
        {
            print("Heyoooo");
            yield return new WaitForSeconds(10);
            print("Hi");
            //if (npc.isStopped == true)
            //{
            //    print("So true bestie");
            //}
            int randomTarget = Random.Range(0, waypoints.Count);
            if (npc.isStopped == true && randomTarget != prevTarget)
            {
                print("We in");
                npc.destination = waypoints[randomTarget].position;
                npc.isStopped = false;
            }
            prevTarget = randomTarget;
        }
    }
}

