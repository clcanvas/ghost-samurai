using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Purchasing;

public class PauseManager : MonoBehaviour
{
    [HideInInspector] public static bool isGamePaused = false;

    [SerializeField] private GameObject pauseMenu;
    public bool paused;
    private bool canUnpause = false;
    private bool canPause = true;

    private void Start() {
        isGamePaused = false;
    }

    private void Update() {
        paused = isGamePaused;
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (canUnpause && isGamePaused) {
                Resume();
                
            } else if (canPause) {
                canUnpause = false;
                canPause = false;
                StartCoroutine(ToggleUnpause());
                Pause();
            }
        }
    }

    private void Pause() {
        
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0f;
        isGamePaused = true;
        StartCoroutine(TogglePause());
    }

    public void Resume() {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    private IEnumerator ToggleUnpause() {
        yield return new WaitForSecondsRealtime(0f);
        canUnpause = true;
    }

    private IEnumerator TogglePause() {
        yield return new WaitForSecondsRealtime(0f);
        canPause = true;
    }
}
