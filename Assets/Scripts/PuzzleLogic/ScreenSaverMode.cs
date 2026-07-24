using UnityEngine;

public class ScreenSaverMode : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 200f;

    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;

    private Vector2 direction;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        direction = Random.insideUnitCircle.normalized;
        if (direction.magnitude < 0.1f)
        {
            direction = Vector2.right;
        }
        moveSpeed = Random.Range(100f, 577f);
    }

    void Update()
    {
        rectTransform.anchoredPosition += direction * moveSpeed * Time.deltaTime;
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
            rectTransform.anchoredPosition = new Vector2(
                leftBound,
                rectTransform.anchoredPosition.y
            );

            direction.x = Mathf.Abs(direction.x);
        }
        else if (rectTransform.anchoredPosition.x >= rightBound)
        {
            rectTransform.anchoredPosition = new Vector2(
                rightBound,
                rectTransform.anchoredPosition.y
            );

            direction.x = -Mathf.Abs(direction.x);
        }
        if (rectTransform.anchoredPosition.y <= bottomBound)
        {
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                bottomBound
            );

            direction.y = Mathf.Abs(direction.y);
        }
        else if (rectTransform.anchoredPosition.y >= topBound)
        {
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x,
                topBound
            );

            direction.y = -Mathf.Abs(direction.y);
        }
    }
}