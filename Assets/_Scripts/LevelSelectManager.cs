using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager instance;

    [SerializeField]
    private GameObject levelCardPrefab;

    [SerializeField]
    private RectTransform levelParent;

    [SerializeField]
    private GameObject levelSummaryObject;

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

    private void LoadLevelsIntoScene()
    {
        for (int i = 0; i < LevelList.allLevels.Length; i++)
        {
            this.CreateLevelCard(i);
        }
    }

    private void CreateLevelCard(int levelIndex)
    {
        GameObject newLevelCard = GameObject.Instantiate(this.levelCardPrefab, levelParent);
        LevelCard levelCardComponent = newLevelCard.GetComponent<LevelCard>();

        levelCardComponent.SetupLevelCard(LevelList.allLevels[levelIndex]);
    }

    public void SelectLevel(Level selectedLevel)
    {
        LevelSummary levelSummaryComponent = this.levelSummaryObject.GetComponent<LevelSummary>();
        levelSummaryComponent.SetupLevelSummary(selectedLevel);
        this.levelSummaryObject.SetActive(true);
    }
}
