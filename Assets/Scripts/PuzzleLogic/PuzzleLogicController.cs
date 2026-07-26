using System.Collections;
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
    private LevelTimer levelTimer;

    public int TotalSteps => totalSteps;
    public int NextExpectedStep { get; private set; } = 10;
    public bool IsComplete { get; private set; }
    private void Awake()
    {
        levelTimer = FindAnyObjectByType<LevelTimer>();
    }

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
        levelTimer?.RegainTime();
        OnStepCompleted?.Invoke(stepIndex);
        NextExpectedStep--;

        if (NextExpectedStep == 0)
        {
            IsComplete = true;
            OnSequenceCompleted?.Invoke();
            StartCoroutine(LevelComplete());
        }

        return true;
    }

    public void ResetSequence()
    {
        NextExpectedStep = 10;
        IsComplete = false;
        OnSequenceReset?.Invoke();
    }

    public IEnumerator LevelComplete()
    {
        GameProgress.LastResult = GameProgress.Result.Completed;
        CheeringManager.PlayCheerSfx();
        yield return new WaitForSeconds(2f);

        MainMenuManager.OpenLevelSelectOnNextLoad();
        SceneLoader.Instance.ReturnToLevelSelect();
    }
}
