using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavScript : MonoBehaviour
{
    public Transform waypoint;
    private NavMeshAgent npc;
    // Start is called before the first frame update
    void Start()
    {
        npc = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        npc.destination = waypoint.position;
    }
}
