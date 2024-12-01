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
    public Transform MerchantRest;
    public Transform QuestRest1;
    public Transform QuestRest2;
    public Transform AmbiRest1;
    public Transform AmbiRest2;
    private TrafficManager counter1Script;
    private TrafficManager counter2Script;
    private NavMeshAgent npc;
    [SerializeField] private List<Transform> waypoints;
    private int prevTarget;
    public int q1trigger;
    public int q2trigger;
    public State_Manager sm;
    public QuestScript qs;
    public GameObject QuestMenu;
    public int readyToLeaveLocal;
    // Start is called before the first frame update
    void Start()
    {
        sm = FindObjectOfType<State_Manager>();
        counter1Script = counter1.GetComponent<TrafficManager>();
        counter2Script = counter2.GetComponent<TrafficManager>();
        npc = GetComponent<NavMeshAgent>();
        if (this.gameObject.tag == "AmbienceNPC")
        {
            npc.isStopped = true;
        }
        // Controls Ambience NPC initial movement
        if (this.gameObject.tag == "AmbienceNPC" && sm.stage == "workday")
        {
            int randomTarget = Random.Range(0, waypoints.Count);
            npc.destination = waypoints[randomTarget].position;
            prevTarget = randomTarget;
            StartCoroutine(AmbienceCoroutine());
        }
        StartCoroutine(AmbienceCoroutine());
        //StartCoroutine(waiter());
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
            destinationSender();
        }
    }
    // Depreciated for old implementation
    //private void occupancyChecker()
    //{
    //    if (counter1Script == null)
    //        return;
    //    if (!counter1Script.isOccupied)
    //    {
    //        npc.destination = counter1.position;
    //        if (npc.transform.position == counter1.position)
    //        {
    //            return;
    //        }
    //    }
    //    else if (!counter2Script.isOccupied)
    //    {
    //        npc.destination = counter2.position;
    //        if (npc.transform.position == counter2.position)
    //        {
    //            return;
    //        }
    //    }
    //}

    private void destinationSender()
    {
        if (sm.stage == "morning")
        {
            qs.readyToLeave = 0;
            readyToLeaveLocal = 0;
            if (this.gameObject.name == "MrItemMan")
            {
                npc.destination = counter1.position;
            }
        }
        if (sm.stage == "workday")
        {
            if (this.gameObject.name == "MrItemMan")
            {
                npc.isStopped = false;
                npc.destination = MerchantRest.position;
                //print(npc.destination);
            }
            else if (this.gameObject.name == "QuestGiver1")
            {
                if (q1trigger == 1)
                {
                    //print("We did get into the q1 script");
                    //print(npc.destination);
                    npc.isStopped = false;
                    npc.destination = counter1.position;
                }
                if (readyToLeaveLocal == 1)
                {
                    npc.isStopped = false;
                    QuestMenu.SetActive(false);
                    q1trigger = 0;
                    npc.destination = QuestRest1.position;
                    readyToLeaveLocal = 0;
                    //Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else if (this.gameObject.name == "QuestGiver2")
            {
                if (q2trigger == 1)
                {
                    //print("We did get into the q2 script");
                    //print(npc.destination);
                    //npc.isStopped = false;
                    npc.destination = counter2.position;
                    npc.isStopped = false;
                }
                if (readyToLeaveLocal == 1)
                {
                    //npc.isStopped = false;
                    QuestMenu.SetActive(false);
                    q2trigger = 0;
                    npc.destination = QuestRest2.position;
                    npc.isStopped = false;
                    readyToLeaveLocal = 0;
                    //Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }
        if (sm.stage == "evening")
        {
            if (this.gameObject.name == "QuestGiver1")
            {
                //print("Sup");
                npc.isStopped = false;
                npc.destination = QuestRest1.position;
                //print(npc.destination);
            }
            else if (this.gameObject.name == "QuestGiver2")
            {
                npc.isStopped = false;
                npc.destination = QuestRest2.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        new WaitForSeconds(2);
        if (other.gameObject.tag == "Waypoint" && (this.gameObject.name == "Ambience1" || this.gameObject.name == "Ambience2"))
        {
            //print("Trigger Enter");
            npc.isStopped = true;
        }
    }
    IEnumerator AmbienceCoroutine()
    {
        while (true)
        {
            q1trigger = Random.Range(0, 2);
            q2trigger = Random.Range(0, 2);
            //print(this.gameObject.name + " " + "Q1: " + q1trigger);
            //print(this.gameObject.name + " " + "Q2: " + q2trigger);
            //print(this.gameObject.name + " " + "RTL: " + qs.readyToLeave);
            yield return new WaitForSeconds(10);
            if (this.gameObject.name == "Ambience1" || this.gameObject.name == "Ambience2")
            {
                if (sm.stage == "workday")
                {
                    //print("Heyoooo");
                    //print("Hi");
                    //if (npc.isStopped == true)
                    //{
                    //    print("So true bestie");
                    //}
                    int randomTarget = Random.Range(0, waypoints.Count);
                    if (npc.isStopped == true && randomTarget != prevTarget)
                    {
                        //print("We in");
                        npc.destination = waypoints[randomTarget].position;
                        npc.isStopped = false;
                    }
                    prevTarget = randomTarget;
                }
                ////print("Heyoooo");
                //yield return new WaitForSeconds(10);
                ////print("Hi");
                ////if (npc.isStopped == true)
                ////{
                ////    print("So true bestie");
                ////}
                //int randomTarget = Random.Range(0, waypoints.Count);
                //if (npc.isStopped == true && randomTarget != prevTarget)
                //{
                //    //print("We in");
                //    npc.destination = waypoints[randomTarget].position;
                //    npc.isStopped = false;
                //}
                //prevTarget = randomTarget;
                else if (sm.stage == "evening")
                {
                    //print("Suh suh");
                    if (this.gameObject.name == "Ambience1")
                    {
                        npc.isStopped = false;
                        npc.destination = AmbiRest1.position;
                    }
                    else if (this.gameObject.name == "Ambience2")
                    {
                        npc.isStopped = false;
                        npc.destination = AmbiRest2.position;
                    }
                }
                else if (sm.stage == "morning")
                {
                    //print("No wayooooo");
                    yield return new WaitForSeconds(10);
                }
            }
        }
    }
    //IEnumerator waiter()
    //{
    //    print("WHYYYYY");
    //    yield return new WaitForSeconds(2);
    //    qs.readyToLeave = 0;
    //    readyToLeaveLocal = 0;
    //    print("Hiya Man");
    //}
}

