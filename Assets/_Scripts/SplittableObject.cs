using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Alignment { Good, Bad, Neutral};

public class SplittableObject : MonoBehaviour
{
    public const float GOOD_MULTIPLIER = 1.5f;
    public const float BAD_MULTIPLIER = -1.0f;
    public const float NEUTRAL_MULTIPLIER = 1.0f;

    public bool splittable = false;
    
    public bool mergeable = false;

    public bool isMerging = false;

    public Alignment objectAlignment = Alignment.Neutral;

    [SerializeField]
    private GameObject badObjectPrefab;
    [SerializeField]
    private GameObject goodObjectPrefab;

    [SerializeField]
    private float splitScalePercent = 0.75f;

    [SerializeField]
    private LayerMask mergeableLayer;

    [SerializeField]
    private LayerMask obstacleLayer;

    [SerializeField]
    private LineRenderer predictionLine1;
    [SerializeField]
    private LineRenderer predictionLine2;
    [SerializeField]
    private int predictionDistance;
    [HideInInspector]
    public bool framePredicted;

    [SerializeField]
    private ParticleSystem bounceBurstParticle;

    public Rigidbody rigidBody;

    private int inertFrames = 5;

    private int unmergeableFrames = 10;

    private float splitSpeed = 20.0f;

    public Vector3 previousVelocity;

    private Vector3 spawnPosition1;
    private Vector3 spawnPosition2;

    private Vector3 launchVelocity1;
    private Vector3 launchVelocity2;

    private float sizeMultiplier = 1.0f;
    private float alignmentMultiplier = 1.0f;

    private int baseMultiplier = 10;

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

    private void LateUpdate()
    {
        if (this.framePredicted == true)
        {
            this.framePredicted = false;
        }
        else
        {
            this.predictionLine1.positionCount = 0;
            this.predictionLine2.positionCount = 0;
        }
    }

    private void FixedUpdate()
    {
        this.previousVelocity = this.rigidBody.velocity;
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
        this.CalculateSpawnAndLaunchVectors(splitDirection, splitVelocityPercentage);

        GameObject firstObject = Instantiate(this.goodObjectPrefab, spawnPosition1, new Quaternion());
        GameObject secondObject = Instantiate(this.badObjectPrefab, spawnPosition2, new Quaternion());

        firstObject.transform.localScale = this.gameObject.transform.localScale * this.splitScalePercent;
        secondObject.transform.localScale = this.gameObject.transform.localScale * this.splitScalePercent;

        SplittableObject splittable1 = firstObject.GetComponent<SplittableObject>();
        SplittableObject splittable2 = secondObject.GetComponent<SplittableObject>();

        splittable1.objectAlignment = Alignment.Good;
        splittable2.objectAlignment = Alignment.Bad;

        splittable1.rigidBody.mass = this.rigidBody.mass * this.splitScalePercent;
        splittable2.rigidBody.mass = this.rigidBody.mass * this.splitScalePercent;        

        splittable1.previousVelocity = this.launchVelocity1;
        splittable2.previousVelocity = this.launchVelocity2;

        splittable1.Launch(this.launchVelocity1 * splittable1.rigidBody.mass);
        splittable2.Launch(this.launchVelocity2 * splittable2.rigidBody.mass);

        MergeManager.instance.AddUnmergedObject(splittable1);
        MergeManager.instance.AddUnmergedObject(splittable2);
    }

    public void Launch(Vector3 launchDirection)
    {
        this.rigidBody.AddForce(launchDirection, ForceMode.Impulse);
    }

    public void Split(Vector3 splitDirection, float splitVelocityPercentage)
    {
        this.predictionLine1.positionCount = 0;
        this.predictionLine2.positionCount = 0;
    
        this.SpawnNewSplittableObjects(splitDirection, splitVelocityPercentage);
        MergeManager.instance.RemoveUnmergedObject(this);

        if (LevelTimer.instance.timerStarted == false)
        {
            LevelTimer.instance.StartTimer();
        }
    }

    private void CalculateSpawnAndLaunchVectors(Vector3 splitDirection, float splitVelocityPercentage)
    {
        float splitBallRadius = ((this.gameObject.transform.localScale.x * this.splitScalePercent) / 2.0f);
        float originalBallRadius = (this.gameObject.transform.localScale.x / 2.0f);

        float scaledSplitSpeed = splitVelocityPercentage * this.splitSpeed;

        this.launchVelocity1 = Vector2.Perpendicular(splitDirection).normalized * scaledSplitSpeed;
        this.launchVelocity2 = -launchVelocity1;

        this.spawnPosition1 = this.gameObject.transform.position + (this.launchVelocity1.normalized * (originalBallRadius + (0.25f * originalBallRadius)));
        this.spawnPosition2 = this.gameObject.transform.position + (this.launchVelocity2.normalized * (originalBallRadius + (0.25f * originalBallRadius)));

        Vector3 adjustment1 = Vector3.zero;
        Vector3 adjustment2 = Vector3.zero;

        RaycastHit obstacle1 = this.GetObstacleHitInfo(spawnPosition1);
        RaycastHit obstacle2 = this.GetObstacleHitInfo(spawnPosition2);

        if (obstacle1.transform != null)
        {
            adjustment1 = (this.gameObject.transform.position - obstacle1.point).normalized;
            this.spawnPosition1 = obstacle1.point + (adjustment1 * (splitBallRadius + 0.1f));
            this.launchVelocity1 = adjustment1 * scaledSplitSpeed;
        }

        if (obstacle2.transform != null)
        {
            adjustment2 = (this.gameObject.transform.position - obstacle2.point).normalized;
            this.spawnPosition2 = obstacle2.point + (adjustment2 * (splitBallRadius + 0.1f));
            this.launchVelocity2 = adjustment2 * scaledSplitSpeed;
        }        
    }

    public void UpdatePredictionLines(Vector3 splitDirection, float splitVelocityPercentage)
    {
        this.CalculateSpawnAndLaunchVectors(splitDirection, splitVelocityPercentage);

        this.predictionLine1.positionCount = this.predictionDistance;
        this.predictionLine2.positionCount = this.predictionDistance;

        this.predictionLine1.SetPosition(0, this.spawnPosition1);
        this.predictionLine2.SetPosition(0, this.spawnPosition2);

        Vector3 ghostPosition1 = this.spawnPosition1;
        Vector3 ghostPosition2 = this.spawnPosition2;

        Vector3 ghostVelocity1 = this.launchVelocity1;
        Vector3 ghostVelocity2 = this.launchVelocity2;

        for (int i = 1; i < this.predictionLine1.positionCount; i++)
        {
            ghostPosition1 = ghostPosition1 + (ghostVelocity1 * Time.fixedUnscaledDeltaTime);
            ghostPosition2 = ghostPosition2 + (ghostVelocity2 * Time.fixedUnscaledDeltaTime);

            this.predictionLine1.SetPosition(i, ghostPosition1);
            this.predictionLine2.SetPosition(i, ghostPosition2);

            RaycastHit hitInfo1 = this.GetObstacleHitInfo(ghostPosition1);
            RaycastHit hitInfo2 = this.GetObstacleHitInfo(ghostPosition2);

            if (hitInfo1.transform != null)
            { 
                ghostVelocity1 = Vector3.Reflect(ghostVelocity1, hitInfo1.normal);
            }
            if (hitInfo2.transform != null)
            {
                ghostVelocity2 = Vector3.Reflect(ghostVelocity2, hitInfo2.normal);
            }
        }

        this.framePredicted = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Vector3 reflectionVector = Vector3.Reflect(this.previousVelocity, collision.contacts[0].normal);
            this.rigidBody.velocity = reflectionVector;

            this.bounceBurstParticle.Stop();
            this.bounceBurstParticle.Play();
        }
    }

    public int GetScoreMultiplier()
    {
        float alignmentMultiplier = NEUTRAL_MULTIPLIER;
        float massMultiplier = this.baseMultiplier * this.rigidBody.mass;

        if (this.objectAlignment == Alignment.Good)
        {
            alignmentMultiplier = GOOD_MULTIPLIER;
        }
        else if (this.objectAlignment == Alignment.Bad)
        {
            alignmentMultiplier = BAD_MULTIPLIER;
        }

        return Mathf.RoundToInt(alignmentMultiplier * massMultiplier);
    }
}
