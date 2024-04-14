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

    private void Update()
    {
        this.scoreText.text = this.currentScore.ToString();
    }

    public void UpdateScore(SplittableObject scoredObject, int holeScore)
    {
        this.currentScore += holeScore * scoredObject.GetScoreMultiplier();
        this.scoreBalls.Add(new ScoreBall(scoredObject));
    }
}
