using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    public float destroyDelay = 0.5f;
    public Transform tip;
    private Rigidbody rb;

    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb != null && rb.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.velocity.normalized) * Quaternion.Euler(rotationOffset);
            transform.rotation = targetRot;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
