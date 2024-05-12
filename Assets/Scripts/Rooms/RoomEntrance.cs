using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntrance : MonoBehaviour
{
    [SerializeField] private int roomNumber;
    private bool playerInRoom = false;

    private RoomManager roomManager;

    [SerializeField] private Transform telePosition;

    private void Start() {
        roomManager = FindObjectOfType<RoomManager>();
        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (roomNumber == roomManager.activeRoom) {
            playerInRoom = true;
        } else 
            playerInRoom = false;   
    }
    
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == 7) 
        {
            roomManager.activeRoom = roomNumber;
            if (!playerInRoom)
            {
                collision.transform.position = telePosition != null ? telePosition.position : transform.position;
                playerInRoom = true;
            }
            if (!roomManager.GetRoomStarted(roomNumber))
                roomManager.SetRoomStarted(roomNumber, true);

        }
    }
}
