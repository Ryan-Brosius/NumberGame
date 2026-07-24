/// <summary>
/// Puzzle solved by re-evaluating full world state after each change
/// (e.g. "are all slots in sorted order right now?"), rather than tracking
/// an incremental step sequence.
/// </summary>
public class StateCheckPuzzleController : PuzzleLogicController
{
    /// <summary>Call after any change that might affect the solved state.</summary>
    public void Evaluate(bool isSolved)
    {
        if (isSolved)
            Complete();
        else if (IsComplete)
            ResetSequence();
    }
}
