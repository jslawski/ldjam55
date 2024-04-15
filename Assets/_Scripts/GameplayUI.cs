using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameplayUI : MonoBehaviour
{
    [SerializeField]
    private Image timerImage;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private TextMeshProUGUI goalText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI personalBestText;
    
    private void Awake()
    {
        this.timerText.text = LevelList.GetCurrentLevel().timeLimit.ToString();
        this.goalText.text = LevelList.GetCurrentLevel().scoreGoal.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        this.timerImage.fillAmount = LevelTimer.instance.GetImageFloatAmount();
        this.timerText.text = this.GetFormattedTimerText(LevelTimer.instance.GetCurrentTime());

        this.scoreText.text = ScoreKeeper.instance.currentScore.ToString();
        this.scoreText.text = ScoreKeeper.instance.personalBestScore.ToString();
    }

    private string GetFormattedTimerText(float currentTime)
    {
        string formattedTimer = string.Empty;

        if (currentTime <= 0.0f)
        {
            return "00.00";
        }

        if (currentTime < 10.0f)
        {
            formattedTimer += "0";
        }

        formattedTimer += Mathf.FloorToInt(currentTime).ToString();

        if (currentTime % 1 == 0)
        {
            formattedTimer += ".00";
            return formattedTimer;
        }

        decimal decimalValues = Math.Round((decimal)(currentTime) % 1, 2);

        formattedTimer += decimalValues.ToString().Substring(1);

        if (formattedTimer.Length < 5)
        {
            formattedTimer += "0";
        }

        return formattedTimer;
    }
}