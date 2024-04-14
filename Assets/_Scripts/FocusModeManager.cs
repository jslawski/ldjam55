using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusModeManager : MonoBehaviour
{
    public static FocusModeManager instance;

    private float focusTimeScale = 0.05f;
    private float maxFixedTime;

    [SerializeField]
    private AnimationCurve slowDownCurve;

    private float enterDuration = 0.5f;

    [SerializeField]
    private Transform cameraHolderTransform;
    private Vector3 originalCameraHolderPosition;
    private float originalCameraFOV;
    private float targetCameraFOV = 50.0f;

    [SerializeField]
    private LayerMask collisionLayer;

    private Vector3 targetCameraHolderPosition;

    public bool focusOnClick = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.maxFixedTime = Time.fixedDeltaTime;
        this.originalCameraFOV = Camera.main.fieldOfView;
        this.originalCameraHolderPosition = this.cameraHolderTransform.position;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            this.focusOnClick = !this.focusOnClick;
        }
    
        if (this.focusOnClick == true)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit, float.PositiveInfinity, this.collisionLayer))
            {
                this.EnterFocusMode(hit.point);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            this.ExitFocusMode();
        }        
    }

    public void SetTargetPoint(Vector3 targetPoint)
    {
        this.targetCameraHolderPosition = new Vector3(targetPoint.x, targetPoint.y - (this.GetIsometricYOffset() * 2.0f), this.cameraHolderTransform.position.z);
    }

    private float GetIsometricYOffset()
    {
        return Camera.main.transform.position.z * Mathf.Tan(Camera.main.transform.rotation.x);
    }

    public void EnterFocusMode(Vector3 targetPoint)
    {
        this.SetTargetPoint(targetPoint);
        StartCoroutine(this.FocusModeCoroutine());
    }

    private IEnumerator FocusModeCoroutine()
    {
        float tValue = 0.0f;

        float incrementPerFrame;

        while (tValue < 0.98f)
        {
            float curveValue = this.slowDownCurve.Evaluate(tValue);
            
            Time.timeScale = Mathf.Lerp(1.0f, this.focusTimeScale, curveValue);
            Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);

            Camera.main.fieldOfView = Mathf.Lerp(this.originalCameraFOV, this.targetCameraFOV, curveValue);
            this.cameraHolderTransform.position = Vector3.Lerp(this.originalCameraHolderPosition, targetCameraHolderPosition, curveValue);

            incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;            
            tValue += incrementPerFrame;
            
            yield return null;
        }

        Time.timeScale = this.focusTimeScale;
        Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);
        Camera.main.fieldOfView = this.targetCameraFOV;
        this.cameraHolderTransform.position = this.targetCameraHolderPosition;

        //StartCoroutine(this.CameraZoomCoroutine());
    }
    /*
    private IEnumerator CameraZoomCoroutine()
    {
        float elapsedTime = 0.0f;

        while (Input.GetKey(KeyCode.Space))
        {
            Debug.LogError(Camera.main.fieldOfView * this.postZoomFOVIncrementPercent);
  
            

            Camera.main.fieldOfView = Camera.main.fieldOfView * (1.0f / (elapsedTime + 1));

            elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }
    }
    */
    public void ExitFocusMode()
    {
        this.StopAllCoroutines();
        StartCoroutine(this.ExitFocusModeCoroutine());
    }

    private IEnumerator ExitFocusModeCoroutine()
    {
        float tValue = 0.0f;

        float incrementPerFrame;

        while (tValue < 0.98f)
        {
            float curveValue = this.slowDownCurve.Evaluate(tValue);
            Time.timeScale = Mathf.Lerp(this.focusTimeScale, 1.0f, curveValue);
            Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);

            Camera.main.fieldOfView = Mathf.Lerp(this.targetCameraFOV, this.originalCameraFOV, curveValue);
            this.cameraHolderTransform.position = Vector3.Lerp(this.targetCameraHolderPosition, this.originalCameraHolderPosition, curveValue);

            incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;
            tValue += incrementPerFrame;

            yield return null;
        }

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = this.maxFixedTime;
        Camera.main.fieldOfView = this.originalCameraFOV;
        this.cameraHolderTransform.position = this.originalCameraHolderPosition;
    }
}
