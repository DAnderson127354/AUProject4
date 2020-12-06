using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BackUpEnemyControl : MonoBehaviour
{
    public Animator anim;
    //public NavMeshAgent agent;
    public CharacterController controller;
    public float speed;
    public ButtonFunction animationControl;
    public Transform eyes;
    public Collider sword;

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

    public float pauseBtwAttackTime;
    private float pauseBtwAttackTimer;
    private float pauseBtwBlockTimer;

    public GameObject player;
    public GameObject playerCam;
    private bool isFollowing = false;

    public Slider healthBar;
    public float health = 5f;
    private float halfHealth;
    private float quarterHealth;

    public bool isBlocking = false;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        movementTimer = SetTimer("Movement");
        pauseTimer = SetTimer("Pause");
        animationControl.Walk();
        halfHealth = (health / 2);
        quarterHealth = (health * 0.25f);
        //agent.SetDestination(routePoints[currentPoint].position);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.transform.LookAt(playerCam.transform);

        if (health <= 0)
        {
            anim.SetTrigger("Death");
        }
        else
        {
            if (!isFollowing)
                Movement();

            CheckCollisions();
        }
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
            //transform.LookAt(routePoints[currentPoint]);
            LookAtTarget(routePoints[currentPoint].position);
            var forward = transform.TransformDirection(Vector3.forward);
            if (Physics.Raycast(transform.position, forward, out hitInfo, 3))
            {
                if (hitInfo.collider.tag == "Obstacle")
                {
                    forward = transform.TransformDirection(Vector3.right);
                }
                
            }
            controller.SimpleMove(forward * speed);
        }

        if (Vector3.Distance(transform.position, routePoints[currentPoint].position) < 0.5f)
        {
            GoToNextPoint();
        }
        

        /*if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }*/
    }

    public void LookAtTarget(Vector3 target)
    {
        Vector3 lookVector = target - transform.position;

        Quaternion rot = Quaternion.LookRotation(lookVector, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2 * Time.deltaTime);
    }

    private void Pause()
    {
        if (pauseTimer > 0)
        {
            //agent.isStopped = true;
            pauseTimer -= Time.deltaTime;
            animationControl.Idle();
        }
        else
        {
            //agent.isStopped = false;
            movementTimer = SetTimer("Movement");
            pauseTimer = SetTimer("Pause");
            animationControl.Walk();
        }

    }

    private float SetTimer(string timer)
    {
        if (timer == "Movement")
        {
            return Random.Range(minMovementTime, maxMovementTime);
        }
        else if (timer == "AttackPause")
        {
            return pauseBtwAttackTime;
        }

        return pauseTime;
    }

    private void GoToNextPoint()
    {
        // Returns if no points have been set up
        if (routePoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        //agent.destination = routePoints[currentPoint].position;
        // transform.LookAt(routePoints[currentPoint]);
        LookAtTarget(routePoints[currentPoint].position);

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
                Debug.DrawRay(eyes.position, (hitCollider.transform.position - transform.position), Color.blue);
                if (Physics.Raycast(eyes.position, (hitCollider.transform.position - transform.position), out hitInfo, visionRange) && hitInfo.collider.name == "Player")
                {
                    //Debug.Log(hitInfo.collider);
                    if (health >= halfHealth)
                    {
                        isFollowing = true;
                        if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius)
                        {
                            EngageWithPlayer(1);
                        }
                        else
                        {
                            FollowTarget(player.transform);
                            anim.SetBool("Circle", false);
                        }
                    }
                    else if (health >= quarterHealth)
                    {
                        isFollowing = true;
                        if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius)
                        {
                            EngageWithPlayer(4);
                        }
                        else
                        {
                            FollowTarget(player.transform);
                            anim.SetBool("Circle", false);
                        }
                    }
                    else
                    {
                        anim.SetBool("Circle", false);
                        //agent.isStopped = false;
                        animationControl.SprintJump();
                        isBlocking = false;
                        isAttacking = false;
                        Run();
                    }

                }
                else
                {
                    //agent.isStopped = false;
                    isFollowing = false;
                }

            }
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            if (pauseBtwAttackTimer <= 0 && !isBlocking)
            {
                anim.SetTrigger("Attack1");
                pauseBtwAttackTimer = 1f;
            }
            else
            {
                pauseBtwAttackTimer -= Time.deltaTime;
            }
        }


    }

    private void Block()
    {
        //enemy blocking
        if (!isBlocking)
        {
            if (pauseBtwBlockTimer <= 0 && !isAttacking)
            {
                anim.SetTrigger("Block");
                pauseBtwBlockTimer = anim.GetCurrentAnimatorStateInfo(1).length;
            }
            else
            {
                pauseBtwBlockTimer -= Time.deltaTime;
            }
        }

    }

    public void Hit()
    {
        pauseBtwAttackTimer = SetTimer("AttackPause");
        //maybe use for sound effects later?
    }

    public void EnableWeaponCollision()
    {
        sword.enabled = true;
    }

    public void DisableWeaponCollision()
    {
        sword.enabled = false;
    }

    public void Death()
    {
        gameObject.SetActive(false);
    }

    private void EngageWithPlayer(float attackLikiness)
    {
        //agent.isStopped = true;

        if (!isAttacking && !isBlocking)
        {
            transform.RotateAround(player.transform.position, Vector3.up, 20 * Time.deltaTime);
            anim.SetBool("Circle", true);
        }


        if (Random.Range(0, 5) >= attackLikiness)
            Attack();
        else
            Block();

        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Block"))
        {
            anim.SetBool("Circle", false);
            animationControl.Idle();
            //Debug.Log("blocking");
            isBlocking = true;
            // agent.isStopped = true;
            Vector3 lookVector = player.transform.position - transform.position;

            Quaternion rot = Quaternion.LookRotation(lookVector, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1 * Time.deltaTime);
            //transform.LookAt(player.transform);
        }
        else
        {
            isBlocking = false;
            //agent.isStopped = false;
        }


        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            anim.SetBool("Circle", false);
            isAttacking = true;
            // agent.isStopped = true;
            Vector3 lookVector = player.transform.position - transform.position;

            Quaternion rot = Quaternion.LookRotation(lookVector, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1 * Time.deltaTime);
            //transform.LookAt(player.transform);
        }
        else
        {
            isAttacking = false;
            // agent.isStopped = false;
        }

    }

    public void FollowTarget(Transform target)
    {
        //agent.isStopped = false;
        animationControl.SprintJump();
        transform.LookAt(target);
        var forward = transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * speed);
        //agent.SetDestination(target.position);
    }

    private void Run()
    {
        Debug.Log("run");

        Vector3 posDiff = transform.position - player.transform.position;   //Calculates the difference in position between player and NPC
        Vector3 destination = transform.position + posDiff; //Creates a new destination, based on the difference in position. 
        //transform.LookAt(destination);
        LookAtTarget(destination);
        var forward = transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * speed);
        //agent.SetDestination(destination);  //Sets the new destination for the NPC, and actually makes it run away.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerFist")
        {
            if (!isBlocking)
            {
                health -= player.GetComponent<PlayerControl>().stength;
                healthBar.value = health;
            }

            anim.SetTrigger("Hit");
        }
    }
}





