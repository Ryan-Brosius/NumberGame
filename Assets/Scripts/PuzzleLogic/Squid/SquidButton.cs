using UnityEngine;

public class SquidButton : MonoBehaviour, ICursorClickable
{
    [SerializeField] private PuzzleLogicController controller;
    [SerializeField] private SquidNumberBlockView blockView;

    private bool isClicked = false;
    public int StepIndex => blockView.Value;

    private void Awake()
    {
        if (blockView == null)
            blockView = GetComponentInChildren<SquidNumberBlockView>();
    }

    private void Start()
    {
        if (controller == null)
            controller = FindAnyObjectByType<PuzzleLogicController>();

        controller.OnSequenceReset.AddListener(HandleReset);
        blockView.IsPressed = false;
    }

    private void HandleReset()
    {
        blockView.IsPressed = false;
        isClicked = false;
    }

    public void CursorClick()
    {
        if (isClicked)
            return;

        bool correct = controller.ReportAction(StepIndex);

        if (correct)
        {
            isClicked = true;
            blockView.PlayPressedEffects();
            blockView.IsPressed = true;
            
            //gameObject.SetActive(false);
        }
        else
        {
            blockView.PlayIncorrectEffects();
        }
    }
}
