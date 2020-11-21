using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    public float gravity;

    //public float dashDistance;
    //public Slider dashFill;
    //public float Timer;
    //public GameObject dashButton;
    //public Sprite upSprite;
    //public Sprite downSprite;

    public CharacterController controller;
    //public Animator anim;
    Vector3 playerVelocity;
    bool canJump;
    //bool canDash;
    //bool dashing;

    // Start is called before the first frame update
    void Start()
    {
        //canDash = true;
        //dashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        /*if (dashing)
            Dash();

        if (dashFill.value < 1)
            dashFill.value += Time.deltaTime * Timer;
        else
        {
            dashButton.GetComponent<Image>().sprite = upSprite;
            dashButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Shift";
            canDash = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Shoot");
            //PlayerShoot();
        }*/
    }

    void PlayerMovement()
    {
        canJump = controller.isGrounded;

        //Dash control
       /* if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            anim.SetTrigger("Dash");
            dashing = true;
            dashButton.GetComponent<Image>().sprite = downSprite;
            dashButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Wait";
        }*/

        if (canJump && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        /*if (x != 0 || z != 0)
            anim.SetBool("Moving", true);
        else
            anim.SetBool("Moving", false);*/

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            //UnityEngine.Debug.Log("jump");
            //anim.SetTrigger("Jump");
            playerVelocity.y = jumpHeight;
        }


        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

   /* void PlayerShoot()          //Handles player shooting -- called during animation event
    {
        Debug.Log("shoot");

        Rigidbody clone;
        Vector3 startPosition;

        startPosition = Camera.main.ScreenToWorldPoint(crossHairs.position);

        clone = Instantiate(projectile, startPosition + Camera.main.transform.forward, Camera.main.transform.rotation);

        clone.transform.Rotate(0f, 90f, 0f);
        clone.velocity = Camera.main.transform.forward * shootSpeed;
        soundManager.Fire();
    }

    void Dash()
    {
        Vector3 dash = Camera.main.transform.forward * dashDistance;
        dash.y = 0f;
        controller.Move(dash * Time.deltaTime);

        dashFill.value -= Time.deltaTime * 2;

        canDash = false;

        if (dashFill.value == 0)
        {
            dashing = false;
            anim.SetBool("Moving", false);
        }

    }*/
}
