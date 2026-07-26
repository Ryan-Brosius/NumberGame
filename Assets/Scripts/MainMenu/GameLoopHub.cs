using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameLoopHub : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private List<NumberBlockView> buttonViews;
    [SerializeField] private List<NumberBlockData> numberDatas;

    [Header("Levels")]
    [SerializeField] private List<LevelData> levelPool;

    [Header("Success Timings")]
    [Tooltip("press button after this seconds")]
    [SerializeField] private float activateDelay = 0.8f;
    [Tooltip("Pause after clicking a button for a level")]
    [SerializeField] private float launchDelay = 0.4f;

    [Header("Fail Timings")]
    [Tooltip("Pause before button roll")]
    [SerializeField] private float rollbackStartDelay = 0.6f;
    [Tooltip("Delay between first button re-pressing")]
    [SerializeField] private float rollbackFirstGap = 0.45f;
    [Tooltip("get faster")]
    [SerializeField] private float rollbackSpeedup = 0.75f;
    [SerializeField] private float rollbackMinGap = 0.05f;

    [Header("Shuffle")]
    [SerializeField] private int shuffleSwaps = 14;
    [SerializeField] private float shuffleGap = 0.09f;

    [Header("Events")]
    public UnityEvent OnRunCompleted;

    private bool busy;

    private void OnEnable()
    {
        StartCoroutine(HandleArrival());
    }

    private IEnumerator HandleArrival()
    {
        busy = true;
        ApplyBaseState();

        switch (GameProgress.LastResult)
        {
            case GameProgress.Result.Completed:
                GameProgress.LastResult = GameProgress.Result.None;
                yield return ActivateEarnedButton();
                break;

            case GameProgress.Result.Failed:
                GameProgress.LastResult = GameProgress.Result.None;
                yield return RollbackAndShuffle();
                break;
        }

        busy = false;
    }

    private void ApplyBaseState()
    {
        for (int i = 0; i < buttonViews.Count; i++)
        {
            buttonViews[i].SetData(numberDatas[i], numberDatas[i]);
            buttonViews[i].IsPressed = !(i >= GameProgress.CompletedCount);
        }
    }

    private IEnumerator ActivateEarnedButton()
    {
        yield return new WaitForSeconds(activateDelay);

        NumberBlockView earned = buttonViews[GameProgress.CompletedCount];
        earned.IsPressed = true;
        earned.PlayPressedEffects();

        GameProgress.CompletedCount++;

        if (GameProgress.CompletedCount >= buttonViews.Count)
        {
            yield return new WaitForSeconds(0.8f);
            OnRunCompleted?.Invoke();
        }
    }

    private IEnumerator RollbackAndShuffle()
    {
        yield return new WaitForSeconds(rollbackStartDelay);

        // re-enable each button that was pressed
        float gap = rollbackFirstGap;
        for (int i = GameProgress.CompletedCount - 1; i >= 0; i--)
        {
            buttonViews[i].IsPressed = false;
            buttonViews[i].PlayIncorrectEffects();

            yield return new WaitForSeconds(gap);
            gap = Mathf.Max(rollbackMinGap, gap * rollbackSpeedup);
        }

        GameProgress.ResetRun();

        yield return new WaitForSeconds(0.3f);

        // shuffle everything
        for (int s = 0; s < shuffleSwaps; s++)
        {
            int a = Random.Range(0, buttonViews.Count);
            int b = Random.Range(0, buttonViews.Count - 1);
            if (b >= a) b++;   // distinct pair

            NumberBlockData dataA = buttonViews[a].BlockData;
            buttonViews[a].SetData(buttonViews[b].BlockData, buttonViews[b].BlockData);
            buttonViews[b].SetData(dataA, dataA);

            buttonViews[a].PlayResetEffects();
            buttonViews[b].PlayResetEffects();

            yield return new WaitForSeconds(shuffleGap);
        }

        yield return new WaitForSeconds(0.3f);

        // reset (TODO: more natrual later?)
        ApplyBaseState();
    }

    public void HandleButtonClicked(int index)
    {
        Debug.Log("try");
        if (busy || index < 0 || index >= buttonViews.Count)
            return;

        if (index == GameProgress.CompletedCount)
            StartCoroutine(LaunchRandomLevel(buttonViews[index]));
        else
            buttonViews[index].PlayIncorrectEffects();
    }

    private IEnumerator LaunchRandomLevel(NumberBlockView clickedView)
    {
        busy = true;
        clickedView.PlayPressedEffects();

        LevelData level = PickRandomUnusedLevel();

        GameProgress.UsedLevels.Add(level.Index);

        yield return new WaitForSeconds(launchDelay);
        TransitionManager.Open(level.SceneString, level.WindowBorderSprite);
        //SceneLoader.Instance.LoadLevel(level.SceneString);
    }

    private LevelData PickRandomUnusedLevel()
    {
        List<LevelData> unused = new List<LevelData>();
        foreach (LevelData level in levelPool)
            if (!GameProgress.UsedLevels.Contains(level.Index))
                unused.Add(level);

        if (unused.Count == 0)
            return null;

        return unused[Random.Range(0, unused.Count)];
    }
}
