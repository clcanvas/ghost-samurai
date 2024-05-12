using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int numberOfLevels;
    public float[] levelTimes;

    public int currentLevel;
    public float levelTimer;

    private void Awake() {
        if(FindObjectsOfType<GameManager>().Length > 1) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
        Cursor.visible = true;
    }

    private void Start() {
        levelTimes = new float[numberOfLevels];
    }

    private void Update() {
        levelTimer += Time.deltaTime;
    }

    public void ResetLevel() {
        currentLevel = 0;
    }

    public void StartTimer(int levelNumber) {
        Debug.Log("Start Timer: " + levelNumber);
        Debug.Log("Current Level: " + currentLevel);
        if (currentLevel != levelNumber) {
            currentLevel = levelNumber;
            levelTimer = 0;
        }
    }

    public int[] LevelTime() {
        int minutes = (int)Mathf.Floor(levelTimer / 60);
        int seconds = (int)Mathf.Floor(levelTimer % 60);
        int miliseconds = (int)Mathf.Floor((levelTimer - 60 * minutes - seconds) * 1000);
        int[] times = {minutes, seconds, miliseconds};

        return times;
    }

    public void UpdateTime() {
        if (levelTimes[currentLevel - 1] == 0 || levelTimer < levelTimes[currentLevel - 1]) {
            levelTimes[currentLevel - 1] = levelTimer;
        }
    }
}
