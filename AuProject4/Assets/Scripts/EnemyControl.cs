using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyControl : MonoBehaviour
{
    //public Animator anim;
    public NavMeshAgent agent;
    public GameObject animationControl;
    public Transform eyes;

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
        animationControl.GetComponent<ButtonFunction>().Walk();
        //anim.SetBool("Moving", true);
        halfHealth = (health / 2);
        quarterHealth = (health * 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.transform.LookAt(playerCam.transform);

        if (health <= 0)
        {
            GetComponent<Animator>().SetTrigger("Death");
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
            animationControl.GetComponent<ButtonFunction>().Idle();
            //anim.SetBool("Moving", false);
        }
        else
        {
            agent.isStopped = false;
            movementTimer = SetTimer("Movement");
            pauseTimer = SetTimer("Pause");
            animationControl.GetComponent<ButtonFunction>().Walk();
            //anim.SetBool("Moving", true);
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
                            animationControl.GetComponent<Animator>().SetBool("Circle", false);
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
                            animationControl.GetComponent<Animator>().SetBool("Circle", false);
                        }
                    }
                    else
                    {
                        animationControl.GetComponent<Animator>().SetBool("Circle", false);
                        agent.isStopped = false;
                        animationControl.GetComponent<ButtonFunction>().SprintJump();
                        isBlocking = false;
                        isAttacking = false;
                        Run();
                    }
                    
                }
                else
                    isFollowing = false;
            }
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            if (pauseBtwAttackTimer <= 0 && !isBlocking)
            {
                GetComponent<Animator>().SetTrigger("Attack1");
                //isAttacking = true;
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
                GetComponent<Animator>().SetTrigger("Block");
                //isBlocking = true;
                pauseBtwBlockTimer = GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).length;
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

    public void EnemyDeath()
    {
        gameObject.SetActive(false);
    }

    private void EngageWithPlayer(float attackLikiness)
    {
        agent.isStopped = true;

        if (!isAttacking && !isBlocking)
        {
            transform.RotateAround(player.transform.position, Vector3.up, 20 * Time.deltaTime);
            //transform.LookAt(player.transform);
            animationControl.GetComponent<Animator>().SetBool("Circle", true);
        }
        

        if (Random.Range(0, 5) >= attackLikiness)
            Attack();
        else
            Block();

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Block"))
        {
            animationControl.GetComponent<Animator>().SetBool("Circle", false);
            animationControl.GetComponent<ButtonFunction>().Idle();
            //Debug.Log("blocking");
            isBlocking = true;
            // agent.isStopped = true;
            transform.LookAt(player.transform);
        }
        else
        {
            isBlocking = false;
            //agent.isStopped = false;
        }
            

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            animationControl.GetComponent<Animator>().SetBool("Circle", false);
            isAttacking = true;
           // agent.isStopped = true;
            transform.LookAt(player.transform);
        }
        else
        {
            isAttacking = false;
           // agent.isStopped = false;
        }
            
    }

    private void FollowTarget(Transform target)
    {
        agent.isStopped = false;
        animationControl.GetComponent<ButtonFunction>().SprintJump();
        //anim.SetBool("Moving", true);
        agent.SetDestination(target.position);
    }

    private void Run()
    {
        Debug.Log("run");
        Vector3 posDiff = transform.position - player.transform.position;   //Calculates the difference in position between player and NPC
        Vector3 destination = transform.position + posDiff; //Creates a new destination, based on the difference in position. 

        agent.SetDestination(destination);  //Sets the new destination for the NPC, and actually makes it run away.
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
            
            GetComponent<Animator>().SetTrigger("Hit");
        }
    }
}
