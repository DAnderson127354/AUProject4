using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HutOpen : MonoBehaviour
{

        Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.SetBool("OpenDoor", true);
            Debug.Log("OpenDoor");

        }

    }

}
