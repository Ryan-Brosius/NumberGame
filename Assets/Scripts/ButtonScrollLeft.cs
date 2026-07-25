using UnityEngine;

public class ButtonScrollLeft : MonoBehaviour
{
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float duration = 10f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.left * moveDistance;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveDistance / duration * Time.deltaTime
        );
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = startPosition;
        }
    }
}