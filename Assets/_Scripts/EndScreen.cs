using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI neutralBallCountText;
    [SerializeField]
    private TextMeshProUGUI goodBallCountText;
    [SerializeField]
    private TextMeshProUGUI badBallCountText;

    [SerializeField]
    private TextMeshProUGUI rankText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI personalBestText;

    [SerializeField]
    private TextMeshProUGUI currentLevelNumberText;
    [SerializeField]
    private TextMeshProUGUI currentLevelNameText;

    [SerializeField]
    private TextMeshProUGUI nextLevelNumberText;
    [SerializeField]
    private TextMeshProUGUI nextLevelNameText;

    // Start is called before the first frame update
    void Start()
    {
        this.rankText.text = PlayerPrefs.GetString("name", "");

        this.SetBallCounts();
        this.SetScoreElements();
        this.SetLevelInfo();

        LeaderboardManager.instance.RefreshLeaderboard(LevelList.GetCurrentLevel().sceneName);
    }

    private void SetBallCounts()
    {
        this.neutralBallCountText.text = ScoreKeeper.instance.neutralBallCount.ToString();
        this.goodBallCountText.text = ScoreKeeper.instance.goodBallCount.ToString();
        this.badBallCountText.text = ScoreKeeper.instance.badBallCount.ToString();
    }

    private void SetScoreElements()
    {
        this.scoreText.text = ScoreKeeper.instance.currentScore.ToString();
        this.personalBestText.text = ScoreKeeper.instance.GetPersonalBestScore().ToString();
    }

    private void SetLevelInfo()
    {
        Level currentLevel = LevelList.GetCurrentLevel();
        this.currentLevelNumberText.text = currentLevel.levelIndex.ToString();
        this.currentLevelNameText.text = currentLevel.levelName;

        Level nextLevel = LevelList.GetLevel(currentLevel.levelIndex + 1);

        if (nextLevel != null)
        {
            this.nextLevelNumberText.text = nextLevel.levelIndex.ToString();
            this.nextLevelNameText.text = nextLevel.levelName;
        }
    }

    public void NextLevelButtonClicked()
    {
        Level nextLevel = LevelList.GetLevel((LevelList.GetCurrentLevel().levelIndex + 1));

        if (nextLevel != null)
        {
            SceneLoader.instance.LoadScene(nextLevel.sceneName);
        }
        else
        {
            SceneLoader.instance.LoadScene("LevelSelect");
        }
    }
}
