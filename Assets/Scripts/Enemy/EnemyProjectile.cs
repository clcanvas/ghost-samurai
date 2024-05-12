using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] public float speed = 10f;
    [SerializeField] public float duration = 5f;
    [SerializeField] private GameObject hitFX;
    public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, duration);
    }
    // Start is called before the first frame update
    void Update()
    {
        rb.velocity = transform.up * speed;
    }

    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnTriggerEnter2D(Collider2D trigger)
    {
        string tag = trigger.gameObject.tag;

        if (tag == "Player")
        {
            trigger.gameObject.GetComponentInParent<PlayerMovement>().Death();
            
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.seesPlayer = false;
                }
            }
            
        }
        if (tag != "Enemy" && tag != "Projectile" && rb.velocity.x != 0 && rb.velocity.y != 0)
        {
            Instantiate(hitFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }


}
