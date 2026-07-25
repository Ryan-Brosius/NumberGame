using UnityEngine;

public class CarrotHorseCode : MonoBehaviour
{
    [Header("cursor")]
    public Transform cursorTransform;
    [Header("follow settings")]
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float stoppingDistance = 2f;
    [SerializeField] private float moveSpeed = 5f;
    private Vector3 lastCursorPosition;

    private void Start()
    {
        if (cursorTransform != null)
        {
            lastCursorPosition = cursorTransform.position;
        }
    }

    private void Update()
    {
        if (cursorTransform == null)
            return;
        Vector3 cursorMovement = cursorTransform.position - lastCursorPosition;
        if (cursorMovement.sqrMagnitude > 0.001f)
        {
            cursorMovement.Normalize();
        }
        lastCursorPosition = cursorTransform.position;
        if (Vector3.Distance(transform.position, cursorTransform.position) <= stoppingDistance)
        {
            return;
        }
        Vector3 targetPosition =
            cursorTransform.position - cursorMovement * followDistance;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }
}