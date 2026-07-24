using UnityEngine;

public class CentipedeMode : MonoBehaviour
{
    [Header("centipede Segments")]
    [SerializeField] private RectTransform[] segments = new RectTransform[10];

    [Header("movement")]
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private float segmentSpacing = 50f;

    [Header("turning")]
    [SerializeField] private float turnChance = 0.02f;
    private RectTransform canvasRectTransform;
    private Vector2[] positionHistory;
    private float historyResolution = 1f;
    private Vector2 direction;
    private float baseMoveSpeed;

    void Start()
    {
        canvasRectTransform = GetComponentInParent<Canvas>()
            .GetComponent<RectTransform>();

        direction = Random.insideUnitCircle.normalized;
        if (direction.magnitude < 0.1f)
        {
            direction = Vector2.right;
        }

        baseMoveSpeed = Random.Range(100f, 577f);
        moveSpeed = baseMoveSpeed;
        int totalHistoryLength = Mathf.CeilToInt(
            (segmentSpacing * (segments.Length - 1)) / historyResolution
        ) + 2;
        positionHistory = new Vector2[totalHistoryLength];
        Vector2 startPosition = segments[0].anchoredPosition;
        for (int i = 0; i < positionHistory.Length; i++)
        {
            positionHistory[i] = startPosition;
        }
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].anchoredPosition = startPosition;
        }
    }

    void Update()
    {
        if (segments == null || segments.Length != 10)
        {
            return;
        }
        if (positionHistory == null)
        {
            return;
        }
        int disabledSegments = 0;
        for (int i = 0; i < segments.Length; i++)
        {
            if (!segments[i].gameObject.activeSelf)
            {
                disabledSegments++;
            }
        }
        moveSpeed = baseMoveSpeed * (1f + disabledSegments * 0.15f);
        RectTransform head = segments[0];
        Vector2 oldPosition = head.anchoredPosition;
        head.anchoredPosition +=
            direction * moveSpeed * Time.deltaTime;
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;
        float headWidth = head.rect.width;
        float headHeight = head.rect.height;
        float leftBound = -canvasWidth / 2f + headWidth / 2f;
        float rightBound = canvasWidth / 2f - headWidth / 2f;
        float bottomBound = -canvasHeight / 2f + headHeight / 2f;
        float topBound = canvasHeight / 2f - headHeight / 2f;
        bool hitBoundary = false;
        if (head.anchoredPosition.x <= leftBound)
        {
            head.anchoredPosition = new Vector2(
                leftBound,
                head.anchoredPosition.y
            );
            direction.x = Mathf.Abs(direction.x);
            hitBoundary = true;
        }
        else if (head.anchoredPosition.x >= rightBound)
        {
            head.anchoredPosition = new Vector2(
                rightBound,
                head.anchoredPosition.y
            );

            direction.x = -Mathf.Abs(direction.x);
            hitBoundary = true;
        }
        if (head.anchoredPosition.y <= bottomBound)
        {
            head.anchoredPosition = new Vector2(
                head.anchoredPosition.x,
                bottomBound
            );

            direction.y = Mathf.Abs(direction.y);
            hitBoundary = true;
        }
        else if (head.anchoredPosition.y >= topBound)
        {
            head.anchoredPosition = new Vector2(
                head.anchoredPosition.x,
                topBound
            );

            direction.y = -Mathf.Abs(direction.y);
            hitBoundary = true;
        }
        float distanceMoved = Vector2.Distance(
            oldPosition,
            head.anchoredPosition
        );
        if (distanceMoved > 0f)
        {
            ShiftHistory(distanceMoved);
        }
        positionHistory[0] = head.anchoredPosition;
        for (int i = 1; i < segments.Length; i++)
        {
            float targetDistance = segmentSpacing * i;
            int historyIndex = Mathf.Clamp(
                Mathf.RoundToInt(targetDistance / historyResolution),
                0,
                positionHistory.Length - 1
            );
            segments[i].anchoredPosition =
                positionHistory[historyIndex];
        }
        if (hitBoundary && Random.value < turnChance)
        {
            TurnRandomly();
        }
    }

    private void ShiftHistory(float distanceMoved)
    {
        int shiftAmount = Mathf.FloorToInt(
            distanceMoved / historyResolution
        );
        if (shiftAmount <= 0)
        {
            return;
        }
        shiftAmount = Mathf.Min(
            shiftAmount,
            positionHistory.Length - 1
        );
        for (int i = positionHistory.Length - 1;
             i >= shiftAmount;
             i--)
        {
            positionHistory[i] =
                positionHistory[i - shiftAmount];
        }
        for (int i = 0; i < shiftAmount; i++)
        {
            positionHistory[i] =
                segments[0].anchoredPosition;
        }
    }

    private void TurnRandomly()
    {
        if (Random.value < 0.5f)
        {
            direction = new Vector2(
                -direction.y,
                direction.x
            );
        }
        else
        {
            direction = new Vector2(
                direction.y,
                -direction.x
            );
        }
        direction.Normalize();
    }
}