using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [SerializeField]
    private Image tutorialImage;

    [SerializeField]
    private float fadeDuration = 0.5f;
    private float alphaChangePerFrame;

    public bool hasSplit = false;
    public bool hasFocused = false;
    public bool isFading = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        int hasSeenTutorial = PlayerPrefs.GetInt("seenTutorial", 0);        

        this.alphaChangePerFrame = (1.0f / this.fadeDuration) * Time.fixedDeltaTime;    

        if (hasSeenTutorial == 0)
        {
            this.ShowTutorialAnimations();
        }
    }

    private void Update()
    {
        if (this.hasSplit == true && this.hasFocused == true && this.isFading == false)
        {
            this.isFading = true;
            StartCoroutine(this.FadeOut());
        }
    }

    private void ShowTutorialAnimations()
    {   
        StartCoroutine(this.FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (this.tutorialImage.color.a < 1)
        {
            float newAlpha = this.tutorialImage.color.a + this.alphaChangePerFrame;
            Color updatedColor = new Color(this.tutorialImage.color.r, this.tutorialImage.color.g, this.tutorialImage.color.b, newAlpha);
            this.tutorialImage.color = updatedColor;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FadeOut()
    {
        while (this.tutorialImage.color.a > 0)
        {
            float newAlpha = this.tutorialImage.color.a - this.alphaChangePerFrame;
            Color updatedColor = new Color(this.tutorialImage.color.r, this.tutorialImage.color.g, this.tutorialImage.color.b, newAlpha);
            this.tutorialImage.color = updatedColor;

            yield return new WaitForFixedUpdate();
        }

        this.tutorialImage.enabled = false;

        PlayerPrefs.SetInt("seenTutorial", 1);
    }
}
