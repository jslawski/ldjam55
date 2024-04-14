using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusModeManager : MonoBehaviour
{
    private float focusTimeScale = 0.1f;
    private float maxFixedTime;

    [SerializeField]
    private AnimationCurve slowDownCurve;

    private float enterDuration = 0.5f;
    private float incrementPerFrame;

    // Start is called before the first frame update
    void Start()
    {
        this.maxFixedTime = Time.fixedDeltaTime;

        this.incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            this.EnterFocusMode();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            this.StopAllCoroutines();
            this.ExitFocusMode();
        }
    }

    private void EnterFocusMode()
    {
        StartCoroutine(this.FocusModeCoroutine());
    }
    
    private IEnumerator FocusModeCoroutine()
    {
        float tValue = 0.0f;
        
        while (tValue < 0.98f)
        {
            float curveValue = this.slowDownCurve.Evaluate(tValue);
            Time.timeScale = Mathf.Lerp(1.0f, this.focusTimeScale, curveValue);
            Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);

            this.incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;
            tValue += incrementPerFrame;
            
            yield return null;
        }

        Time.timeScale = this.focusTimeScale;
        Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);
    }
    
    private void ExitFocusMode()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = this.maxFixedTime;
    }
}
