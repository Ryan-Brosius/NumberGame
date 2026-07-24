using UnityEngine;

public class WorldButton : MonoBehaviour, ICursorClickable
{
    [SerializeField] private PuzzleLogicController controller;
    [SerializeField] private int stepIndex = 1;

    [SerializeField] private NumberBlockData blockData;

    private bool isClicked = false;
    private SpriteRenderer _spriteRenderer;
    public int StepIndex => stepIndex;

    private void OnValidate()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (blockData != null && _spriteRenderer != null)
            _spriteRenderer.sprite = blockData.Sprite1;
    }

    private void Start()
    {
        controller = FindAnyObjectByType<PuzzleLogicController>();

        controller.OnSequenceReset.AddListener(HandleReset);
    }

    private void HandleReset()
    {
        _spriteRenderer.sprite = blockData.Sprite1;
        isClicked = false;
    }

    public void CursorClick()
    {
        if (isClicked)
            return;

        bool correct = controller.ReportAction(stepIndex);
        if (correct)
        {
            _spriteRenderer.sprite = blockData.Sprite2;
            isClicked = true;
        }
    }
}
