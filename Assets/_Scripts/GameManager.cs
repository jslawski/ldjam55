using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private float secondsBeforeEndScreen = 1.0f;

    private bool levelEnded = false;
    
    [SerializeField]
    private GameObject endScreen;

    private AudioClip[] allMusic;
    private AudioChannelSettings musicSettings;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.allMusic = Resources.LoadAll<AudioClip>("Soundtracks");
        this.musicSettings = new AudioChannelSettings(true, 1.0f, 1.0f, 0.5f, "BGM");
    }

    private void Start()
    {
        LevelTimer.instance.onTimerCompleted -= this.EndLevel;
        LevelTimer.instance.onTimerCompleted += this.EndLevel;

        this.PlayRandomSong();
    }

    private void PlayRandomSong()
    {
        int randomIndex = Random.Range(0, this.allMusic.Length);
        AudioManager.instance.Play(this.allMusic[randomIndex], this.musicSettings);
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

        string playerName = PlayerPrefs.GetString("name", "");

        if (playerName != "")
        {
            LeaderboardManager.instance.QueueLeaderboardUpdate(playerName, ScoreKeeper.instance.GetPersonalBestScore(), LevelList.GetCurrentLevel().sceneName);
        }
        
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
