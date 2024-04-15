using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class ScoreBall
{
    public Alignment ballAlignment;
    public float scale;

    public ScoreBall(SplittableObject scoredObject)
    {
        this.ballAlignment = scoredObject.objectAlignment;
        this.scale = scoredObject.gameObject.transform.localScale.x;
    }

    public ScoreBall(ScoreBall newBall)
    {
        this.ballAlignment = newBall.ballAlignment;
        this.scale = newBall.scale;
    }
}

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper instance;

    private List<ScoreBall> scoreBalls;
    
    public int currentScore = 0;
    private int personalBestScore = 0;

    public int neutralBallCount = 0;
    public int goodBallCount = 0;
    public int badBallCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    
        this.scoreBalls = new List<ScoreBall>();
    }

    private void Start()
    {
        string levelStats = PlayerPrefs.GetString(LevelList.GetCurrentLevel().sceneName, "");
        string[] levelStatsArray = levelStats.Split(',');

        this.personalBestScore = (levelStats == "") ? 0 : Int32.Parse(levelStatsArray[0]);

        LevelTimer.instance.onTimerCompleted -= this.UpdatePersonalBestScore;
        LevelTimer.instance.onTimerCompleted += this.UpdatePersonalBestScore;
    }

    public void UpdateScore(SplittableObject scoredObject, int holeScore)
    {
        this.currentScore += holeScore * scoredObject.GetScoreMultiplier();
        this.UpdateBallCounts(scoredObject.objectAlignment);
        this.scoreBalls.Add(new ScoreBall(scoredObject));
    }

    private void UpdateBallCounts(Alignment alignment)
    {
        switch (alignment)
        {
            case Alignment.Neutral:
                this.neutralBallCount++;
                break;
            case Alignment.Good:
                this.goodBallCount++;
                break;
            case Alignment.Bad:
                this.badBallCount++;
                break;
            default:
                break;
        }
    }

    private void UpdatePersonalBestScore()
    {
        if (this.currentScore >= this.personalBestScore)
        {
            string prefsString = ScoreKeeper.instance.personalBestScore.ToString() + "," +
                            ScoreKeeper.instance.neutralBallCount + "," +
                            ScoreKeeper.instance.goodBallCount + "," +
                            ScoreKeeper.instance.badBallCount;

            PlayerPrefs.SetString(SceneManager.GetActiveScene().name, prefsString);

            this.personalBestScore = this.currentScore;
        }
    }

    public int GetPersonalBestScore()
    {
        this.UpdatePersonalBestScore();
        return this.personalBestScore;
    }
}
