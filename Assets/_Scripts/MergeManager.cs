using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MergePair
{
    public SplittableObject object1;
    public SplittableObject object2;

    public MergePair(SplittableObject object1, SplittableObject object2)
    {
        this.object1 = object1;
        this.object2 = object2;
    }

    public MergePair(MergePair newPair)
    {
        this.object1 = newPair.object1;
        this.object2 = newPair.object2;
    }
}

public class MergeManager : MonoBehaviour
{
    public static MergeManager instance;

    [SerializeField]
    private GameObject neutralObjectPrefab;
    [SerializeField]
    private GameObject goodObjectPrefab;
    [SerializeField]
    private GameObject badObjectPrefab;

    [SerializeField]
    private List<SplittableObject> unmergedObjects;
    private List<MergePair> mergingPairs;

    [SerializeField]
    private GameObject mergeParticles;

    [SerializeField]
    private AudioClip mergeSound;
    private AudioChannelSettings mergeAudioSettings;

    int spawnIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.unmergedObjects = new List<SplittableObject>();
        this.mergingPairs = new List<MergePair>();

        this.mergeAudioSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 1.3f, "SFX");
    }

    public void AddUnmergedObject(SplittableObject newObject)
    {
        this.unmergedObjects.Add(newObject);
        newObject.gameObject.name = "Ball" + this.spawnIndex;
        this.spawnIndex++;
    }

    public void RemoveUnmergedObject(SplittableObject oldObject)
    {
        this.unmergedObjects.Remove(oldObject);
        Destroy(oldObject.gameObject);
    }

    private void Update()
    {
        this.AccumulateMergePairs();
        this.MergeAllPairs();
    }

    private void AccumulateMergePairs()
    {
        for (int i = 0; i < this.unmergedObjects.Count; i++)
        {
            if (this.unmergedObjects[i].mergeable == true && this.unmergedObjects[i].isMerging == false)
            {
                SplittableObject collidingObject = this.unmergedObjects[i].GetCollidingObject();

                if (collidingObject != null)
                {
                    this.unmergedObjects[i].isMerging = true;
                    collidingObject.isMerging = true;
                    this.mergingPairs.Add(new MergePair(this.unmergedObjects[i], collidingObject));
                }
            }
        }
    }

    private void MergeAllPairs()
    { 
        for (int i = 0; i < this.mergingPairs.Count; i++)
        {
            SplittableObject object1 = this.mergingPairs[i].object1;
            SplittableObject object2 = this.mergingPairs[i].object2;

            Vector3 spawnPoint = Vector3.Lerp(object1.gameObject.transform.position, object2.gameObject.transform.position, 0.5f);
            Vector3 compositeScale = (object1.gameObject.transform.localScale * (2.0f / 3.0f) + object2.gameObject.transform.localScale * (2.0f / 3.0f));
            Vector3 compositeVelocity = ((object1.rigidBody.velocity * object1.rigidBody.mass) + (object2.rigidBody.velocity * object2.rigidBody.mass));
            float compositeMass = (object1.rigidBody.mass * (2.0f / 3.0f) + object2.rigidBody.mass * (2.0f / 3.0f));
            Alignment compositeAlignment = this.GetCompositeAlignment(object1.objectAlignment, object2.objectAlignment);

            this.RemoveUnmergedObject(object1);
            this.RemoveUnmergedObject(object2);

            this.SpawnMergedSplittableObject(spawnPoint, compositeScale, compositeVelocity, compositeMass, compositeAlignment);
        }

        this.mergingPairs.Clear();
    }
    
    private void SpawnMergedSplittableObject(Vector3 spawnPoint, Vector3 newScale, Vector3 launchVelocity, float newMass, Alignment newAlignment)
    {
        GameObject newObject;

        switch (newAlignment)
        {
            case Alignment.Good:
                newObject = Instantiate(this.goodObjectPrefab, spawnPoint, new Quaternion());
                break;
            case Alignment.Bad:
                newObject = Instantiate(this.badObjectPrefab, spawnPoint, new Quaternion());
                break;
            default:
                newObject = Instantiate(this.neutralObjectPrefab, spawnPoint, new Quaternion());
                break;
        }
        
        newObject.transform.localScale = newScale;

        SplittableObject splittableComponent = newObject.GetComponent<SplittableObject>();
        splittableComponent.rigidBody.mass = newMass;
        splittableComponent.Launch(launchVelocity);

        this.AddUnmergedObject(splittableComponent);

        GameObject particleInstance = Instantiate(this.mergeParticles, spawnPoint, new Quaternion());
        particleInstance.transform.localScale = newScale;

        AudioManager.instance.Play(this.mergeSound, this.mergeAudioSettings);
    }

    private Alignment GetCompositeAlignment(Alignment alignment1, Alignment alignment2)
    {
        if (alignment1 == Alignment.Good && alignment2 == Alignment.Good)
        {
            return Alignment.Good;
        }
        else if (alignment1 == Alignment.Bad && alignment2 == Alignment.Bad)
        {
            return Alignment.Bad;
        }

        return Alignment.Neutral;
    }
}
