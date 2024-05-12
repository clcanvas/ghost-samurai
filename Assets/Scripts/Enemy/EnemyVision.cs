using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    private Enemy enemy;

    private void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.tag == "Player")
        {
            enemy.isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.gameObject.tag == "Player")
        {
            enemy.isPlayerInRange = false;
        }
    }
}
