using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager instance;
    
    private void Awake()
    {            
        if (instance == null)
        {
            instance = this;
        }

        this.SetupLevelList();
    }

    private void SetupLevelList()
    {
        LevelList.SetupList(Resources.LoadAll<Level>("Levels"));

        //Request all personalBest scores here, then update the LevelList with them
    }
    
}
