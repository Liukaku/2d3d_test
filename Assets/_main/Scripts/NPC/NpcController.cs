using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    public NpcType npcType;

    [SerializeField]
    private bool WalkingToTarget = false;
    [SerializeField]
    private bool Waiting = false;
    [SerializeField]
    private NavMeshAgent NavMeshAgent;
    [SerializeField]
    private Animator Animator;

    // default values for the target positions
    // this could be moved into a scriptable object or a database
    // for better organization and to avoid hardcoding values
    private Vector3 TargetPositionOne = new Vector3(15.846f, 0.54f, 23.33f);
    private Vector3 TargetPositionTwo = new Vector3(15f, 0.716f, 11.097f);
    private Vector3 TargetPositionThree = new Vector3(10.98f, 0.54f, 18.75f);
    private Vector3 TargetPositionFour = new Vector3(20.67f, 0.48f, 13.67f);

    [SerializeField]
    private Vector3 LastVisted = new Vector3();
    
    private List<Vector3> TargetPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Awake()
    {
        TargetPositions = new List<Vector3> { TargetPositionOne, TargetPositionTwo, TargetPositionThree, TargetPositionFour };
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        Debug.Log(npcType);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!WalkingToTarget && !Waiting)
        {
            DecisionTree();
            AlwaysFaceCamera();
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

    void AlwaysFaceCamera()
    {
        // make the NPC always face the camera
        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0; // keep the NPC upright
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    private Vector3 GetRandomTargetPosition()
    {
        // get a random target position from the dictionary and return the value if it isn't the last visited position
        Vector3 randomTarget;
        do
        {
            int randomIndex = Random.Range(0, TargetPositions.Count);
            randomTarget = TargetPositions[randomIndex];
        } while (randomTarget == LastVisted);
        return randomTarget;
    }

    void DecisionTree()
    {
        Vector3 targetPosition = GetRandomTargetPosition();
        WalkToTarget(targetPosition);
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

    [SerializeField]
    public enum NpcType
    {
        Wanderer,
        GoingToClass,
    }
}