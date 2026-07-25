using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

[RequireComponent(typeof(BoxCollider2D))]
public class SquidMode : MonoBehaviour
{
    [Header("propulsion")]
    [SerializeField] private float minPropelSpeed = 5f;
    [SerializeField] private float maxPropelSpeed = 10f;
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

    [Header("references")]
    [SerializeField] private Transform visuals;
    [SerializeField] private Camera boundsCamera;   // defaults to Camera.main
    [SerializeField] private PuzzleLogicController controller;

    private BoxCollider2D box;
    private Vector2 direction;
    private Vector2 targetDirection;
    private float moveSpeed;
    private float stateTimer;
    private bool isPropelling;
    private Vector3 originalScale;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();

        controller = FindFirstObjectByType<PuzzleLogicController>();

        if (boundsCamera == null)
            boundsCamera = Camera.main;

        if (visuals == null)
        {
            var view = GetComponentInChildren<NumberBlockView>();
            if (view != null)
                visuals = view.transform;
        }
    }

    void Start()
    {
        originalScale = visuals != null ? visuals.localScale : Vector3.one;

        direction = Random.insideUnitCircle.normalized;

        if (direction.magnitude < 0.1f)
        {
            direction = Vector2.up;
        }

        targetDirection = direction;
        controller.OnSequenceReset.AddListener(() => gameObject.SetActive(true));

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
        stateTimer -= Time.deltaTime;

        if (isPropelling)
        {
            Propel();
        }
        else
        {
            IdleAndTurn();
        }

        HandleCameraBounds();
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
            transform.up,
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
        transform.position +=
            (Vector3)(direction * moveSpeed * Time.deltaTime);

        RotateTowardsDirection();

        if (stateTimer <= 0f)
        {
            StartIdle();
        }
    }

    void RotateTowardsTarget()
    {
        RotateTowards(targetDirection);
    }

    void RotateTowardsDirection()
    {
        RotateTowards(direction);
    }

    void RotateTowards(Vector2 dir)
    {
        if (dir.magnitude < 0.01f)
        {
            return;
        }

        float targetAngle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, targetAngle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void RotateInstantly()
    {
        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation =
            Quaternion.Euler(0f, 0f, targetAngle);
    }

    void HandleCameraBounds()
    {
        float halfHeight = boundsCamera.orthographicSize;
        float halfWidth = halfHeight * boundsCamera.aspect;
        Vector3 camPos = boundsCamera.transform.position;

        Vector3 scale = transform.lossyScale;
        float halfW = box.size.x * 0.5f * Mathf.Abs(scale.x);
        float halfH = box.size.y * 0.5f * Mathf.Abs(scale.y);

        float leftBound = camPos.x - halfWidth + halfW;
        float rightBound = camPos.x + halfWidth - halfW;
        float bottomBound = camPos.y - halfHeight + halfH;
        float topBound = camPos.y + halfHeight - halfH;

        Vector3 pos = transform.position;

        if (pos.x <= leftBound)
        {
            pos.x = leftBound;
            direction.x = Mathf.Abs(direction.x);
            targetDirection.x = Mathf.Abs(targetDirection.x);
        }
        else if (pos.x >= rightBound)
        {
            pos.x = rightBound;
            direction.x = -Mathf.Abs(direction.x);
            targetDirection.x = -Mathf.Abs(targetDirection.x);
        }

        if (pos.y <= bottomBound)
        {
            pos.y = bottomBound;
            direction.y = Mathf.Abs(direction.y);
            targetDirection.y = Mathf.Abs(targetDirection.y);
        }
        else if (pos.y >= topBound)
        {
            pos.y = topBound;
            direction.y = -Mathf.Abs(direction.y);
            targetDirection.y = -Mathf.Abs(targetDirection.y);
        }

        transform.position = pos;
    }

    void HandleSquash()
    {
        if (!useSquash || visuals == null)
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

        visuals.localScale = Vector3.Lerp(
            visuals.localScale,
            targetScale,
            squashSpeed * Time.deltaTime
        );
    }
}