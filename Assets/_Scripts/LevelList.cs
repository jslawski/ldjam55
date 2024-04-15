using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelList
{
    public static int currentLevelIndex = 0;
    public static Level[] allLevels;

    public static void SetupList(Level[] levelResources)
    {
        LevelList.allLevels = levelResources;
    }
    
    public static Level GetLevel(int index)
    {
        if (index >= LevelList.allLevels.Length)
        {
            return null;
        }
        
        return LevelList.allLevels[index];
    }

    public static Level GetCurrentLevel()
    {
        return LevelList.allLevels[LevelList.currentLevelIndex];
    }

    public static void SetLevelIndex(int index)
    {
        LevelList.currentLevelIndex = index;
    }    
}
