using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private Transform cursorTransform;
    
    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        this.cursorTransform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
    }
}
