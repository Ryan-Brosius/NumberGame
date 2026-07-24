using UnityEngine;

public class FakeButton : MonoBehaviour, ICursorClickable
{
    public const int FakeStepIndex = -1;

    private PuzzleLogicController controller;
    private NumberBlockView blockView;

    private void Awake()
    {
        blockView = GetComponentInChildren<NumberBlockView>();
    }

    private void Start()
    {
        controller = FindAnyObjectByType<PuzzleLogicController>();
    }

    public void SetBlockData(NumberBlockData data)
    {
        blockView.SetData(data, data);
    }

    public void CursorClick()
    {
        if (controller == null || controller.IsComplete)
            return;

        controller.ReportAction(FakeStepIndex);
        blockView.PlayIncorrectEffects();
    }
}
