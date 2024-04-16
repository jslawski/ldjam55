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
    private ParticleSystem particles;

    [SerializeField]
    private AudioClip badSound;
    [SerializeField]
    private AudioClip goodSound;
    [SerializeField]
    private AudioClip neutralSound;

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

            AudioChannelSettings badChannelSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 0.5f, "SFX");
            AudioChannelSettings goodChannelSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 0.75f, "SFX");
            AudioChannelSettings neutralChannelSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 0.75f, "SFX");

            if (splittableComponent.objectAlignment == Alignment.Bad)
            {
                AudioManager.instance.Play(this.badSound, badChannelSettings);
            }
            else if (splittableComponent.objectAlignment == Alignment.Good)
            {
                AudioManager.instance.Play(this.goodSound, goodChannelSettings);
            }
            else
            {
                AudioManager.instance.Play(this.neutralSound, goodChannelSettings);
            }
            //AudioManager.instance.Play(this.victorySound, goodChannelSettings);

            ScoreKeeper.instance.UpdateScore(splittableComponent, this.scoreValue);

            this.particles.Stop();
            this.particles.Play();
        }
    }
}
