using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shared completion/reset contract for puzzles. Subclasses decide what
/// counts as progress, failure, or a solved state, and call the protected
/// Complete/Reset methods accordingly.
/// </summary>
public abstract class PuzzleLogicController : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnSequenceCompleted = new();
    public UnityEvent OnSequenceReset = new();

    public bool IsComplete { get; private set; }

    protected void Complete()
    {
        if (IsComplete) return;
        IsComplete = true;
        OnSequenceCompleted?.Invoke();
    }

    public void ResetSequence()
    {
        IsComplete = false;
        OnSequenceReset?.Invoke();
    }
}
