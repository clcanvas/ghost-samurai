using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    //Called every frame that the residue is hitting an object
    void OnTriggerStay2D(Collider2D other)
    {
        //Kills the player if they step in the residue
        PlayerMovement player = other.gameObject.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            player.Death();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Kills the player if they step in the residue
        PlayerMovement player = other.gameObject.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            player.Death();
        }
    }
}
