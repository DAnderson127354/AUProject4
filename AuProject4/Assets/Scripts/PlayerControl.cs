using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    public float gravity;

    public CharacterController controller;
    public Animator anim;
    Vector3 playerVelocity;
    bool canJump;

    private bool autoLock = false;
    public float proximityAwareness;
    public float visionRange;
    RaycastHit hitInfo;
    int currentTarget = 0;

    public Camera playerCam;
    public GameObject playerEyes;

    public float stength = 1.5f;

    List<Transform> nearbyEnemies;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        AutoLock();
    }

    void PlayerMovement()
    {
        canJump = controller.isGrounded;

        if (canJump && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
            anim.SetBool("Moving", true);
        else
            anim.SetBool("Moving", false);

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            //UnityEngine.Debug.Log("jump");
            //anim.SetTrigger("Jump");
            playerVelocity.y = jumpHeight;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetTrigger("Attack1Trigger");
        }


        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void AutoLock()
    {
        nearbyEnemies = CheckCollisions();

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log(autoLock);
            if (autoLock || nearbyEnemies.Count == 0)
            {
                autoLock = false;
                RemoveLock();
            }
             else
            {
                if (nearbyEnemies.Count != 0)
                {
                    SetLockTo(nearbyEnemies[FindClosestTarget()]);
                    autoLock = true;
                }
            }
                
        }

        if (autoLock)
        {
           // Debug.Log("count " + nearbyEnemies.Count);
            if (nearbyEnemies.Count != 0)
            {
                if (currentTarget < 0 || currentTarget >= nearbyEnemies.Count)
                {
                    //Debug.Log("current target" + currentTarget);
                    currentTarget = 0;
                    return;
                }

                if (nearbyEnemies.Count > 1)
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        Debug.Log("switch");
                        currentTarget = (currentTarget + 1) % nearbyEnemies.Count;
                        SetLockTo(nearbyEnemies[currentTarget]);
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("switch");
                        if (currentTarget == 0)
                            currentTarget = nearbyEnemies.Count - 1;
                        else
                            currentTarget--;

                        SetLockTo(nearbyEnemies[currentTarget]);
                    }
                }
            }
            else
            {
                autoLock = false;
                RemoveLock();
            }   
        }
    }

    private int FindClosestTarget()
    {
        int closest = 0;
        foreach (Transform target in nearbyEnemies)
        {
            if (Vector3.Distance(target.position, transform.position) < Vector3.Distance(nearbyEnemies[closest].transform.position, transform.position))
            {
                closest = nearbyEnemies.IndexOf(target);
            }
        }

        return closest;
    }

    private void SetLockTo(Transform target)
    {
        playerCam.GetComponent<CameraControl>().SetTarget(target);
    }

    private void RemoveLock()
    {
        playerCam.GetComponent<CameraControl>().RemoveTarget();
    }

    private List<Transform> CheckCollisions()
    {
        List<Transform> nearbyEnemies = new List<Transform>();

        Collider[] otherObjectsInRadius = Physics.OverlapSphere(transform.position, proximityAwareness);

        foreach (var hitCollider in otherObjectsInRadius)
        {
            if (hitCollider.gameObject.tag == "Enemy")
            {
                if (Physics.Raycast(playerEyes.transform.position, (hitCollider.transform.position - transform.position), out hitInfo, visionRange) && (hitInfo.collider.tag == "Enemy" || hitInfo.collider.name == "Sword"))
                {
                    //Debug.Log("detected");
                    nearbyEnemies.Add(hitCollider.transform);
                }
                else
                {
                    nearbyEnemies.Remove(hitCollider.transform);
                }
            }
        }

        return nearbyEnemies;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.name == "Sword")
        {
            //player take damage
            Debug.Log("Damage taken");
        }
    }
}
