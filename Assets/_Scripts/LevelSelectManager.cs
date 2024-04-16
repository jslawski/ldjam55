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

    private AudioClip[] allMusic;
    private AudioChannelSettings musicSettings;


    private void Awake()
    {            
        if (instance == null)
        {
            instance = this;
        }

        this.SetupLevelList();

        this.LoadLevelsIntoScene();

        this.allMusic = Resources.LoadAll<AudioClip>("Soundtracks");
        this.musicSettings = new AudioChannelSettings(true, 1.0f, 1.0f, 0.5f, "BGM");
    }

    private void Start()
    {
        this.PlayRandomSong();
    }

    private void PlayRandomSong()
    {
        int randomIndex = Random.Range(0, this.allMusic.Length);
        AudioManager.instance.Play(this.allMusic[randomIndex], this.musicSettings);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            SceneLoader.instance.LoadScene("MainMenu");
        }
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
