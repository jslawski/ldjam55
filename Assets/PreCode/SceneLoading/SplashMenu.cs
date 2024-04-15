using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashMenu : MonoBehaviour
{
    [SerializeField]
    private FadePanelManager fadePanel;

    [SerializeField]
    private string nextSceneName;

    private bool loading = false;
    
    private void Awake()
    {
        this.fadePanel.OnFadeSequenceComplete += this.DisplaySplashScreen;
        this.fadePanel.FadeFromBlack();
    }

    private void DisplaySplashScreen()
    {
        this.fadePanel.OnFadeSequenceComplete -= this.DisplaySplashScreen;
        StartCoroutine(this.DisplayCoroutine());
    }

    private IEnumerator DisplayCoroutine()
    {
        yield return new WaitForSeconds(3.0f);

        string playerName = PlayerPrefs.GetString("name", "");

        if (playerName == "")
        {
            SceneLoader.instance.LoadScene("LoginScene");
        }
        else
        {
            SceneLoader.instance.LoadScene("LevelSelect");
        }

        //SceneLoader.instance.LoadScene(this.nextSceneName);
    }
}
