using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public int levelIndex;
    public string sceneName;
    public string levelName;
    public string levelDifficulty;
    public string imageFileName;
    public float timeLimit;

    public string nextLevelName;
}
