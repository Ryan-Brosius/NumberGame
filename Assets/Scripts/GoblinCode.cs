using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class GoblinCode : MonoBehaviour
{
    [Header("movement")]
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float wanderRadius = 15f;
    [Header("cursor")]
    [SerializeField] private GameObject cursorObject;
    [Header("animator")]
    [SerializeField] private Animator animator;
    [Header("number block int")]
    [SerializeField] private float contactTimeRequired = 1f;
    [Header("puzzle")]
    [SerializeField] private PuzzleLogicController puzzleController;
    [Header("retreat speed")]
    [SerializeField] private float retreatSpeed = 200f;
    [Header("return cooldown")]
    [SerializeField] private float returnCooldown = 1f;
    [Header("pain squash & stretch")]
    [SerializeField] private float squashAmount = 0.7f;
    [SerializeField] private float stretchAmount = 1.3f;
    [SerializeField] private float painDuration = 0.12f;
    [SerializeField] private float recoveryDuration = 0.2f;

    [Header("Audio")]
    [SerializeField] private List<AudioResource> laughSounds;
    [SerializeField] private List<AudioResource> bonkSounds;

    private RectTransform rectTransform;
    private Vector2 startingPosition;
    private Vector2 wanderTarget;
    private Vector3 originalScale;
    private Coroutine painCoroutine;
    private NumberBlockView targetBlock;
    private float contactTimer;
    private bool cursorInside;
    private bool returnCooldownActive;
    private enum GoblinState
    {
        Wandering,
        GoingToPressedBlock,
        Retreating
    }

    private GoblinState currentState;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        puzzleController = FindAnyObjectByType<PuzzleLogicController>();
        ChooseNewWanderTarget();
        currentState = GoblinState.Wandering;
        if (animator != null)
        {
            animator.SetBool("ishurt", false);
        }
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            return;
        }
        if (returnCooldownActive)
        {
            return;
        }
        if (cursorInside && Input.GetMouseButtonDown(0))
        {
            RetreatToStartingPosition();
            return;
        }
        switch (currentState)
        {
            case GoblinState.Wandering:
                Wander();
                FindPressedBlock();
                break;
            case GoblinState.GoingToPressedBlock:
                GoToPressedBlock();
                break;
            case GoblinState.Retreating:
                Retreat();
                break;
        }
    }

    private void Wander()
    {
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

    private void FindPressedBlock()
    {
        NumberBlockView[] blocks = FindObjectsByType<NumberBlockView>(
            FindObjectsSortMode.None
        );
        foreach (NumberBlockView block in blocks)
        {
            if (block != null && block.IsPressed)
            {
                targetBlock = block;
                contactTimer = 0f;
                currentState = GoblinState.GoingToPressedBlock;
                return;
            }
        }
    }

    private void GoToPressedBlock()
    {
        if (targetBlock == null)
        {
            targetBlock = null;
            contactTimer = 0f;
            currentState = GoblinState.Wandering;
            return;
        }

        if (!targetBlock.IsPressed)
        {
            targetBlock = null;
            contactTimer = 0f;
            currentState = GoblinState.Wandering;
            return;
        }

        Vector2 targetPosition = targetBlock.transform.position;
        Vector2 currentPosition = rectTransform.position;

        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        rectTransform.position = newPosition;

        float distance = Vector2.Distance(
            newPosition,
            targetPosition
        );

        if (distance < 1f)
        {
            contactTimer += Time.deltaTime;

            if (contactTimer >= contactTimeRequired)
            {
                UnpressTargetBlock();
            }
        }
        else
        {
            contactTimer = 0f;
        }
    }

    private void UnpressTargetBlock()
    {
        if (targetBlock == null)
        {
            return;
        }

        targetBlock.IsPressed = false;

        if (puzzleController != null)
        {
            puzzleController.ResetSequence();
        }

        targetBlock = null;
        contactTimer = 0f;

        ChooseNewWanderTarget();
        currentState = GoblinState.Wandering;

        SoundManager.PlaySound( laughSounds[ Random.Range(0, laughSounds.Count-1) ], volume: 0.6f);
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
            StartCoroutine(ReturnCooldown());
            ChooseNewWanderTarget();
            currentState = GoblinState.Wandering;
        }
    }

    private IEnumerator ReturnCooldown()
    {
        returnCooldownActive = true;
        if (animator != null)
        {
            animator.SetBool("ishurt", true);
        }
        yield return new WaitForSeconds(returnCooldown);
        returnCooldownActive = false;
        if (animator != null)
        {
            animator.SetBool("ishurt", false);
        }
    }
    private void RetreatToStartingPosition()
    {
        if (currentState == GoblinState.Retreating)
        {
            return;
        }
        targetBlock = null;
        contactTimer = 0f;
        if (animator != null)
        {
            animator.SetBool("ishurt", true);
        }
        PlayPainAnimation();
        currentState = GoblinState.Retreating;
    }

    private void PlayPainAnimation()
    {
        if (painCoroutine != null)
        {
            StopCoroutine(painCoroutine);
        }
        painCoroutine = StartCoroutine(PainSquashStretch());
        SoundManager.PlaySound(bonkSounds[Random.Range(0, bonkSounds.Count-1)], volume: 0.6f);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cursorObject == null)
        {
            return;
        }

        if (other.gameObject == cursorObject)
        {
            cursorInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (cursorObject == null)
        {
            return;
        }

        if (other.gameObject == cursorObject)
        {
            cursorInside = false;
        }
    }
}