using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public int levelIndex;
    public string sceneName;
    public string levelName;
    public int personalBestScore;
    public string imageFileName;

    public string nextLevelName;
}
