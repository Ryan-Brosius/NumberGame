using UnityEngine;

public class MathButton : MonoBehaviour, ICursorClickable
{
    [SerializeField] private Color unpressedColor;
    [SerializeField] private Color pressedColor;

    [SerializeField] private NumberBlockView blockView;
    [SerializeField] private int stepIndex => blockView.Value;
    [SerializeField] private PuzzleLogicController controller;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isClicked = false;

    private void Start()
    {
        controller = FindAnyObjectByType<PuzzleLogicController>();
        blockView = GetComponentInChildren<NumberBlockView>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        controller.OnSequenceReset.AddListener(HandleReset);
        blockView.IsPressed = false;
    }

    private void HandleReset()
    {
        spriteRenderer.color = unpressedColor;
        isClicked = false;
    }

    public void CursorClick()
    {
        if (isClicked)
            return;

        bool correct = controller.ReportAction(stepIndex);
        if (correct)
        {
            spriteRenderer.color = pressedColor;
            blockView.PlayPressedEffects();
            isClicked = true;
        }
        else
        {
            spriteRenderer.color = unpressedColor;
            blockView.PlayIncorrectEffects();
        }
    }
}
