using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private float secondsBeforeEndScreen = 2.0f;

    private bool levelEnded = false;

    private GameObject endScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.endScreen = GameObject.Find("EndScreen");
    }

    private void Start()
    {
        LevelTimer.instance.onTimerCompleted -= this.EndLevel;
        LevelTimer.instance.onTimerCompleted += this.EndLevel;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (this.levelEnded == true)
            {
                SceneLoader.instance.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    private void EndLevel()
    {
        this.levelEnded = true;
        StartCoroutine(this.DisplayEndScreenAfterDelay());
    }

    private IEnumerator DisplayEndScreenAfterDelay()
    {
        yield return new WaitForSecondsRealtime(this.secondsBeforeEndScreen);
        this.DisplayEndScreen();
    }

    private void DisplayEndScreen()
    {
        this.endScreen.SetActive(true);
    }
}
