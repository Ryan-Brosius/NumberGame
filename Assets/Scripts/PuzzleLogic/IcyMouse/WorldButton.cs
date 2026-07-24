using UnityEngine;

public class WorldButton : MonoBehaviour, ICursorClickable
{
    [SerializeField] private PuzzleLogicController controller;
    [SerializeField] private int stepIndex = 1;

    [SerializeField] private NumberBlockView blockView;

    private bool isClicked = false;
    public int StepIndex => stepIndex;

    private void Start()
    {
        controller = FindAnyObjectByType<PuzzleLogicController>();
        blockView = GetComponentInChildren<NumberBlockView>();

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

        bool correct = controller.ReportAction(stepIndex);
        Debug.Log(correct);
        if (correct)
        {
            blockView.IsPressed = true;
            isClicked = true;
        }
    }
}
