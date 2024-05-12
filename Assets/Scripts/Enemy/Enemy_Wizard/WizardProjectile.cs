using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WizardProjectile : EnemyProjectile
{
    [SerializeField] private float rotationTime = 1f;
    private PlayerMovement player;
    private bool isLookingAtPlayer;

    //The start function is called when the projectile first spawns in
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerMovement>(); //Stores the player for future reference
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>(); //Stores the physics controller of the projectile for futrue reference

        //Makes the projectile rotate in place at first, and then shoot towards the player
        isLookingAtPlayer = true;
        StartCoroutine(TurnOffRotation());

        Destroy(gameObject, duration); //Force destroys the projectile if it does not hit anything to boost performance
    }

    void Update()
    {
        //Makes the projectiles stay stationary and simply turn towards the player for the before firing
        if (isLookingAtPlayer)
        {
            //Calculates the direction the projectile should face to "look at" the player
            Vector2 direction = player.transform.position - transform.position;
            float shootAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            transform.rotation = Quaternion.Euler(0, 0, shootAngle); //Rotates the projectile to face the player
        }
        else {
            transform.parent = null;
            rb.velocity = transform.up * speed; //Fires the projectiles at the player
        }
    }

    //Stops the projectile from turning after a while and makes it shoot towards the player
    private IEnumerator TurnOffRotation()
    {
        yield return new WaitForSeconds(rotationTime);
        isLookingAtPlayer = false;
    }
}
