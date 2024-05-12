using System.Collections;
using System.Net;
using System.Resources;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState {
        moving,
        waiting,
        shooting,
        reloading,
        stuckInWall
    }

    [Header("State")]
    public EnemyState state;
    [HideInInspector] public float stateTimer;
    [HideInInspector] public Animator animator;

    [Header("Movement")]
    [SerializeField] private float walkDistance = 4f;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float timeBtwMove = 3f;
    [HideInInspector] public Vector2 initPosition;
    [HideInInspector] public Rigidbody2D rb;
    private float walkAngle;
    private Vector2 targetPosition;
    [SerializeField] private bool shouldRotateShootPoint = true;

    [Header("Health")]
    [SerializeField] private int health = 1;
    [SerializeField] private float invincibilityTime = 0.6f;
    private bool invincible = false;

    [Header("Player Detection")]
    [SerializeField] public float timeToLoseFocus = 2f;
    [SerializeField] public LayerMask ignoreEnemy;
    [HideInInspector] public bool isPlayerInRange;
    [HideInInspector] public GameObject player;
     public bool seesPlayer;
    [HideInInspector] public float focusTimer;

    [Header("Shooting")]
    [SerializeField] private float reloadingTime = 1f;
    [SerializeField] private float shootInaccuracy = 10f;
    [SerializeField] public GameObject projectile;
    [SerializeField] public Transform shootPoint;
    [SerializeField] public Transform shootRotator;
    public float randomAngle;
    [HideInInspector] public bool readyToShoot;
    private float shootAngle;
    private float scaleX;

    [Header("Visual")]
    private SpriteRenderer rend;

    //The start function is called when the enemy first spawns in
    private void Start()
    {
        Init(); //Initializes variables for the enemy
    }

    //Initializes variables for the enemy
    public void Init()
    {
        rend = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<PlayerMovement>().gameObject; //Gets the player object for future reference
        rb = GetComponent<Rigidbody2D>(); //Gets the physics controller of the enemy for future reference
        animator = GetComponent<Animator>(); //Gets the animation controller of the enemy for future reference
        state = EnemyState.moving; //Enemy starts in the moving state
        scaleX = Mathf.Abs(transform.localScale.x); //Stores the size of the enemy on the x-axis. Can be inverted in the future to "flip" the enemy
        initPosition = transform.position; //Sets the inital position of the enemy's first movement
        targetPosition = FindDestination(); //Sets the final position of the enemy's first movement
        transform.localScale = new Vector2(targetPosition.x > transform.position.x ? -scaleX : scaleX, transform.localScale.y);
        readyToShoot = true; //Lets the enemy know it is able to shoot is the player comes in range
        randomAngle = Random.Range(0 , 359);
    }

    //Update is calles once per frame. This means that the number of times this function is called is dependent on the player device's framerate
    private void Update() {
        if (!PauseManager.isGamePaused) {
            Logic();
        }
        Detection(); //Checks if the enemy should begin attacking
    }

    //Contains the base state logic for the enemy
    public void Logic() {
        stateTimer += Time.deltaTime; //Used to measure how long the enemy has been in its current state

        //Performs the relevant action based on the enemies current state
        switch (state) {
            case EnemyState.moving: Move(); break;
            case EnemyState.waiting: Wait(timeBtwMove); break;
            case EnemyState.shooting: Shoot(); break;
            case EnemyState.reloading: Reload(); break;
            case EnemyState.stuckInWall: Wait(.5f); break;
        }

        if (state != EnemyState.shooting && state != EnemyState.reloading && shouldRotateShootPoint)
        {
            float currentAngle = shootRotator.eulerAngles.z;
            float rotateAmt = Mathf.Lerp(currentAngle, randomAngle, 5 * Time.deltaTime);
            if (Mathf.Abs(rotateAmt - randomAngle) > 1.0f)
            {
                shootRotator.rotation = Quaternion.Euler(0, 0, rotateAmt);
                
            } else 
            {
                randomAngle = Random.Range(0, 359);
            }
        }
            
    }

    //Contains the logic for the enemy when it is in the moving state
    private void Move() {
        rb.velocity = (targetPosition - (Vector2)transform.position).normalized * walkSpeed; //Sets the velocity vector of the enemy towards its destination
        

        //Forces the enemy to "give up" in the edge case of it trying to move to an impossible to reach destination
        if (stateTimer > 2f)
        {
            targetPosition = initPosition;
            initPosition = transform.position;
            stateTimer = 0;
        }

        //Sets the enemy to the waiting state after it has reached its destination or traveled its maximum distance
        if (Mathf.Abs(Vector2.Distance((Vector2)transform.position, targetPosition)) < .5f || Mathf.Abs(Vector2.Distance(initPosition, transform.position)) > walkDistance)
        {
            ChangeEnemyState(EnemyState.waiting);
        }
    }

    //Checks if the enemy should begin attacking
    public virtual void Detection() {
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
            if (IsHostile() && focusTimer > timeToLoseFocus) {
                ChangeEnemyState(EnemyState.moving);
                readyToShoot = true;
            }
        }
    }

    //Changes the enemy to its moving state after standing still for a certain amount of time. Allows the player to more easily sneak up on the enemy
    private void Wait(float waitTime) {
        rb.velocity = Vector2.zero; //Locks the enemy from moving
        
        //Sets the enemy state to the moving state after a specified amount of time
        if (stateTimer > waitTime)
        {
            initPosition = transform.position; //Sets the initial position of the enemy's next movement
            targetPosition = FindDestination(); //Sets the final position of the enemy's next movement
            if (!IsHostile())
                ChangeEnemyState(EnemyState.moving);
            else
                ChangeEnemyState(EnemyState.shooting);
        }
    }

    //Contains the logic for the enemy when it is in the shooting state
    public virtual void Shoot() {
        rb.velocity = Vector2.zero; //Locks the enemy from moving
        if (shootRotator.rotation != Quaternion.Euler(0, 0, shootAngle))
        {
            ChangeEnemyState(EnemyState.reloading);
            return;
        }
        //Calculates the position and rotation of the projectile based on the player's position and the enemy's accuracy
        shootRotator.rotation = Quaternion.Euler(0, 0, shootAngle+ Random.Range(-shootInaccuracy, shootInaccuracy));
        Instantiate(projectile, shootPoint.position, shootRotator.rotation); //Creates the projectile

        ChangeEnemyState(EnemyState.reloading); //Sets the enemy to the reloading state
    }

    //Contains the logic for the enemy when it is in the reloading state
    private void Reload() {
        rb.velocity = Vector2.zero; //Locks the enemy from moving
        Vector2 direction = player.transform.position - transform.position;
        if (seesPlayer)
            shootAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        shootRotator.rotation = Quaternion.Euler(0, 0, shootAngle);
        //Sets the enemy's state to shooting after the enemy is done reloading
        if (stateTimer > reloadingTime) {
            Debug.Log("Reloaded");
            ChangeEnemyState(EnemyState.shooting);
        }
    }

    //Sets the state of the enemy to shooting after 0.3 seconds
    public IEnumerator DelayAttack() {
        readyToShoot = false;
        yield return new WaitForSeconds(0.3f);
        if (state != EnemyState.reloading && state != EnemyState.shooting)
            ChangeEnemyState(EnemyState.shooting);
    }

    //Looks for a viable coordinate for the enemy to move to
    public Vector2 FindDestination()
    {
        int loopCounter = 0; //Counts the number of failed paths the enemy has looked thruogh
        Vector2 destination;
        //The loops will repeat until it ouputs a destination coordinate for the enemy
        do
        {
            //Selects a random angle as a potential direction to move towards
            walkAngle = Random.Range(0f, 2f * Mathf.PI);
            Vector2 walkDirection = new Vector2(Mathf.Cos(walkAngle), Mathf.Sin(walkAngle));

            destination = initPosition + walkDirection * walkDistance; //Calculates a coordinate a certain distance away in the random direction
            
            //Forces the enemy to move towards (0,0) if it takes more than five tries to find a viable path to save performance
            loopCounter++;
            if (loopCounter > 5)
            {
                destination = Vector2.zero;
                break;
            }
        } while (IsPathObstructed(destination)); //Repeats the loop is the "destination" coordinate is not reachable

        return destination; //Outputs a set of random coordinates that are reachable and are a certain distance away from the enemy
    }

    //Turns the enemy to face the right direction
    public void Orient() {
        //Makes sure the bug is facing in the right direction
        bool facingRight = (state == EnemyState.moving && initPosition.x < transform.position.x) || (IsHostile() && player.transform.position.x > transform.position.x);
        transform.localScale = new Vector2(facingRight ? -scaleX : scaleX, transform.localScale.y);
    }

    //Checks is the enemy has a clear path to a ceratin coordinate
    private bool IsPathObstructed(Vector2 destination)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, destination, walkDistance, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }

    //Called when the enemy 
    private void OnCollisionEnter2D(Collision2D collision) {

        //Checks if the enemy has moved inside a wall
        if (collision.gameObject.tag == "Wall")
        {
            ChangeEnemyState(EnemyState.stuckInWall); //Sets the enemy state to stuckInWall
            rb.velocity = Vector2.zero; //Stops the enemy from moving further into the wall
        }
    }

    public void Damage() {
        rb.velocity = Vector2.zero; //Stops the enemy
        if (!invincible)
        {
            health--; //Damages the enemy
            StartCoroutine(Invincibility()); //Gives the enemy temporary invincibility (i-frames) after getting hit
        }

        //Kills the enemy when it runs out of health
        if (health <= 0)
            Destroy(gameObject);

        StartCoroutine(DelayAttack());
        
    }

    //Gives the enemy temporary invinvibility (i-frames) after getting hit
    private IEnumerator Invincibility()
    {
        Debug.Log(gameObject.name + " Invicibility");
        invincible = true;   
        //Turns collssions off on the enemy to prevent it from getting hit
        GetComponent<BoxCollider2D>().isTrigger = true;
        //Halfs the opacity of the enemy to visually show its invincibility
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f);
        //Waits for a certain amount of time before ending the invincibility
        yield return new WaitForSeconds(invincibilityTime);
        invincible = false;
        GetComponent<BoxCollider2D>().isTrigger = false;
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 1f);

    }

    //Checks if the enemy is either shooting or reloading, the two hostile states
    public bool IsHostile() {
        return state == EnemyState.shooting || state == EnemyState.reloading;
    }

    //Checks if the enemy is ready to enter the shooting state
    public bool IsReadyToShoot() {
        return readyToShoot && (IsHostile() || isPlayerInRange); //NOTE: The "isPlayerInRange" variable is set by the EnemyVision script (The circle around the enemy)
    }

    //Changes the state of the enemy
    public void ChangeEnemyState(EnemyState state) {
        this.state = state; //Sets the enemy's state
        stateTimer = 0f; //Resets the amount of time the enemy has been in its current state
    }

    //Used for debugging and demonstration purposes
    private void OnDrawGizmos()
    {
        //Visually shows the line of sight of the enemy
        if (player != null)
            Gizmos.DrawRay(transform.position, player.transform.position - transform.position);
    }
}