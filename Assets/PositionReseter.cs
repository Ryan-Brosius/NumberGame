using UnityEngine;

public class PositionReseter : MonoBehaviour
{
    [Header("Reset Settings")]
    public float seconds = 6f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(ResetPosition), seconds);
    }

    private void ResetPosition()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}