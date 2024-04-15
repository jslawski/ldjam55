using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSummary : MonoBehaviour
{
    private Level associatedLevel;

    [SerializeField]
    private TextMeshProUGUI levelIndexText;
    [SerializeField]
    private TextMeshProUGUI levelName;

    [SerializeField]
    private TextMeshProUGUI playerRank;
    [SerializeField]
    private TextMeshProUGUI playerScore;

    private AudioClip openSound;
    private AudioClip confirmSound;
    private AudioClip closeSound;

    [SerializeField]
    private TextMeshProUGUI neutralBallsCountText;
    [SerializeField]
    private TextMeshProUGUI goodBallsCountText;
    [SerializeField]
    private TextMeshProUGUI badBallsCountText;

    private AudioChannelSettings audioChannelSettings;

    private void Awake()
    {
        this.audioChannelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");
    }

    private void OnEnable()
    {
        AudioManager.instance.Play(this.openSound, this.audioChannelSettings);
    }

    public void SetupLevelSummary(Level setupLevel)
    {
        this.associatedLevel = setupLevel;

        this.levelName.text = setupLevel.levelName;

        string levelStats = PlayerPrefs.GetString(setupLevel.sceneName, "");

        string[] levelStatsArray = levelStats.Split(',');

        this.playerScore.text = (levelStats == "") ? "Unbeaten" : levelStatsArray[0];

        //Setup ball counts here
    }

    public void PlayButtonPressed()
    {
        AudioManager.instance.Play(this.confirmSound, this.audioChannelSettings);
        LevelList.SetLevelIndex(this.associatedLevel.levelIndex);
        SceneLoader.instance.LoadScene(this.associatedLevel.sceneName);
    }

    public void BackButtonPressed()
    {
        AudioManager.instance.Play(this.closeSound, this.audioChannelSettings);        
        this.gameObject.SetActive(false);
    }
}
