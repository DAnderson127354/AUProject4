using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;

    public float proximityAwareness;
    public float visionRange;
    public float attackRadius;
    RaycastHit hitInfo;

    public Transform[] routePoints;
    public int currentPoint;

    public float maxMovementTime;
    public float minMovementTime;
    public float pauseTime;
    private float movementTimer;
    private float pauseTimer;

    public GameObject player;
    private bool isFollowing = false;

    // Start is called before the first frame update
    void Start()
    {
        movementTimer = SetTimer("Movement");
        pauseTimer = SetTimer("Pause");
        anim.SetBool("Moving", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFollowing)
            Movement();

        CheckCollisions();
    }

    private void Movement()
    {
        if (movementTimer <= 0)
        {
            Pause();
        }
        else
        {
            movementTimer -= Time.deltaTime;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    private void Pause()
    {
        if (pauseTimer > 0)
        {
            agent.isStopped = true;
            pauseTimer -= Time.deltaTime;
            anim.SetBool("Moving", false);
        }
        else
        {
            agent.isStopped = false;
            movementTimer = SetTimer("Movement");
            pauseTimer = SetTimer("Pause");
            anim.SetBool("Moving", true);
        }
            
    }

    private float SetTimer(string timer)
    {
        if (timer == "Movement")
        {
            return Random.Range(minMovementTime, maxMovementTime);
        }

        return pauseTime;
    }

    private void GoToNextPoint()
    {
        // Returns if no points have been set up
        if (routePoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = routePoints[currentPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        currentPoint = (currentPoint + 1) % routePoints.Length;
    }

    private void CheckCollisions()
    {
        Collider[] otherObjectsInRadius = Physics.OverlapSphere(transform.position, proximityAwareness);

        foreach (var hitCollider in otherObjectsInRadius)
        {
            if (hitCollider.gameObject.name == "Player")
            {
                if (Physics.Raycast(transform.position, (hitCollider.transform.position - transform.position), out hitInfo, visionRange))
                {
                    isFollowing = true;
                    if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius)
                    {
                        Attack();
                    }
                    else
                    {
                        FollowTarget(player.transform);
                    }
                }
                else
                    isFollowing = false;
            }
        }
    }

    private void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(player.transform);
        anim.SetTrigger("Attack1Trigger");
    }

    private void FollowTarget(Transform target)
    {
        agent.isStopped = false;
        anim.SetBool("Moving", true);
        agent.SetDestination(target.position);
    }
}
