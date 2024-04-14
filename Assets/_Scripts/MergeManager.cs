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
    private GameObject splittableObjectPrefab;

    
    [SerializeField]
    private List<SplittableObject> unmergedObjects;
    private List<MergePair> mergingPairs;

    int spawnIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.unmergedObjects = new List<SplittableObject>();
        this.mergingPairs = new List<MergePair>();
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
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
        }
    
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
            Vector3 compositeVelocity = (object1.rigidBody.velocity + object2.rigidBody.velocity);

            this.RemoveUnmergedObject(object1);
            this.RemoveUnmergedObject(object2);

            this.SpawnMergedSplittableObject(spawnPoint, compositeScale, compositeVelocity);
        }

        this.mergingPairs.Clear();
    }
    
    private void SpawnMergedSplittableObject(Vector3 spawnPoint, Vector3 newScale, Vector3 launchVelocity)
    {
        GameObject newObject = Instantiate(this.splittableObjectPrefab, spawnPoint, new Quaternion());

        newObject.transform.localScale = newScale;

        SplittableObject splittableComponent = newObject.GetComponent<SplittableObject>();
        splittableComponent.Launch(launchVelocity.normalized * 2.0f);

        this.AddUnmergedObject(splittableComponent);
    }
}
