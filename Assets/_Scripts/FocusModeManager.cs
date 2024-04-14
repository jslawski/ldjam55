using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusModeManager : MonoBehaviour
{
    public static FocusModeManager instance;

    private float focusTimeScale = 0.05f;
    private float maxFixedTime;

    [SerializeField]
    private AnimationCurve enterCurve;
    [SerializeField]
    private AnimationCurve exitCurve;

    private float enterDuration = 0.25f;

    [SerializeField]
    private Transform cameraHolderTransform;
    private Vector3 originalCameraHolderPosition;
    
    private float originalCameraFOV;
    [SerializeField]
    private float targetCameraFOV = 50.0f;

    private Vector3 originalCameraRotation;
    private Vector3 targetCameraRotation = Vector3.zero;

    [SerializeField]
    private LayerMask collisionLayer;

    private Vector3 targetCameraHolderPosition;

    public bool focusOnClick = false;

    private float totalFocusTime = 5.0f;
    private float currentFocusTime = 0.0f;

    private bool focusDepleted = false;

    private bool focusActive = false;

    private Coroutine focusReplenishCoroutine = null;

    [SerializeField]
    private Image focusMeterImage;

    private float tValue = 0.0f;

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
        this.originalCameraRotation = Camera.main.transform.rotation.eulerAngles;
        this.targetCameraRotation = this.originalCameraRotation;

        this.currentFocusTime = this.totalFocusTime;
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
        
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit, float.PositiveInfinity, this.collisionLayer))
            {
                if (this.focusActive == false)
                {
                    this.EnterFocusMode(hit.point);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(1))
        {
            if (this.focusActive == true)
            {
                this.ExitFocusMode();
            }
        }
    }

    public void SetTargetPoint(Vector3 targetPoint)
    {
        this.targetCameraHolderPosition = this.originalCameraHolderPosition;    
    
    //this.targetCameraHolderPosition = new Vector3(0, 0, this.cameraHolderTransform.position.z);
        //this.targetCameraHolderPosition = new Vector3(targetPoint.x, targetPoint.y - (this.GetIsometricYOffset() * 2.0f), this.cameraHolderTransform.position.z);
    }

    private float GetIsometricYOffset()
    {
        return Camera.main.transform.position.z * Mathf.Tan(Camera.main.transform.rotation.x);
    }

    public void EnterFocusMode(Vector3 targetPoint)
    {           
        if (this.focusDepleted == true)
        {
            return;
        }
    
        this.focusActive = true;

        this.SetTargetPoint(targetPoint);
        StartCoroutine(this.FocusModeCoroutine());
        StartCoroutine(this.DepleteFocus());
    }

    private IEnumerator FocusModeCoroutine()
    {
        float incrementPerFrame;
        
        while (this.tValue < 0.98f && this.focusActive == true)
        {        
            float curveValue = this.enterCurve.Evaluate(tValue);
            
            Time.timeScale = Mathf.Lerp(1.0f, this.focusTimeScale, curveValue);
            Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);

            Camera.main.fieldOfView = Mathf.Lerp(this.originalCameraFOV, this.targetCameraFOV, curveValue);
            this.cameraHolderTransform.position = Vector3.Lerp(this.originalCameraHolderPosition, targetCameraHolderPosition, curveValue);

            Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(this.originalCameraRotation), Quaternion.Euler(this.targetCameraRotation), curveValue);

            incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;

            tValue += incrementPerFrame;

            yield return null;
        }

        if (this.tValue >= 0.98)
        {
            Time.timeScale = this.focusTimeScale;
            Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);
            Camera.main.fieldOfView = this.targetCameraFOV;
            this.cameraHolderTransform.position = this.targetCameraHolderPosition;
            Camera.main.transform.rotation = Quaternion.Euler(this.targetCameraRotation);
            this.tValue = 1.0f;
        }
    }

    private IEnumerator DepleteFocus()
    {
        float percentageIncrement;
        float remainingPercentage = (this.currentFocusTime / this.totalFocusTime);
    
        while (remainingPercentage >= 0.0f && this.focusActive == true)
        {
            percentageIncrement = (1.0f / this.totalFocusTime) * Time.unscaledDeltaTime;

            remainingPercentage -= percentageIncrement;

            this.currentFocusTime = this.totalFocusTime * remainingPercentage;

            this.focusMeterImage.fillAmount = (this.currentFocusTime / this.totalFocusTime);

            yield return null;
        }

        if (remainingPercentage <= 0.0f)
        {
            this.currentFocusTime = 0.0f;
            this.focusDepleted = true;
            this.focusMeterImage.color = Color.gray;
            this.focusMeterImage.fillAmount = 0.0f;
            this.ExitFocusMode();
        }
    }

    public void ExitFocusMode()
    {
        this.focusActive = false;
        
        StartCoroutine(this.ExitFocusModeCoroutine());

        if (this.focusReplenishCoroutine == null)
        {
            this.focusReplenishCoroutine = StartCoroutine(this.ReplenishFocus());
        }
    }

    private IEnumerator ReplenishFocus()
    {
        float percentageIncrement;
        float remainingPercentage = (this.currentFocusTime / this.totalFocusTime);

        while (remainingPercentage < 0.98f && ((this.focusDepleted == true) || (this.focusActive == false)))
        {
            percentageIncrement = (1.0f / this.totalFocusTime) * Time.unscaledDeltaTime;

            remainingPercentage += percentageIncrement;

            this.currentFocusTime = this.totalFocusTime * remainingPercentage;

            this.focusMeterImage.fillAmount = (this.currentFocusTime / this.totalFocusTime);

            yield return null;

            }

        if (remainingPercentage >= 0.98f)
        {
            this.currentFocusTime = this.totalFocusTime;
            this.focusMeterImage.fillAmount = 1.0f;
        }

        if (this.focusDepleted == true && this.currentFocusTime == this.totalFocusTime)
        {
            this.focusDepleted = false;
            this.focusMeterImage.color = Color.white;
        }
        
        this.focusReplenishCoroutine = null;
    }

    private IEnumerator ExitFocusModeCoroutine()
    {
        float incrementPerFrame;

        while (this.tValue > 0.0f && this.focusActive == false)
        {
            float curveValue = this.enterCurve.Evaluate(tValue);

            Time.timeScale = Mathf.Lerp(1.0f, this.focusTimeScale, curveValue);
            Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);

            Camera.main.fieldOfView = Mathf.Lerp(this.originalCameraFOV, this.targetCameraFOV, curveValue);
            this.cameraHolderTransform.position = Vector3.Lerp(this.originalCameraHolderPosition, targetCameraHolderPosition, curveValue);

            Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(this.originalCameraRotation), Quaternion.Euler(this.targetCameraRotation), curveValue);

            incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;
            tValue -= incrementPerFrame;

            yield return null;
        }

        if (this.tValue <= 0.0f)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = this.maxFixedTime;
            Camera.main.fieldOfView = this.originalCameraFOV;
            this.cameraHolderTransform.position = this.originalCameraHolderPosition;
            Camera.main.transform.rotation = Quaternion.Euler(this.originalCameraRotation);
            this.tValue = 0.0f;
        }
    }
}
