using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField]
    private GameObject startingPoint;
    [SerializeField]
    private GameObject endingPoint;

    [SerializeField]
    private LayerMask splittableLayer;
    [SerializeField]
    private LayerMask collisionLayer;

    private float maxDistance = 5.0f;
    public float currentDistance = 0.0f;

    private bool isSetup = false;

    private bool controlsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        this.lineRenderer = GetComponentInChildren<LineRenderer>();

        LevelTimer.instance.onTimerCompleted -= this.DisableControls;
        LevelTimer.instance.onTimerCompleted += this.DisableControls;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.controlsDisabled == true)
        {
            if (this.isSetup == true)
            {
                this.CleanupSplitter();
            }

            return;
        }
    
        if (FocusModeManager.instance.focusActive == false)
        {
            if (this.isSetup == true)
            {
                this.CleanupSplitter();
            }

            return;
        }
    
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
                if (this.isSetup == true)
                {
                    this.UpdateSplitter(hit);
                    this.UpdatePredictionLines();
                }
                else
                {
                    this.SetupSplitter(hit);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && this.isSetup == true)
        {
            this.ExecuteSplits();
            this.CleanupSplitter();
        }
    }

    private void DisableControls()
    {
        this.controlsDisabled = true;
    }

    private void SetupSplitter(RaycastHit hit)
    {
        this.startingPoint.SetActive(true);
        this.lineRenderer.gameObject.SetActive(true);
        this.endingPoint.SetActive(true);

        this.lineRenderer.SetPosition(0, this.startingPoint.transform.position);
        this.lineRenderer.SetPosition(1, this.startingPoint.transform.position);

        this.startingPoint.transform.position = hit.point;

        if (FocusModeManager.instance.focusOnClick == true)
        {
            FocusModeManager.instance.EnterFocusMode(this.startingPoint.transform.position);
        }

        this.isSetup = true;
    }

    private void UpdateSplitter(RaycastHit hit)
    {
        this.endingPoint.transform.position = hit.point;

        if (Vector3.Distance(this.startingPoint.transform.position, this.endingPoint.transform.position) > this.maxDistance)
        {
            Vector3 clampedVector = (this.endingPoint.transform.position - this.startingPoint.transform.position).normalized * this.maxDistance;
            this.endingPoint.transform.position = this.startingPoint.transform.position + clampedVector;
        }

        this.currentDistance = Vector3.Distance(this.startingPoint.transform.position, this.endingPoint.transform.position);

        this.lineRenderer.SetPosition(0, this.startingPoint.transform.position);
        this.lineRenderer.SetPosition(1, this.endingPoint.transform.position);
    }

    private void UpdatePredictionLines()
    {
        Vector3 origin = this.startingPoint.transform.position;
        Vector3 direction = (this.endingPoint.transform.position - this.startingPoint.transform.position).normalized;
        RaycastHit[] hits = Physics.SphereCastAll(origin, 0.1f, direction, this.currentDistance, this.splittableLayer);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                SplittableObject splitComponent = hits[i].collider.gameObject.GetComponent<SplittableObject>();
                if (splitComponent != null)
                {
                    splitComponent.UpdatePredictionLines(direction, (this.currentDistance / this.maxDistance));
                }
            }
        }
    }   

    private void ExecuteSplits()
    {
        Vector3 origin = this.startingPoint.transform.position;
        Vector3 direction = (this.endingPoint.transform.position - this.startingPoint.transform.position).normalized;
        RaycastHit[] hits = Physics.SphereCastAll(origin, 0.1f, direction, this.currentDistance, this.splittableLayer);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                SplittableObject splitComponent = hits[i].collider.gameObject.GetComponent<SplittableObject>();
                if (splitComponent != null)
                {
                    splitComponent.Split(direction, (this.currentDistance / this.maxDistance));

                    if (TutorialManager.instance.hasFocused == false)
                    {
                        TutorialManager.instance.hasFocused = true;
                    }
                }
            }
        }
    }

    private void CleanupSplitter()
    {        
        this.startingPoint.SetActive(false);
        this.lineRenderer.gameObject.SetActive(false);
        this.endingPoint.SetActive(false);

        this.lineRenderer.SetPosition(0, this.startingPoint.transform.position);
        this.lineRenderer.SetPosition(1, this.startingPoint.transform.position);
        
        if (FocusModeManager.instance.focusOnClick == true)
        {
            FocusModeManager.instance.ExitFocusMode();
        }

        this.isSetup = false;
    }
}
