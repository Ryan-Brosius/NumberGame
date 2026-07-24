using UnityEngine;

public class SquidMode : MonoBehaviour
{
    [Header("propulsion")]
    [SerializeField] private float minPropelSpeed = 500f;
    [SerializeField] private float maxPropelSpeed = 1000f;
    [SerializeField] private float minPropelTime = 0.3f;
    [SerializeField] private float maxPropelTime = 0.8f;
    [SerializeField] private float minIdleTime = 0.5f;
    [SerializeField] private float maxIdleTime = 2f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("squash")]
    [SerializeField] private bool useSquash = true;
    [SerializeField] private float squashAmount = 0.8f;
    [SerializeField] private float squashSpeed = 8f;

    [Header("ink splat")]
    [SerializeField] private GameObject inkSplat;

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;
    private Vector2 direction;
    private Vector2 targetDirection;
    private float moveSpeed;
    private float stateTimer;
    private bool isPropelling;
    private Vector3 originalScale;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        originalScale = rectTransform.localScale;

        direction = Random.insideUnitCircle.normalized;

        if (direction.magnitude < 0.1f)
        {
            direction = Vector2.up;
        }

        targetDirection = direction;

        RotateInstantly();
        StartIdle();
    }

    void OnDisable()
    {
        if (inkSplat == null)
        {
            return;
        }

        GameObject splat = Instantiate(
            inkSplat,
            transform.position,
            Quaternion.identity,
            transform.parent
        );

        splat.name = "InkSplat";
        splat.SetActive(true);
    }

    void Update()
    {
        if (canvasRectTransform == null)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        if (isPropelling)
        {
            Propel();
        }
        else
        {
            IdleAndTurn();
        }

        HandleCanvasBounds();
        HandleSquash();
    }

    void StartIdle()
    {
        isPropelling = false;

        stateTimer = Random.Range(
            minIdleTime,
            maxIdleTime
        );

        targetDirection = Random.insideUnitCircle.normalized;

        if (targetDirection.magnitude < 0.1f)
        {
            targetDirection = Vector2.up;
        }
    }

    void IdleAndTurn()
    {
        RotateTowardsTarget();

        float angleDifference = Vector2.Angle(
            rectTransform.up,
            targetDirection
        );

        if (angleDifference < 2f)
        {
            StartPropulsion();
        }
    }

    void StartPropulsion()
    {
        isPropelling = true;

        direction = targetDirection.normalized;

        moveSpeed = Random.Range(
            minPropelSpeed,
            maxPropelSpeed
        );

        stateTimer = Random.Range(
            minPropelTime,
            maxPropelTime
        );
    }

    void Propel()
    {
        rectTransform.anchoredPosition +=
            direction * moveSpeed * Time.deltaTime;

        RotateTowardsDirection();

        if (stateTimer <= 0f)
        {
            StartIdle();
        }
    }

    void RotateTowardsTarget()
    {
        if (targetDirection.magnitude < 0.01f)
        {
            return;
        }

        float targetAngle =
            Mathf.Atan2(
                targetDirection.y,
                targetDirection.x
            ) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation =
            Quaternion.Euler(
                0f,
                0f,
                targetAngle
            );

        rectTransform.rotation = Quaternion.RotateTowards(
            rectTransform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void RotateTowardsDirection()
    {
        if (direction.magnitude < 0.01f)
        {
            return;
        }

        float targetAngle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation =
            Quaternion.Euler(
                0f,
                0f,
                targetAngle
            );

        rectTransform.rotation = Quaternion.RotateTowards(
            rectTransform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void RotateInstantly()
    {
        float targetAngle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg - 90f;

        rectTransform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                targetAngle
            );
    }

    void HandleCanvasBounds()
    {
        float canvasWidth = canvasRectTransform.rect.width;
        float canvasHeight = canvasRectTransform.rect.height;
        float buttonWidth = rectTransform.rect.width;
        float buttonHeight = rectTransform.rect.height;
        float leftBound = -canvasWidth / 2f + buttonWidth / 2f;
        float rightBound = canvasWidth / 2f - buttonWidth / 2f;
        float bottomBound = -canvasHeight / 2f + buttonHeight / 2f;
        float topBound = canvasHeight / 2f - buttonHeight / 2f;

        if (rectTransform.anchoredPosition.x <= leftBound)
        {
            rectTransform.anchoredPosition =
                new Vector2(
                    leftBound,
                    rectTransform.anchoredPosition.y
                );

            direction.x = Mathf.Abs(direction.x);
            targetDirection.x = Mathf.Abs(targetDirection.x);
        }
        else if (rectTransform.anchoredPosition.x >= rightBound)
        {
            rectTransform.anchoredPosition =
                new Vector2(
                    rightBound,
                    rectTransform.anchoredPosition.y
                );

            direction.x = -Mathf.Abs(direction.x);
            targetDirection.x = -Mathf.Abs(targetDirection.x);
        }

        if (rectTransform.anchoredPosition.y <= bottomBound)
        {
            rectTransform.anchoredPosition =
                new Vector2(
                    rectTransform.anchoredPosition.x,
                    bottomBound
                );

            direction.y = Mathf.Abs(direction.y);
            targetDirection.y = Mathf.Abs(targetDirection.y);
        }
        else if (rectTransform.anchoredPosition.y >= topBound)
        {
            rectTransform.anchoredPosition =
                new Vector2(
                    rectTransform.anchoredPosition.x,
                    topBound
                );

            direction.y = -Mathf.Abs(direction.y);
            targetDirection.y = -Mathf.Abs(targetDirection.y);
        }
    }

    void HandleSquash()
    {
        if (!useSquash)
        {
            return;
        }
        Vector3 targetScale = originalScale;
        if (isPropelling)
        {
            targetScale = new Vector3(
                originalScale.x * squashAmount,
                originalScale.y / squashAmount,
                originalScale.z
            );
        }
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            squashSpeed * Time.deltaTime
        );
    }
}