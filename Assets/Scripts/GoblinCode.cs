using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoblinCode : MonoBehaviour
{
    [Header("movement")]
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float wanderRadius = 15f;
    [SerializeField] private float stoppingDistance = 30f;
    [Header("button interaction")]
    [SerializeField] private float buttonWaitTime = 2f;
    [Header("retreat speed")]
    [SerializeField] private float retreatSpeed = 200f;
    [Header("pain squash & stretch (definitely optional)")]
    [SerializeField] private float squashAmount = 0.7f;
    [SerializeField] private float stretchAmount = 1.3f;
    [SerializeField] private float painDuration = 0.12f;
    [SerializeField] private float recoveryDuration = 0.2f;
    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;
    private Vector2 startingPosition;
    private Vector2 wanderTarget;
    private Button targetButton;
    private Button ownButton;
    private Vector3 originalScale;
    private float waitTimer = 0f;
    private Coroutine painCoroutine;

    private enum GoblinState
    {
        Wandering,
        WalkingToButton,
        WaitingAtButton,
        Retreating
    }

    private GoblinState currentState;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
        startingPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        ownButton = GetComponent<Button>();

        if (ownButton != null)
        {
            ownButton.onClick.AddListener(RetreatToStartingPosition);
        }
        ChooseNewWanderTarget();
        currentState = GoblinState.Wandering;
    }

    void Update()
    {
        if (rectTransform == null)
        {
            return;
        }
        if (currentState != GoblinState.Retreating)
        {
            if (targetButton == null)
            {
                FindInactiveButton();
            }
        }

        switch (currentState)
        {
            case GoblinState.Wandering:
                Wander();
                break;
            case GoblinState.WalkingToButton:
                WalkToButton();
                break;
            case GoblinState.WaitingAtButton:
                WaitAtButton();
                break;
            case GoblinState.Retreating:
                Retreat();
                break;
        }
    }

    private void Wander()
    {
        if (targetButton != null)
        {
            currentState = GoblinState.WalkingToButton;
            return;
        }
        Vector2 currentPosition = rectTransform.anchoredPosition;
        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            wanderTarget,
            moveSpeed * Time.deltaTime
        );
        rectTransform.anchoredPosition = newPosition;
        if (Vector2.Distance(newPosition, wanderTarget) < 1f)
        {
            ChooseNewWanderTarget();
        }
    }

    private void WalkToButton()
    {
        if (targetButton == null)
        {
            currentState = GoblinState.Wandering;
            ChooseNewWanderTarget();
            return;
        }
        RectTransform buttonRect =
            targetButton.GetComponent<RectTransform>();
        if (buttonRect == null)
        {
            targetButton = null;
            currentState = GoblinState.Wandering;
            return;
        }

        Vector2 targetPosition = buttonRect.anchoredPosition;
        Vector2 currentPosition = rectTransform.anchoredPosition;

        float distance = Vector2.Distance(
            currentPosition,
            targetPosition
        );
        if (distance <= stoppingDistance)
        {
            waitTimer = 0f;
            currentState = GoblinState.WaitingAtButton;
            return;
        }
        rectTransform.anchoredPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void WaitAtButton()
    {
        if (targetButton == null)
        {
            currentState = GoblinState.Wandering;
            return;
        }

        RectTransform buttonRect =
            targetButton.GetComponent<RectTransform>();
        if (buttonRect == null)
        {
            targetButton = null;
            currentState = GoblinState.Wandering;
            return;
        }
        float distance = Vector2.Distance(
            rectTransform.anchoredPosition,
            buttonRect.anchoredPosition
        );
        if (distance > stoppingDistance)
        {
            waitTimer = 0f;
            currentState = GoblinState.WalkingToButton;
            return;
        }
        waitTimer += Time.deltaTime;
        if (waitTimer >= buttonWaitTime)
        {
            ClickButton();

            targetButton = null;
            waitTimer = 0f;

            ChooseNewWanderTarget();
            currentState = GoblinState.Wandering;
        }
    }

    private void Retreat()
    {
        rectTransform.anchoredPosition = Vector2.MoveTowards(
            rectTransform.anchoredPosition,
            startingPosition,
            retreatSpeed * Time.deltaTime
        );
        if (Vector2.Distance(
            rectTransform.anchoredPosition,
            startingPosition
        ) < 0.1f)
        {
            rectTransform.anchoredPosition = startingPosition;
            targetButton = null;
            waitTimer = 0f;
            ChooseNewWanderTarget();
            currentState = GoblinState.Wandering;
        }
    }

    private void FindInactiveButton()
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button button in buttons)
        {
            if (button == null)
            {
                continue;
            }
            if (button == ownButton)
            {
                continue;
            }
            if (!button.gameObject.activeSelf)
            {
                targetButton = button;
                currentState = GoblinState.WalkingToButton;
                return;
            }
        }
    }

    private void ClickButton()
    {
        if (targetButton == null)
        {
            return;
        }
        targetButton.gameObject.SetActive(true);
        targetButton.onClick.Invoke();
    }

    private void RetreatToStartingPosition()
    {
        PlayPainAnimation();
        targetButton = null;
        waitTimer = 0f;
        currentState = GoblinState.Retreating;
    }

    private void PlayPainAnimation()
    {
        if (painCoroutine != null)
        {
            StopCoroutine(painCoroutine);
        }
        painCoroutine = StartCoroutine(PainSquashStretch());
    }

    private IEnumerator PainSquashStretch()
    {
        Vector3 squashScale = new Vector3(
            originalScale.x * stretchAmount,
            originalScale.y * squashAmount,
            originalScale.z
        );

        Vector3 stretchScale = new Vector3(
            originalScale.x * squashAmount,
            originalScale.y * stretchAmount,
            originalScale.z
        );
        float timer = 0f;
        while (timer < painDuration)
        {
            timer += Time.deltaTime;
            float t = timer / painDuration;
            rectTransform.localScale = Vector3.Lerp(
                originalScale,
                squashScale,
                t
            );
            yield return null;
        }
        timer = 0f;
        while (timer < painDuration)
        {
            timer += Time.deltaTime;
            float t = timer / painDuration;
            rectTransform.localScale = Vector3.Lerp(
                squashScale,
                stretchScale,
                t
            );
            yield return null;
        }
        timer = 0f;
        while (timer < recoveryDuration)
        {
            timer += Time.deltaTime;

            float t = timer / recoveryDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            rectTransform.localScale = Vector3.Lerp(
                stretchScale,
                originalScale,
                t
            );
            yield return null;
        }
        rectTransform.localScale = originalScale;
        painCoroutine = null;
    }

    private void ChooseNewWanderTarget()
    {
        float randomX = Random.Range(
            -wanderRadius,
            wanderRadius
        );
        float randomY = Random.Range(
            -wanderRadius,
            wanderRadius
        );
        wanderTarget = startingPosition +
                       new Vector2(randomX, randomY);
    }

    private void OnDestroy()
    {
        if (ownButton != null)
        {
            ownButton.onClick.RemoveListener(
                RetreatToStartingPosition
            );
        }
    }
}