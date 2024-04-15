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

    public bool timerStarted = false;

    [SerializeField]
    private float startingFillAmount;

    private float currentFillAmount;

    public delegate void TimerCompleted();
    public TimerCompleted onTimerCompleted;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        this.timerStarted = false;
        this.currentTime = this.startingTime;
    }

    public void StartTimer()
    {
        this.timerStarted = true;
        StartCoroutine(this.TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        this.currentTime = this.startingTime;

        float percentFinished = 0.0f;

        while (this.currentTime >= 0.0f)
        {
            this.currentTime -= Time.deltaTime;
            percentFinished = (this.startingTime - this.currentTime) / this.startingTime;

            this.currentFillAmount = (this.startingFillAmount * (1.0f - percentFinished));

            yield return null;
        }

        this.TimerEnd();
    }

    private void TimerEnd()
    {
        if (this.onTimerCompleted != null)
        {
            this.onTimerCompleted();
        }
    }

    public float GetImageFloatAmount()
    {
        return this.currentFillAmount;
    }

    public float GetCurrentTime()
    {
        return this.currentTime;
    }
}
