using UnityEngine;
using UnityEngine.Events;

public class PuzzleLogicController : MonoBehaviour
{
    [Header("Puzzle Information")]
    [Tooltip("How many actions must be done in order to complete the level.")]
    [SerializeField] private int totalSteps = 10;
    [Tooltip("If true, a wrong action resets progress")]
    [SerializeField] private bool resetOnFail = true;

    [Header("Events")]
    public UnityEvent<int> OnStepCompleted;
    public UnityEvent OnSequenceCompleted;
    public UnityEvent<int, int> OnSequenceFailed;   // Expected, Received
    public UnityEvent OnSequenceReset;

    public int TotalSteps => totalSteps;
    public int NextExpectedStep { get; private set; } = 1;
    public bool IsComplete { get; private set; }

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
        {
            IsComplete = true;
            OnSequenceCompleted?.Invoke();
        }

        return true;
    }

    public void ResetSequence()
    {
        NextExpectedStep = 1;
        IsComplete = false;
        OnSequenceReset?.Invoke();
    }
}
