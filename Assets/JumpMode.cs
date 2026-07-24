using UnityEngine;
using System.Collections;

public class JumpMode : MonoBehaviour
{
    [Header("jump")]
    [SerializeField] private float minJumpInterval = 1f;
    [SerializeField] private float maxJumpInterval = 3f;
    [SerializeField] private float jumpHeight = 250f;
    [SerializeField] private float jumpDuration = 0.8f;
    [Header("starting pos")]
    [SerializeField] private float lowestPosition = -620f;
    private RectTransform rectTransform;
    private Coroutine jumpRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        startPosition.y = lowestPosition;
        rectTransform.anchoredPosition = startPosition;
        jumpRoutine = StartCoroutine(JumpRoutine());
    }

    private void OnDisable()
    {
        if (jumpRoutine != null)
        {
            StopCoroutine(jumpRoutine);
            jumpRoutine = null;
        }
    }

    private IEnumerator JumpRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minJumpInterval, maxJumpInterval);
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(Jump());
        }
    }

    private IEnumerator Jump()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        startPosition.y = lowestPosition;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / jumpDuration);
            float arc = 4f * normalizedTime * (1f - normalizedTime);
            Vector2 currentPosition = startPosition;
            currentPosition.y = lowestPosition + (jumpHeight * arc);
            rectTransform.anchoredPosition = currentPosition;
            yield return null;
        }

        Vector2 finalPosition = rectTransform.anchoredPosition;
        finalPosition.y = lowestPosition;
        rectTransform.anchoredPosition = finalPosition;
    }
}