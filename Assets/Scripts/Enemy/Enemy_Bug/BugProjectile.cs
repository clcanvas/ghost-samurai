using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Purchasing;

public class BugProjectile : MonoBehaviour
{
    [SerializeField] private float jumpUpForce = 2f;
    [SerializeField] private float maxSpeed = 7;
    [SerializeField] private GameObject residue;
    private Transform parentBug;
    private Rigidbody2D rb;
    private Transform player;
    private float speed;
    private float targetHeight;
    private float targetX = 0;
    private float distance = 10;

    //The start function is called when the projectile first spawns in
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;//Stores the player for future reference
        if (player.GetComponent<PlayerMovement>() == null)
            player = player.GetComponentInParent<Transform>(); 
        distance = Vector2.Distance(player.transform.position, transform.position);
        Debug.Log(distance);
        rb = GetComponent<Rigidbody2D>(); //Stores the projectile's physics controller for future reference
        parentBug = transform.parent; //Stores the bug that spawned the projectile for future reference
        if (distance != 1000)
        { 
            rb.velocity = new Vector2(0, jumpUpForce); //Sends an upward velocity to the projectile
            targetHeight = player.position.y; //Sets the final height of the projectile
            speed = Mathf.Clamp(Vector2.Distance(player.transform.position, parentBug.position), 0.5f, maxSpeed) + (float)Random.Range(-2f, -.5f); //Calculates the speed of the projectile
            
        } else {
            targetHeight = player.position.y;
            speed = 7f;
            targetX = player.position.x;
            rb.gravityScale = 0;

        }
        transform.parent = null; //Seperates the projectile from the bug in the hierarchy
        Destroy(gameObject, 10);
    }

    //Update is calles once per frame. This means that the number of times this function is called is dependent on the player device's framerate
    void Update()
    {


        //When the projectile lands, it destroys itsef and creates leftover damage-dealing residue
        if (distance != 1000 )
        {
            float xVelocity = transform.up.x * (speed / 1.8f);
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
            if ((transform.position.y <= targetHeight - .5f) && rb.velocity.y < 0) {
                Instantiate(residue, transform.position, Quaternion.Euler(0, 0, 0));
                Destroy(gameObject); 
            }
        } else { 
            rb.velocity = transform.up * speed;
            if (transform.position.y >= targetHeight - .5f && transform.position.y <= targetHeight + .5f && transform.position.x <= targetX + .2f && transform.position.x >= targetX - .2f) 
            {
                Instantiate(residue, transform.position, Quaternion.Euler(0, 0, 0));
                Destroy(gameObject); 
            } 
        }
    }
}
