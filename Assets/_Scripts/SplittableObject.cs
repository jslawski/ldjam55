using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject splittableObjectPrefab;
    [SerializeField]
    private float splitScalePercent = 0.75f;
    [SerializeField]
    private Rigidbody rigidBody;

    private void SpawnNewSplittableObjects(Vector3 splitDirection)
    {
        GameObject firstObject = Instantiate(this.splittableObjectPrefab, this.gameObject.transform.position, new Quaternion());
        GameObject secondObject = Instantiate(this.splittableObjectPrefab, this.gameObject.transform.position, new Quaternion());

        firstObject.transform.localScale = this.gameObject.transform.localScale * this.splitScalePercent;
        secondObject.transform.localScale = this.gameObject.transform.localScale * this.splitScalePercent;

        Vector3 launchDirection = Vector2.Perpendicular(splitDirection).normalized * 3.0f;

        firstObject.GetComponent<SplittableObject>().Launch(-launchDirection);
        secondObject.GetComponent<SplittableObject>().Launch(launchDirection);
    }

    public void Launch(Vector3 launchDirection)
    {
        this.rigidBody.AddForce(launchDirection, ForceMode.Impulse);
    }

    public void Split(Vector3 splitDirection)
    {
        this.SpawnNewSplittableObjects(splitDirection);
        Destroy(this.gameObject);
    }

    private void Merge(GameObject other)
    {
        Debug.LogError("Merging with " + other.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Splittable")
        {
            this.Merge(collision.collider.gameObject);
        }
    }
}
