using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

        Animator anim;
    public GameObject player;

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
                anim.SetBool("Hover", true);
                Debug.Log("Key");

            }

        }

    public void PlaceInInventory()
    {
        player.GetComponent<PlayerControl>().AddToInventory(gameObject);
        gameObject.SetActive(false);
    }

}