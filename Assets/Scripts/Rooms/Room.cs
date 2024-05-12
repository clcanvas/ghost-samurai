using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private int roomNumber;

    private RoomManager roomManager;
    [HideInInspector] public bool isStarted;
    private bool isRoomCleared;

    private int initialChildCount = 0;
    private int enemyCount = 0;
    private bool finishedCounting = false;

    private void Start() {
        isRoomCleared = false;
        roomManager = FindObjectOfType<RoomManager>();
        isStarted = roomNumber == 1;
        initialChildCount = transform.childCount;
    }

    private void Update() {
        if (finishedCounting && !isRoomCleared && transform.childCount == initialChildCount - enemyCount) {
            isRoomCleared = true;
        }
        if (isStarted && !finishedCounting) {
            foreach (Transform child in transform) {
                if (child.tag == "Enemy")
                {
                    child.gameObject.SetActive(true);
                    enemyCount++;
                }
            }

            finishedCounting = true;
            
        }
        if (roomManager.activeRoom == roomNumber) {
            roomManager.SetDoorsOpen(isRoomCleared);
            if(roomNumber == roomManager.rooms.Length) {
                roomManager.SetExitOpen(isRoomCleared);
            }
        }
    }
}
