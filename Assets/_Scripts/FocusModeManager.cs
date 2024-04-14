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
    private AnimationCurve slowDownCurve;

    [SerializeField]
    private float enterDuration = 2.0f;

    [SerializeField]
    private Transform cameraHolderTransform;
    private Vector3 originalCameraHolderPosition;
    
    private float originalCameraFOV;
    [SerializeField]
    private float targetCameraFOV = 30.0f;

    private Vector3 originalCameraRotation;
    private Vector3 targetCameraRotation = Vector3.zero;

    [SerializeField]
    private LayerMask collisionLayer;

    private Vector3 targetCameraHolderPosition;

    public bool focusOnClick = false;

    public float totalFocusTime = 3.0f;
    private float currentFocusTime = 0.0f;
    private float focusChangePerFixedUpdate;

    private bool focusDepleted = false;

    private bool focusActive = false;

    private Coroutine focusReplenishCoroutine = null;

    [SerializeField]
    private Image focusMeterImage;

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

        this.currentFocusTime = this.totalFocusTime;
        this.focusChangePerFixedUpdate = this.totalFocusTime * Time.fixedUnscaledDeltaTime;
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
        this.targetCameraHolderPosition = new Vector3(0, 0, this.cameraHolderTransform.position.z);
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
        float tValue = 0.0f;

        float incrementPerFrame;

        while (tValue < 0.98f)
        {
            float curveValue = this.slowDownCurve.Evaluate(tValue);
            
            Time.timeScale = Mathf.Lerp(1.0f, this.focusTimeScale, curveValue);
            Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);

            Camera.main.fieldOfView = Mathf.Lerp(this.originalCameraFOV, this.targetCameraFOV, curveValue);
            this.cameraHolderTransform.position = Vector3.Lerp(this.originalCameraHolderPosition, targetCameraHolderPosition, curveValue);

            Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(this.originalCameraRotation), Quaternion.Euler(this.targetCameraRotation), curveValue);

            incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;            
            tValue += incrementPerFrame;
            
            yield return null;
        }

        Time.timeScale = this.focusTimeScale;
        Time.fixedDeltaTime = Mathf.Clamp(this.maxFixedTime * Time.timeScale, 0.0f, this.maxFixedTime);
        Camera.main.fieldOfView = this.targetCameraFOV;
        this.cameraHolderTransform.position = this.targetCameraHolderPosition;
        Camera.main.transform.rotation = Quaternion.Euler(this.targetCameraRotation);
    }

    private IEnumerator DepleteFocus()
    {
        while (this.currentFocusTime > 0.0f && this.focusActive == true)
        {
            this.currentFocusTime -= this.focusChangePerFixedUpdate;
            this.focusMeterImage.fillAmount = (this.currentFocusTime / this.totalFocusTime);
            yield return new WaitForFixedUpdate();
        }

        if (this.currentFocusTime <= 0.0f)
        {
            this.focusDepleted = true;
            this.ExitFocusMode();
        }
    }

    public void ExitFocusMode()
    {
        this.focusActive = false;
        
        this.StopAllCoroutines();
        StartCoroutine(this.ExitFocusModeCoroutine());

        if (this.focusReplenishCoroutine == null)
        {
            this.focusReplenishCoroutine = StartCoroutine(this.ReplenishFocus());
        }
    }

    private IEnumerator ReplenishFocus()
    {
        while ((this.currentFocusTime < this.totalFocusTime) && ((this.focusDepleted == true) || (this.focusActive == false)))
        {
            this.currentFocusTime += this.focusChangePerFixedUpdate;
            this.focusMeterImage.fillAmount = (this.currentFocusTime / this.totalFocusTime);
            yield return new WaitForFixedUpdate();
        }

        if (this.currentFocusTime > this.totalFocusTime)
        {
            this.currentFocusTime = this.totalFocusTime;
        }

        if (this.focusDepleted == true && this.currentFocusTime == this.totalFocusTime)
        {
            this.focusDepleted = false;
        }
        
        this.focusReplenishCoroutine = null;
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

            Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(this.targetCameraRotation), Quaternion.Euler(this.originalCameraRotation), curveValue);

            incrementPerFrame = (1.0f / this.enterDuration) * Time.unscaledDeltaTime;
            tValue += incrementPerFrame;

            yield return null;
        }

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = this.maxFixedTime;
        Camera.main.fieldOfView = this.originalCameraFOV;
        this.cameraHolderTransform.position = this.originalCameraHolderPosition;
        Camera.main.transform.rotation = Quaternion.Euler(this.originalCameraRotation);
    }
}
