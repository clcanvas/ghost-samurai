using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class Menus : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private AudioSource hoverSound;

    [Header("Main Menu")]
    [SerializeField] private TMP_Text[] times;
    
    private string[] numbers;
    private LevelManager levelManager;
    private GameManager gameManager;

    private void Start() {
        levelManager = FindObjectOfType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
        numbers = new string[] {"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten"};

        if (times.Length > 0) {
            UpdateTimes();
        }

        if (GetComponent<PauseManager>() == null) {
            Cursor.visible = true;
        }
    }

    private void UpdateTimes() {
        float[] levelTimes = gameManager.levelTimes;
        Debug.Log("Updating Times");
        for (int i = 0; i < levelTimes.Length; i++) {
            if(levelTimes[i] > 0) {
                int minutes = (int)Mathf.Floor(levelTimes[i] / 60);
                int seconds = (int)Mathf.Floor(levelTimes[i]);
                int miliseconds = (int)Mathf.Floor((levelTimes[i] - 60 * minutes - seconds) * 1000);

                times[i].text = "Level " + numbers[i] + ": " + minutes + " m " + seconds + " s " + miliseconds + " ms";
            } else {
                times[i].text = "Level " + numbers[i] + ": " + "N/A";
            }
        }
    }

    public void StartGame()
    {   
        int level = 1;
        for (int i = 0; i < times.Length; i++)
        {
            if (gameManager.levelTimes[i] == 0)
            {
                level = i + 1;
                break;
            }
        }
        levelManager.LoadLevel(level);

            
    }

    public void RestartLevel() {
        GetComponent<PauseManager>().Resume();
        gameManager.ResetLevel();
        levelManager.ReloadLevel();
    }

    public void Menu() {
        if (GetComponent<PauseManager>()) {
            GetComponent<PauseManager>().Resume();
        }
        if (gameManager) {
            gameManager.ResetLevel();
        }
        levelManager.LoadMenu();
    }

    public void LoadFirstLevel() {
        levelManager.LoadLevel(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Hover()
    {
        hoverSound.Play();
    }
}
