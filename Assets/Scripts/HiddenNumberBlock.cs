using UnityEngine;

[RequireComponent(typeof(NumberBlockView))]
[RequireComponent(typeof(Collider2D))]
public class HiddenNumberBlock : MonoBehaviour
{
    [Header("puzzle controller")]
    [SerializeField] private PuzzleLogicController puzzleController;
    private NumberBlockView numberBlock;
    public SpriteRenderer spriteBlockRenderer;
    public SpriteRenderer spriteTopRenderer;

    private void Awake()
    {
        numberBlock = GetComponent<NumberBlockView>();
        Transform spriteBlock = transform.Find("SpriteBlock");
        Transform spriteTop = transform.Find("SpriteTop");
        if (spriteBlock != null)
        {
            spriteBlockRenderer = spriteBlock.GetComponent<SpriteRenderer>();
        }
        if (spriteTop != null)
        {
            spriteTopRenderer = spriteTop.GetComponent<SpriteRenderer>();
        }
        HideVisuals();
    }

    private void OnEnable()
    {
        if (puzzleController != null)
        {
            puzzleController.OnStepCompleted.AddListener(OnStepCompleted);
            puzzleController.OnSequenceReset.AddListener(HideVisuals);
        }
    }

    private void OnDisable()
    {
        if (puzzleController != null)
        {
            puzzleController.OnStepCompleted.RemoveListener(OnStepCompleted);
            puzzleController.OnSequenceReset.RemoveListener(HideVisuals);
        }
    }

    private void OnMouseDown()
    {
        if (puzzleController == null || numberBlock == null)
            return;
        int clickedValue = numberBlock.Value;
        puzzleController.ReportAction(clickedValue);
    }

    private void OnStepCompleted(int completedValue)
    {
        if (numberBlock == null)
            return;
        if (numberBlock.Value == completedValue)
        {
            ShowVisuals();
        }
    }

    private void HideVisuals()
    {
        if (spriteBlockRenderer != null)
            spriteBlockRenderer.enabled = false;

        if (spriteTopRenderer != null)
            spriteTopRenderer.enabled = false;
    }

    private void ShowVisuals()
    {
        if (spriteBlockRenderer != null)
            spriteBlockRenderer.enabled = true;

        if (spriteTopRenderer != null)
            spriteTopRenderer.enabled = true;
    }
}