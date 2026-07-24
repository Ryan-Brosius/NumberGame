using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InkSplat : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 3f;

    [Header("Scale")]
    [SerializeField] private float targetScale = 1.25f;
    [SerializeField] private float scaleUpDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 15f;

    private RectTransform rectTransform;
    private Image image;
    private Vector3 originalScale;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        originalScale = rectTransform.localScale;

        StartCoroutine(InkSplatRoutine());
    }

    private IEnumerator InkSplatRoutine()
    {
        Vector3 targetScaleVector = originalScale * targetScale;

        Color originalColor = image.color;

        float timer = 0f;

        while (timer < lifetime)
        {
            timer += Time.deltaTime;
            rectTransform.Rotate(
                0f,
                0f,
                rotationSpeed * Time.deltaTime
            );
            if (timer < scaleUpDuration)
            {
                float t = Mathf.SmoothStep(
                    0f,
                    1f,
                    timer / scaleUpDuration
                );

                rectTransform.localScale = Vector3.Lerp(
                    originalScale,
                    targetScaleVector,
                    t
                );
            }
            else if (timer > lifetime - fadeOutDuration)
            {
                float fadeTimer = timer - (lifetime - fadeOutDuration);
                float t = Mathf.SmoothStep(
                    0f,
                    1f,
                    fadeTimer / fadeOutDuration
                );
                rectTransform.localScale = Vector3.Lerp(
                    targetScaleVector,
                    Vector3.zero,
                    t
                );
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(
                    originalColor.a,
                    0f,
                    t
                );

                image.color = newColor;
            }
            else
            {
                rectTransform.localScale = targetScaleVector;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
