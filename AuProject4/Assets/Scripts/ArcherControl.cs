using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ArcherControl : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject animationControl;
    public Animator anim;
    public Transform eyes;

    public float proximityAwareness;
    public float visionRange;
    public float attackRadius;
    RaycastHit hitInfo;

    public GameObject player;
    public GameObject playerCam;
    private bool isFollowing = false;

    public float pauseBtwAttackTime;
    private float pauseBtwAttackTimer;

    public Slider healthBar;
    public float health = 5f;
    private float halfHealth;
    private float quarterHealth;

    private bool isAttacking = false;

    public float arrowSpeed;
    public GameObject arrow;
    public Transform archerHand;

    private List<Collider> nearbyEnemies = new List<Collider>();
    private bool enemiesCalled = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            anim.SetTrigger("Death");
        }
        else
        {
            CheckCollisions();

            if (isFollowing)
            {
                Vector3 lookVector = player.transform.position - transform.position;
                
                Quaternion rot = Quaternion.LookRotation(lookVector, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 2 * Time.deltaTime);
                isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1");
                Attack();
            }
        }
        

    }

    private void CheckCollisions()
    {
        Collider[] otherObjectsInRadius = Physics.OverlapSphere(transform.position, proximityAwareness);

        foreach (var hitCollider in otherObjectsInRadius)
        {
            if (hitCollider.gameObject.tag == "Enemy" && !hitCollider.gameObject.name.Contains("Archer"))
            {
                if (Vector3.Distance(hitCollider.transform.position, transform.position) <= visionRange && !nearbyEnemies.Contains(hitCollider))
                    nearbyEnemies.Add(hitCollider);
                else if (Vector3.Distance(hitCollider.transform.position, transform.position) > visionRange && nearbyEnemies.Contains(hitCollider))
                    nearbyEnemies.Remove(hitCollider);

                //Debug.Log(nearbyEnemies.Count);
            }

            if (hitCollider.gameObject.name == "Player")
            {
                if (Physics.Raycast(eyes.position, (hitCollider.transform.position - transform.position), out hitInfo, visionRange) && hitInfo.collider.name == "Player")
                {
                    isFollowing = true;
                    if (!enemiesCalled && nearbyEnemies.Count > 0)
                    {
                        anim.SetTrigger("Point");
                        enemiesCalled = true;
                        foreach (Collider enemy in nearbyEnemies)
                        {
                            Debug.Log(enemy.gameObject.name);
                            enemy.gameObject.GetComponent<EnemyControl>().FollowTarget(player.transform);
                        }
                    }
                }
                else
                {
                    isFollowing = false;
                    enemiesCalled = false;
                }
            }

            
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            if (pauseBtwAttackTimer <= 0)
            {
                GetComponent<Animator>().SetTrigger("Attack");
                //isAttacking = true;
                pauseBtwAttackTimer = pauseBtwAttackTime;
            }
            else
            {
                pauseBtwAttackTimer -= Time.deltaTime;
            }
        }
    }

    public void EnemyDeath()
    {
        gameObject.SetActive(false);
    }

    public void GetArrow()
    {
        arrow.SetActive(true);
    }

    public void Shoot()
    {
        Rigidbody clone;
        clone = Instantiate(arrow.GetComponent<Rigidbody>(), arrow.transform.position, Quaternion.identity);
        clone.transform.LookAt(player.transform);
        clone.velocity = clone.transform.forward * arrowSpeed;
        clone.gameObject.GetComponent<ProjectileControl>().enabled = true;
        clone.gameObject.GetComponent<SphereCollider>().enabled = true;
        clone.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 73.146f, 0);
        arrow.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerFist")
        {
            health -= player.GetComponent<PlayerControl>().stength;
            healthBar.value = health;

            GetComponent<Animator>().SetTrigger("Hit");
        }
    }
}
