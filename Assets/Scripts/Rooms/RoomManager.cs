using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour
{
    [SerializeField] public Transform[] rooms;
    [SerializeField] private int levelNumber;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject doors;
    [SerializeField] private GameObject exit;
    
    public int activeRoom;
    private GameManager gameManager;
    private LevelManager levelManager;
    
    void Start() {
        levelManager = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.StartTimer(levelNumber);
        activeRoom = 1;
    }

    private void Update() {
        if (gameManager == null) {
             gameManager = FindObjectOfType<GameManager>();
        }
        int[] times = gameManager.LevelTime();
        if (timerText != null)
            timerText.text = times[0] + ":" + times[1] + ":" + times[2];
    }

    public void FinishedLastRoom() {
        gameManager.UpdateTime();
        levelManager.LoadNextLevel();
    }

    public bool GetRoomStarted(int roomNumber) {
        return rooms[roomNumber - 1].GetComponent<Room>().isStarted;
    }

    public void SetRoomStarted(int roomNumber, bool isStarted) {
        rooms[roomNumber - 1].GetComponent<Room>().isStarted = isStarted;
        if (isStarted) {
            activeRoom = roomNumber;
        }
    }

    public Vector2 GetRoomPosition() {
        return rooms[activeRoom - 1].position;
    }

    public void SetDoorsOpen(bool open) {
        doors.SetActive(!open);
    }

    public void SetExitOpen(bool open){
        exit.SetActive(open);
    }
}
