using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHole : MonoBehaviour
{
    [SerializeField]
    private AudioClip victorySound;

    private Collider goalCollider;
    
    [SerializeField]
    private int scoreValue = 100;

    [SerializeField]
    private GameObject particlePrefab;

    private void Awake()
    {
        this.goalCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        LevelTimer.instance.onTimerCompleted -= DisableGoal;
        LevelTimer.instance.onTimerCompleted += DisableGoal;
    }

    private void DisableGoal()
    {
        this.goalCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Splittable")
        {
            SplittableObject splittableComponent = other.gameObject.GetComponent<SplittableObject>();

            MergeManager.instance.RemoveUnmergedObject(splittableComponent);

            AudioChannelSettings channelSettings = new AudioChannelSettings();

            AudioManager.instance.Play(this.victorySound, channelSettings);

            ScoreKeeper.instance.UpdateScore(splittableComponent, this.scoreValue);

            Instantiate(this.particlePrefab, this.transform.position, new Quaternion());
        }
    }
}
