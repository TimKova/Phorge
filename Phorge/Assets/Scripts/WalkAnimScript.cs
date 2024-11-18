using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkAnimScript : MonoBehaviour
{

    Animator animator;
    NavMeshAgent npc;
    public GameObject NPCModel;
    public enum walkState
    {
        idle,
        walk
    }
    walkState state = walkState.idle;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        npc = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
            if(npc.velocity.magnitude > 0)
        {
                print("Moving True");
                animator.SetInteger("Moving", 1);
                state = walkState.walk;
        } 
            if(npc.velocity.magnitude == 0)
            {
                print("Moving False");
                animator.SetInteger("Moving", 0);
                state = walkState.idle;
        }
    }
}
