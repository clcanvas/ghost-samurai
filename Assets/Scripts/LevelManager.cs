using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float animTime = 1/6f;
    [SerializeField] private Animator transition;
    [SerializeField] private GameObject wipeScreen;

    private PauseManager pauseManager;

    void Awake()
    {
        wipeScreen.SetActive(true);
    }

    private void Start() {
        pauseManager = FindObjectOfType<PauseManager>();
    }

    public void LoadLevel(int buildIndex) {
        wipeScreen.SetActive(true);
        transition.SetTrigger("Start");
        StartCoroutine(ChangeScene(buildIndex));
    }

    private IEnumerator ChangeScene(int buildIndex) {
        yield return new WaitForSeconds(animTime);
        SceneManager.LoadScene(buildIndex);
    }

    public void LoadMenu() {
        Time.timeScale = 1f;
        LoadLevel(0); 
    }

    public void ReloadLevel() {
        Time.timeScale = 1f;
        Debug.Log("Restarting Level...");
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel() {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
