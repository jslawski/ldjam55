using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;    

    [SerializeField]
    private Transform cursorTransform;

    [SerializeField]
    private Image focusImage;

    [SerializeField]
    private Image cursorImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        this.cursorTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
    }

    public void HideFocusMeter()
    {
        this.focusImage.enabled = false;
    }

    public void SetCursorToBlack()
    {
        this.cursorImage.color = Color.black;
    }

    public void SetCursorToWhite()
    {
        this.cursorImage.color = Color.white;
    }
}
