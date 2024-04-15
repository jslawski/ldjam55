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
    private AudioClip selectLevelSound;

    public void SetupLevelCard(Level setupLevel)
    {
        this.associatedLevel = setupLevel;
        this.levelName.text = setupLevel.levelName;

        if (setupLevel.levelIndex < 10)
        {
            this.levelIndexText.text = "0" + setupLevel.levelIndex.ToString() + setupLevel.levelDifficulty;
        }
        else
        {
            this.levelIndexText.text = setupLevel.levelIndex.ToString() + setupLevel.levelDifficulty;
        }
        
        this.levelImage.sprite = Resources.Load<Sprite>("LevelImages/" + setupLevel.imageFileName);

        
        string levelStats = PlayerPrefs.GetString(setupLevel.sceneName, "");
        string[] levelStatsArray = levelStats.Split(',');

        this.playerScore.text = (levelStats == "") ? "Unbeaten" : levelStatsArray[0];
    }

    public void SelectLevel()
    {
        AudioChannelSettings channelSettings = new AudioChannelSettings(false, 1.0f, 1.0f, 1.0f, "SFX");

        if (this.selectLevelSound != null)
        {
            AudioManager.instance.Play(this.selectLevelSound, channelSettings);
        }
        
        LevelSelectManager.instance.SelectLevel(this.associatedLevel);
    }
}
