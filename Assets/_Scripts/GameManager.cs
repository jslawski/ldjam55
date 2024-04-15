using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private float secondsBeforeEndScreen = 2.0f;

    //Have an EndScreen reference here

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }

    private void Start()
    {
        LevelTimer.instance.onTimerCompleted -= this.EndLevel;
        LevelTimer.instance.onTimerCompleted += this.EndLevel;
    }

    private void EndLevel()
    {
        StartCoroutine(this.DisplayEndScreenAfterDelay());
    }

    private IEnumerator DisplayEndScreenAfterDelay()
    {
        yield return new WaitForSecondsRealtime(this.secondsBeforeEndScreen);
        this.DisplayEndScreen();
    }

    private void DisplayEndScreen()
    { 
    
    }
}
