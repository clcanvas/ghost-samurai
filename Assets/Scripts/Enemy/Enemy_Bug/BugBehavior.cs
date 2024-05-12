using UnityEngine;

public class BugBehavior : Enemy
{
    //The start function is called when the enemy first spawns in
    private void Start()
    {
        Init(); //Initializaes general variables for the enemy
    }

    //Update is calles once per frame. This means that the number of times this function is called is dependent on the player device's framerate
    private void Update()
    {
        Logic(); //Contains the base state logic for the enemy
        Detection(); //Checks if the enemy should begin attacking
        Orient(); //Turns the enemy to face the right direction

        animator.SetBool("IsMoving", state == EnemyState.moving); //Plays the moving animation for the bug
    }
    
    //Contains the logic for the enemy when it is in the shooting state
    public override void Shoot()
    {
        rb.velocity = Vector2.zero; //Locks the enemy from moving

        //Creates the projectile

        float distance = Vector2.Distance(player.transform.position, transform.position);
        if (distance != 10000) {
            Debug.Log("Rotating ");
            Vector2 direction = player.transform.position - shootRotator.position;
            float shootAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            shootRotator.rotation = Quaternion.Euler(0, 0, shootAngle);
        }
        Transform bullet = Instantiate(projectile, transform).transform;
        bullet.position = shootPoint.position;
        bullet.rotation = shootRotator.rotation;
        ChangeEnemyState(EnemyState.reloading); //Sets the enemy to the reloading state
    }

}
