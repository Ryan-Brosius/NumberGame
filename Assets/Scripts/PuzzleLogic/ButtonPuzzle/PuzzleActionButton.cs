using UnityEngine;

public class PuzzleActionButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SequentialPuzzleController controller;

    [Tooltip("Which step in the sequence this button represents (1 indexed)")]
    [SerializeField] private int stepIndex = 1;

    private void Start()
    {
        controller = FindAnyObjectByType<SequentialPuzzleController>();

        controller.OnSequenceReset.AddListener(HandleReset);
    }

    public void Press()
    {
        bool correct = controller.ReportAction(stepIndex);
        Debug.Log(correct);
        if (correct)
            gameObject.SetActive(false);
    }

    private void HandleReset()
    {
        gameObject.SetActive(true);
    }
}
