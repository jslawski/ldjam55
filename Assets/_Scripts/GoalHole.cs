using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHole : MonoBehaviour
{
    [SerializeField]
    private AudioClip victorySound;
    [SerializeField]
    private int scoreValue = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Splittable")
        {
            SplittableObject splittableComponent = other.gameObject.GetComponent<SplittableObject>();

            MergeManager.instance.RemoveUnmergedObject(splittableComponent);

            AudioChannelSettings channelSettings = new AudioChannelSettings();

            AudioManager.instance.Play(this.victorySound, channelSettings);

            ScoreKeeper.instance.UpdateScore(splittableComponent, this.scoreValue);
        }
    }
}
