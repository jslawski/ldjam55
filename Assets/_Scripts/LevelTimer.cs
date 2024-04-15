using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer instance;

    [SerializeField]
    private float startingTime = 10.0f;
    private float currentTime;
    private TextMeshProUGUI timerText;
    private Image timerImage;

    public bool timerStarted = false;

    [SerializeField]
    private float startingFillAmount;

    private void Awake()
    {
        this.timerText = GetComponentInChildren<TextMeshProUGUI>();
        this.timerImage = GetComponentInChildren<Image>();

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        this.timerStarted = false;
        this.currentTime = this.startingTime;    
        this.timerText.text = this.GetFormattedTimerText();
        this.timerImage.fillAmount = this.startingFillAmount;
    }

    public void StartTimer()
    {
        this.timerStarted = true;
        StartCoroutine(this.TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        this.currentTime = this.startingTime;
        float currentFillAmount = 0.0f;

        float percentFinished = 0.0f;

        while (this.currentTime >= 0.0f)
        {
            this.currentTime -= Time.deltaTime;
            percentFinished = (this.startingTime - this.currentTime) / this.startingTime;

            this.timerImage.fillAmount = (this.startingFillAmount * (1.0f - percentFinished));
            this.timerText.text = this.GetFormattedTimerText();

            yield return null;
        }

        this.TimerEnd();
    }

    private void TimerEnd()
    {
        Debug.LogError("TIMES UP!");
    }

    private string GetFormattedTimerText()
    {
        string formattedTimer = string.Empty;

        if (this.currentTime <= 0.0f)
        {
            return "00.00";
        }

        if (this.currentTime < 10.0f)
        {
            formattedTimer += "0";
        }

        formattedTimer += Mathf.FloorToInt(this.currentTime).ToString();

        if (this.currentTime % 1 == 0)
        {
            formattedTimer += ".00";
            return formattedTimer;
        }

        decimal decimalValues = Math.Round((decimal)(this.currentTime) % 1, 2);
        
        formattedTimer += decimalValues.ToString().Substring(1);

        if (formattedTimer.Length < 5)
        {
            formattedTimer += "0";
        }

        return formattedTimer;
    }
}
