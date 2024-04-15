using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public int personalBestScore = 0;

    public int neutralBallCount = 0;
    public int goodBallCount = 0;
    public int badBallCount = 0;

    [SerializeField]
    private TextMeshProUGUI scoreText;

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
        this.personalBestScore = LevelList.GetCurrentLevel().personalBestScore;

        LevelTimer.instance.onTimerCompleted -= this.UpdatePersonalBestScore;
        LevelTimer.instance.onTimerCompleted += this.UpdatePersonalBestScore;
    }

    private void Update()
    {
        this.scoreText.text = this.currentScore.ToString();
    }

    public void UpdateScore(SplittableObject scoredObject, int holeScore)
    {
        this.currentScore += holeScore * scoredObject.GetScoreMultiplier();
        this.UpdateBallCounts(scoredObject.objectAlignment);
        this.scoreBalls.Add(new ScoreBall(scoredObject));

        if (this.currentScore > this.personalBestScore)
        {
            this.personalBestScore = this.currentScore;
        }
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
        if (this.currentScore == this.personalBestScore)
        { 
            //Upload a new personal best to database
        }
    }
}
