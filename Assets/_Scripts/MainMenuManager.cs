using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private AudioClip[] allMusic;
    private AudioChannelSettings musicSettings;

    private void Start()
    {
        this.allMusic = Resources.LoadAll<AudioClip>("Soundtracks");
        this.musicSettings = new AudioChannelSettings(true, 1.0f, 1.0f, 0.5f, "BGM");

        CursorManager.instance.SetCursorToBlack();

        this.PlayRandomSong();
    }

    private void PlayRandomSong()
    {
        int randomIndex = Random.Range(0, this.allMusic.Length);
        AudioManager.instance.Play(this.allMusic[randomIndex], this.musicSettings);
    }

    public void PlayButtonClicked()
    {
        SceneLoader.instance.LoadScene("LevelSelect");
    }

    public void CloseButtonClicked()
    {
        SceneLoader.instance.CloseGame();
    }
}
