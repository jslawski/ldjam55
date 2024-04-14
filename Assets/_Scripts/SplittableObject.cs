using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittableObject : MonoBehaviour
{
    
    public bool splittable = false;
    
    public bool mergeable = false;

    public bool isMerging = false;

    [SerializeField]
    private GameObject splittableObjectPrefab;

    [SerializeField]
    private float splitScalePercent = 0.75f;

    [SerializeField]
    private LayerMask mergeableLayer;

    [SerializeField]
    private LayerMask obstacleLayer;

    public Rigidbody rigidBody;

    private int inertFrames = 5;

    private int unmergeableFrames = 10;

    private float splitSpeed = 10.0f;

    private void Awake()
    {
        this.splittable = false;
        this.mergeable = false;

        StartCoroutine(this.ActivateSplittableFlag());
        StartCoroutine(this.ActivateMergeableFlag());
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, (this.gameObject.transform.localScale.x / 2.0f), this.mergeableLayer, QueryTriggerInteraction.Collide);
    
        if (this.mergeable == false && colliders.Length <= 1)
        {
            this.mergeable = true;
        }
    }

    public SplittableObject GetCollidingObject()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, (this.gameObject.transform.localScale.x / 2.0f), this.mergeableLayer, QueryTriggerInteraction.Collide);

        if (this.mergeable == false || colliders.Length <= 1)
        {
            return null;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.transform.parent != this.gameObject.transform)
            {
                SplittableObject splitObject = colliders[i].gameObject.GetComponentInParent<SplittableObject>();
                if (splitObject != null)
                {
                    return splitObject;
                }                
            }
        }

        return null;
    }

    private IEnumerator ActivateSplittableFlag()
    {
        for (int i = 0; i < this.inertFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        this.splittable = true;
    }

    private IEnumerator ActivateMergeableFlag()
    {
        for (int i = 0; i < this.unmergeableFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        this.mergeable = true;
    }

    private RaycastHit GetObstacleHitInfo(Vector3 checkPosition)
    {
        Vector3 raycastDirection = (checkPosition - this.gameObject.transform.position).normalized;
        float raycastDistance = (checkPosition - this.gameObject.transform.position).magnitude + ((this.gameObject.transform.localScale.x * this.splitScalePercent) / 2.0f);

        RaycastHit hitInfo;
        Physics.Raycast(this.gameObject.transform.position, raycastDirection, out hitInfo, raycastDistance, this.obstacleLayer);
        
        return hitInfo;
    }

    private Vector3 GetAdjustmentVector(RaycastHit hitInfo)
    {
        Vector3 compositeVector = hitInfo.normal.normalized;
        compositeVector += (hitInfo.point - this.gameObject.transform.position).normalized;
        return compositeVector.normalized;
    }

    private void SpawnNewSplittableObjects(Vector3 splitDirection, float splitVelocityPercentage)
    {
        float splitBallRadius = ((this.gameObject.transform.localScale.x * this.splitScalePercent) / 2.0f);
        float originalBallRadius = (this.gameObject.transform.localScale.x / 2.0f);

        Vector3 launchDirection1 = Vector2.Perpendicular(splitDirection).normalized;
        Vector3 launchDirection2 = -launchDirection1;

        Vector3 spawnPosition1 = this.gameObject.transform.position + (launchDirection1 * (originalBallRadius + (0.00f * originalBallRadius)));
        Vector3 spawnPosition2 = this.gameObject.transform.position + (launchDirection2 * (originalBallRadius + (0.00f * originalBallRadius)));

        Vector3 adjustment1 = Vector3.zero;
        Vector3 adjustment2 = Vector3.zero;

        RaycastHit obstacle1 = this.GetObstacleHitInfo(spawnPosition1);
        RaycastHit obstacle2 = this.GetObstacleHitInfo(spawnPosition2);

        if (obstacle1.transform != null)
        {
            adjustment1 = (this.gameObject.transform.position - obstacle1.point).normalized;
            spawnPosition1 = obstacle1.point + (adjustment1 * splitBallRadius);
            launchDirection1 = adjustment1;
        }

        if (obstacle2.transform != null)
        {
            adjustment2 = (this.gameObject.transform.position - obstacle2.point).normalized;
            spawnPosition2 = obstacle2.point + (adjustment2 * splitBallRadius);
            launchDirection2 = adjustment2;
        }

        GameObject firstObject = Instantiate(this.splittableObjectPrefab, spawnPosition1, new Quaternion());
        GameObject secondObject = Instantiate(this.splittableObjectPrefab, spawnPosition2, new Quaternion());

        firstObject.transform.localScale = this.gameObject.transform.localScale * this.splitScalePercent;
        secondObject.transform.localScale = this.gameObject.transform.localScale * this.splitScalePercent;
        
        float scaledSplitSpeed = splitVelocityPercentage * this.splitSpeed;

        firstObject.GetComponent<SplittableObject>().Launch(launchDirection1 * scaledSplitSpeed);
        secondObject.GetComponent<SplittableObject>().Launch(launchDirection2 * scaledSplitSpeed);

        MergeManager.instance.AddUnmergedObject(firstObject.GetComponent<SplittableObject>());
        MergeManager.instance.AddUnmergedObject(secondObject.GetComponent<SplittableObject>());
    }

    public void Launch(Vector3 launchDirection)
    {
        this.rigidBody.AddForce(launchDirection, ForceMode.VelocityChange);
    }

    public void Split(Vector3 splitDirection, float splitVelocityPercentage)
    {
        this.SpawnNewSplittableObjects(splitDirection, splitVelocityPercentage);
        MergeManager.instance.RemoveUnmergedObject(this);
    }
}
