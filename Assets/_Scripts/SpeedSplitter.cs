using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousPosition
{
    public Vector3 position;
    public float deltaTime;

    public PreviousPosition(Vector3 position, float deltaTime)
    {
        this.position = position;
        this.deltaTime = deltaTime;
    }

    public PreviousPosition(PreviousPosition newPosition)
    {
        this.position = newPosition.position;
        this.deltaTime = newPosition.deltaTime;
    }
}

public class SpeedSplitter : MonoBehaviour
{
    public bool isDebug = false;

    [SerializeField]
    private GameObject debugPreviousPosition;
    [SerializeField]
    private GameObject debugCurrentPosition;
    [SerializeField]
    private LineRenderer debugLineRenderer;

    [SerializeField]
    private int maxPreviousPositions = 10;
    private Queue<PreviousPosition> previousPositions;

    private Vector3 currentPosition;

    [SerializeField]
    private LayerMask splittableLayer;
    [SerializeField]
    private LayerMask collisionLayer;

    private float sphereRadius = 0.1f;

    private float minSpeedThreshold = 10f;
    private float maxSpeed = 120.0f;

    private void Awake()
    {
        this.previousPositions = new Queue<PreviousPosition>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit, float.PositiveInfinity, this.collisionLayer))
            {
                this.SetupSplitter(hit);
            }
        }
        if (Input.GetMouseButton(0) == true)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit, float.PositiveInfinity, this.collisionLayer))
            {
                this.UpdateSplitter(hit);
                this.ExecuteSplits();
            }
        }
        if (Input.GetMouseButtonUp(0) == true)
        {
            this.CleanupSplitter();
        }
    }

    private void AddPreviousPosition(Vector3 newPosition)
    {
        PreviousPosition newEntry = new PreviousPosition(newPosition, Time.deltaTime);

        this.previousPositions.Enqueue(newEntry);

        if (this.previousPositions.Count > this.maxPreviousPositions)
        {
            this.previousPositions.Dequeue();
        }
    }

    private Vector3 GetDirectionVector()
    {
        return (this.currentPosition - this.previousPositions.Peek().position).normalized;
    }

    private float GetSpeed()
    {
        float distance = Vector3.Distance(this.currentPosition, this.previousPositions.Peek().position);
        float elapsedTime = 0.0f;

        Queue<PreviousPosition> tempQueue = new Queue<PreviousPosition>();

        int queueCount = this.previousPositions.Count;

        for (int i = 0; i < queueCount; i++)
        {
            PreviousPosition entry = this.previousPositions.Dequeue();
            tempQueue.Enqueue(entry);
            elapsedTime += entry.deltaTime;
        }

        this.previousPositions = tempQueue;

        float currentSpeed = (distance / elapsedTime);

        if (currentSpeed > this.maxSpeed)
        {
            currentSpeed = this.maxSpeed;
        }

        return currentSpeed;
    }

    private void SetupSplitter(RaycastHit hit)
    {        
        this.currentPosition = hit.point;
        this.AddPreviousPosition(this.currentPosition);

        if (FocusModeManager.instance.focusOnClick == true)
        {
            FocusModeManager.instance.EnterFocusMode(this.currentPosition);
        }

        if (this.isDebug == true)
        {
            this.SetupDebugSplitter();
        }
    }

    private void SetupDebugSplitter()
    {
        this.debugPreviousPosition.SetActive(true);
        this.debugPreviousPosition.transform.position = this.previousPositions.Peek().position;
        this.debugCurrentPosition.SetActive(true);
        this.debugCurrentPosition.transform.position = this.currentPosition;
        this.debugLineRenderer.gameObject.SetActive(true);
        this.debugLineRenderer.SetPosition(0, this.previousPositions.Peek().position);
        this.debugLineRenderer.SetPosition(1, this.currentPosition);
    }

    private void UpdateSplitter(RaycastHit hit)
    {
        this.AddPreviousPosition(this.currentPosition);
        this.currentPosition = hit.point;
        
        if (this.isDebug == true)
        {
            this.UpdateDebugSplitter();
        }
    }

    private void UpdateDebugSplitter()
    {
        this.debugPreviousPosition.transform.position = this.previousPositions.Peek().position;
        this.debugCurrentPosition.transform.position = this.currentPosition;
        this.debugLineRenderer.SetPosition(0, this.previousPositions.Peek().position);
        this.debugLineRenderer.SetPosition(1, this.currentPosition);
    }

    private void ExecuteSplits()
    {
        if (this.GetSpeed() < this.minSpeedThreshold)
        {
            return;
        }


        Vector3 sphereCastDirection = this.currentPosition - this.previousPositions.Peek().position;

        RaycastHit[] hits = Physics.SphereCastAll(this.previousPositions.Peek().position, this.sphereRadius, sphereCastDirection.normalized, sphereCastDirection.magnitude, this.splittableLayer);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                SplittableObject splitComponent = hits[i].collider.gameObject.GetComponent<SplittableObject>();
                if (splitComponent != null && splitComponent.splittable == true)
                {
                
                    splitComponent.Split(this.GetDirectionVector(), (this.GetSpeed() / this.maxSpeed));
                }
            }
        }
    }

    private void CleanupSplitter()
    {
        this.previousPositions.Clear();
        this.currentPosition = Vector3.zero;

        if (this.isDebug == true)
        {
            this.CleanupDebugSplitter();
        }

        if (FocusModeManager.instance.focusOnClick == true)
        {
            FocusModeManager.instance.ExitFocusMode();
        }
    }

    private void CleanupDebugSplitter()
    {
        this.debugPreviousPosition.SetActive(false);
        this.debugCurrentPosition.SetActive(false);
        this.debugLineRenderer.SetPosition(0, Vector3.zero);
        this.debugLineRenderer.SetPosition(1, Vector3.zero);
        this.debugLineRenderer.gameObject.SetActive(false);
    }
}
