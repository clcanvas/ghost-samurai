using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    private LevelManager levelManager;
    private GameManager gameManager;

    private void Start() {
        levelManager = FindAnyObjectByType<LevelManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (gameObject.tag == "End_game" && collision.gameObject.tag == "Player") {
            Camera.main.GetComponent<Animator>().SetTrigger("Shake");
            Camera.main.GetComponent<Animator>().speed = 8;
            Time.timeScale = .1f;
            StartCoroutine(EndGame());
        }
        else if (collision.gameObject.tag == "Player") {
            gameManager.UpdateTime();
            levelManager.LoadNextLevel();
        }
    }

    private IEnumerator EndGame() {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        gameManager.UpdateTime();
        levelManager.LoadNextLevel();
    }


}
