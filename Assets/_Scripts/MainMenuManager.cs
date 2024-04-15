using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void PlayButtonClicked()
    {
        SceneLoader.instance.LoadScene("LevelSelect");
    }

    public void CloseButtonClicked()
    {
        SceneLoader.instance.CloseGame();
    }
}
