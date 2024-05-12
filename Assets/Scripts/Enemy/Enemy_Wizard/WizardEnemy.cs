using System.Collections;
using UnityEngine;

public class WizardEnemy : Enemy
{

    [Header("Extra Shoot Positions")]
    [SerializeField] public Transform shootPoint2;
    [SerializeField] public Transform shootPoint3;
    [SerializeField] public Transform shootPoint4;
    private Transform[] shootPoints;

    [Header("Cooldowns")]
    [SerializeField] private float projectileSpawnTime = .125f;

    //The start function is called when the enemy first spawns in
    private void Start()
    {
        shootPoints = new Transform[4] { shootPoint, shootPoint2, shootPoint3, shootPoint4 }; //Creates an array of the 
        Init(); //Initializes variables for the enemy
    }

    //Update is calles once per frame. This means that the number of times this function is called is dependent on the player device's framerate
    private void Update() {
        Logic(); //Contains the base state logic for the enemy
        Detection(); //Checks if the enemy should begin attacking
        Orient(); //Turns the enemy to face the right direction
        
        if (state != EnemyState.shooting && state != EnemyState.reloading)
            animator.SetBool("isAttacking", false);

        //Debug.Log("Sees Player " + seesPlayer);
        //Debug.Log("Hostile " + IsHostile());
         //Plays the shooting animation for the wizard
    }

    //Contains the logic for the enemy when it is in the shooting state
    public override void Shoot() {
        rb.velocity = Vector2.zero; //Locks the enemy from moving
        //Shoots four projectiles (one round) with a slight delay between them
        StartCoroutine(ShootOneRound());
        animator.SetBool("isAttacking", true);
       
    }

    //Shoots four projectiles (one round) with a slight delay between them
    private IEnumerator ShootOneRound() {
        //Spawns each of the four projectiles with a small delay between them    
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            foreach (Transform point in shootPoints) {
                ChangeEnemyState(EnemyState.reloading);
                Transform bullet = Instantiate(projectile, transform).transform;
                bullet.localPosition = point.localPosition;
                bullet.localRotation = point.localRotation;
                yield return new WaitForSeconds(projectileSpawnTime); //Delays the creation of the next projectile by a brief amount to create a more interesting attack pattern
            }
            yield return new WaitForSeconds(0.5f);
            ChangeEnemyState(EnemyState.reloading);
        }
        animator.SetBool("isAttacking", false);
    }

    public override void Detection() {
        //Checks is the enemy has line of sight to the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, 100f, layerMask: ignoreEnemy);
    
        if (hit.collider != null)
            seesPlayer = hit.collider.gameObject.tag == "Player";

        if (seesPlayer) {
            focusTimer = 0f; //Resets "focus" timer
            //When the enemy has line of sight and the player enters its field of vision, the emeny will begin to attack
            if (IsReadyToShoot()) {
                StartCoroutine(DelayAttack()); //Delays shooting the player by a certain amount of time to allow for easier dodging
            }
        }
        else {
            focusTimer += Time.deltaTime; //Increases the "focus" timer

            //When the enemy is attacking, but can not see the player for a certain period of time, it will lose focus and stop attacking
            if (state == EnemyState.reloading && focusTimer > timeToLoseFocus) {
                ChangeEnemyState(EnemyState.moving);
                readyToShoot = true;
            }
        }
    }
}
