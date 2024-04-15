using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSplittableObject : MonoBehaviour
{    
    private Rigidbody parentRigidbody;

    private void Awake()
    {
        this.parentRigidbody = GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate()
    {
        this.transform.Rotate(new Vector3(this.parentRigidbody.velocity.y * Time.timeScale, this.parentRigidbody.velocity.x * Time.timeScale, 0.0f));
    }
}
