using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CraneBlock : MonoBehaviour
{
    [SerializeField] private NumberBlockView blockView;

    private PuzzleLogicController controller;
    private bool isClicked = false;
    public int Value => blockView != null ? blockView.Value : 0;
    public NumberBlockView BlockView => blockView;

    private void Start()
    {
        controller = FindFirstObjectByType<PuzzleLogicController>();
        blockView = GetComponentInChildren<NumberBlockView>();

        controller.OnSequenceReset.AddListener(HandleReset);
    }

    private void HandleReset()
    {
        blockView.IsPressed = false;
        isClicked = false;
    }

    public void Grab(Transform claw)
    {
        if (isClicked)
            return;

        bool correct = controller.ReportAction(Value);
        Debug.Log(correct);
        if (correct)
        {
            blockView.IsPressed = true;
            blockView.PlayPressedEffects();
            isClicked = true;
        }
        else
        {
            blockView.PlayIncorrectEffects();
        }
    }
}
