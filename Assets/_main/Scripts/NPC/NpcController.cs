using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    public NpcType npcType;
    public NpcRoom npcRoom;

    public bool UseRandomSquare = false;
    private Vector3 TopLeft;
    private Vector3 BottomRight;

    [SerializeField]
    private bool WalkingToTarget = false;
    [SerializeField]
    private bool Waiting = false;
    [SerializeField]
    private NavMeshAgent NavMeshAgent;
    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private Vector3 LastVisted = new Vector3();

    private List<Vector3> TargetPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Awake()
    {
        SetDestinationsByType();
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
                if (Waiting == false)
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
            int randomIndex = Random.Range(0, TargetPositions.Count - 1 );
            randomTarget = TargetPositions[randomIndex];
        } while (randomTarget == LastVisted);
        return randomTarget;
    }

    void DecisionTree()
    {
        Vector3 targetPosition;
        if (UseRandomSquare == true)
        {
            targetPosition = GetRandomPositionInRectangle();
        } else
        {
            targetPosition = GetRandomTargetPosition();
        }
        WalkToTarget(targetPosition);
    }

    void WalkToTarget(Vector3 target)
    {
        NavMeshAgent.SetDestination(target);
        if (Animator != null)
        {
            Animator.SetBool("Walking", true);
        }
        WalkingToTarget = true;
        LastVisted = target;
    }

    private void SetDestinationsByType()
    {
        switch (npcRoom) 
        {
            case NpcRoom.SchoolHallway:
                Vector3 TargetPositionOne = new Vector3(15.846f, 0.54f, 23.33f);
                Vector3 TargetPositionTwo = new Vector3(15f, 0.716f, 11.097f);
                Vector3 TargetPositionThree = new Vector3(10.98f, 0.54f, 18.75f);
                Vector3 TargetPositionFour = new Vector3(20.67f, 0.48f, 13.67f);
                TargetPositions =  new List<Vector3> { TargetPositionOne, TargetPositionTwo, TargetPositionThree, TargetPositionFour };
                break;
            case NpcRoom.SchoolClassroomOne:
                TopLeft = new Vector3(-23.36f, 0.46f, 26.46f);
                BottomRight = new Vector3(-9.06f, 0.46f, 3.39f);
                // create a pick a random position from the rectangle defined by TopLeft and BottomRight

                Vector3 TargetPositionFive = new Vector3(-13.93f, 0.53f, 22.98f);
                Vector3 TargetPositionSix = new Vector3(-13.18f, 0.53f, 7.40f);
                Vector3 TargetPositionSeven = new Vector3(-22.45f, 0.53f, 10.59f);
                Vector3 TargetPositionEight = new Vector3(-9.22f, 0.53f, 10.59f);
                TargetPositions = new List<Vector3> { TargetPositionFive, TargetPositionSix, TargetPositionSeven, TargetPositionEight };
                break;
        }
    }

    private Vector3 GetRandomPositionInRectangle()
    {
        float randomX = Random.Range(TopLeft.x, BottomRight.x);
        float randomZ = Random.Range(BottomRight.z, TopLeft.z);
        float y = TopLeft.y; // Assuming the rectangle is on the same plane (y-coordinate is constant)

        return new Vector3(randomX, y, randomZ);
    }



    IEnumerator WaitForSeconds(float seconds)
    {
        Waiting = true;
        WalkingToTarget = false;
        if(Animator != null)
        {
            Animator.SetBool("Walking", false);
        }

        yield return new WaitForSeconds(seconds);
        Waiting = false;

    }

    [SerializeField]
    public enum NpcType
    {
        Wanderer,
        GoingToClass,
    }

    public enum NpcRoom
    {
        SchoolHallway,
        SchoolClassroomOne

    }
}