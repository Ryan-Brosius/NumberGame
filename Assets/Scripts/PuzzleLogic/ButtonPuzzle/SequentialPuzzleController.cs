using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Puzzle solved by reporting a strict 1-indexed sequence of steps in order.
/// </summary>
public class SequentialPuzzleController : PuzzleLogicController
{
    [Header("Puzzle Information")]
    [Tooltip("How many actions must be done in order to complete the level.")]
    [SerializeField] private int totalSteps = 10;
    [Tooltip("If true, a wrong action resets progress")]
    [SerializeField] private bool resetOnFail = true;

    public UnityEvent<int> OnStepCompleted;
    public UnityEvent<int, int> OnSequenceFailed;   // Expected, Received

    public int TotalSteps => totalSteps;
    public int NextExpectedStep { get; private set; } = 1;

    public bool ReportAction(int stepIndex)
    {
        if (IsComplete)
            return false;

        if (stepIndex != NextExpectedStep)
        {
            OnSequenceFailed?.Invoke(NextExpectedStep, stepIndex);
            if (resetOnFail)
                ResetSequence();
            return false;
        }

        OnStepCompleted?.Invoke(stepIndex);
        NextExpectedStep++;

        if (NextExpectedStep > totalSteps)
            Complete();

        return true;
    }

    public new void ResetSequence()
    {
        NextExpectedStep = 1;
        base.ResetSequence();
    }
}
