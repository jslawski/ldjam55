using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class LeaderboardEntryObject : MonoBehaviour
{
    public TextMeshProUGUI username;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private void Start()
    {
        if (this.scoreText.text == "0")
        {
            this.gameObject.SetActive(false);
        }
    }

    public void UpdateEntry(string username, float score)
    {
        this.gameObject.SetActive(true);
        
        if (username == "" || score == 0)
        {            
            return;
        }
        
        this.username.text = username;
        this.scoreText.text = Mathf.RoundToInt(score).ToString();
    }    
}
