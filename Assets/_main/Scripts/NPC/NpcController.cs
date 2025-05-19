using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    [SerializeField]
    private bool WalkingToTarget = false;
    [SerializeField]
    private bool Waiting = false;
    [SerializeField]
    private NavMeshAgent NavMeshAgent;
    [SerializeField]
    private Animator Animator;
    [SerializeField]
    private Vector3 TargetPositionOne = new Vector3(15.846f, 0.54f, 23.33f);
    [SerializeField]
    private Vector3 TargetPositionTwo = new Vector3(15f, 0.716f, 11.097f);
    [SerializeField]
    private Vector3 LastVisted = new Vector3();

    // Start is called before the first frame update
    void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!WalkingToTarget && !Waiting)
        {
            DecisionTree();
        }
        else
        {
            if (WalkingToTarget && Vector3.Distance(transform.position, LastVisted) < 0.5f)
            {
                if(Waiting == false)
                {
                    StartCoroutine(WaitForSeconds(5f));
                }
            }
        }
    }

    void DecisionTree()
    {
        if (LastVisted == TargetPositionOne)
        {
            WalkToTarget(TargetPositionTwo);
        }
        else if (LastVisted == TargetPositionTwo)
        {
            WalkToTarget(TargetPositionOne);
        }
        else
        {
            WalkToTarget(TargetPositionOne);
        }
    }

    void WalkToTarget(Vector3 target)
    {
        NavMeshAgent.SetDestination(target);
        Animator.SetBool("Walking", true);
        WalkingToTarget = true;
        LastVisted = target;
    }

    IEnumerator WaitForSeconds(float seconds)
    {
        Waiting = true;
        WalkingToTarget = false;
        Animator.SetBool("Walking", false);
        Debug.Log("Waiting for " + seconds + " seconds...");
        yield return new WaitForSeconds(seconds);
        Waiting = false;
        Debug.Log("Finished waiting.");
    }


}
