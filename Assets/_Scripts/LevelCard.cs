using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCard : MonoBehaviour
{
    private Level associatedLevel;

    [SerializeField]
    private TextMeshProUGUI levelIndexText;
    [SerializeField]
    private TextMeshProUGUI levelName;
    [SerializeField]
    private Image levelImage;
    [SerializeField]
    private TextMeshProUGUI playerScore;
    [SerializeField]
    private TextMeshProUGUI playerRank;
    [SerializeField]
    private AudioClip selectLevelSound;

    public void SetupLevelCard(Level setupLevel)
    {
        this.associatedLevel = setupLevel;
        this.levelName.text = setupLevel.levelName;
        this.levelImage.sprite = Resources.Load<Sprite>("LevelImages/" + setupLevel.imageFileName);

        string levelStats = PlayerPrefs.GetString(setupLevel.sceneName, "");
        string[] levelStatsArray = levelStats.Split(',');

        this.playerScore.text = (levelStats == "") ? "Unbeaten" : levelStatsArray[1];
    }

    public void SelectLevel()
    {
        AudioChannelSettings channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");
        AudioManager.instance.Play(this.selectLevelSound, channelSettings);

        LevelSelectManager.instance.SelectLevel(this.associatedLevel);
    }
}
