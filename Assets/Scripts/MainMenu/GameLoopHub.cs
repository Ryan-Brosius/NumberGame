using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

public class GameLoopHub : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private List<LevelSelectView> buttonViews;
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
    [SerializeField] private float rollbackFirstGap = 0.15f;
    [Tooltip("get faster")]
    [SerializeField] private float rollbackSpeedup = 0.75f;
    [SerializeField] private float rollbackMinGap = 0.05f;

    [Header("Shuffle")]
    [SerializeField] private int shuffleSwaps = 14;
    [SerializeField] private float shuffleGap = 0.09f;

    [Header("Events")]
    public UnityEvent OnRunCompleted;

    [Header("Visuals")]
    [SerializeField] private float buttonSpacing = 2.5f;
    [SerializeField] private SpriteRenderer levelNameSR;
    [SerializeField] private SpriteRenderer levelHintSR;

    [Header("Audio")]
    [SerializeField] private AudioResource shuffleSound;

    private bool busy;

    public void Update()
    {
        // Update position of button views
        var yOffset = 0f;
        for (int i = 0; i < buttonViews.Count; i++)
        {
            yOffset = (i == GameProgress.CompletedCount) ? 0.0625f * 3f : 0f;
            buttonViews[i].targetPosition = new Vector3(buttonSpacing * (float)i - (buttonSpacing * GameProgress.CompletedCount), yOffset, 0f);

            //buttonViews[i].targetScale = (i > GameProgress.CompletedCount) ? 0.5f : 1f;

                //buttonViews[i].targetScale = 1.0f - Mathf.Abs(i - GameProgress.CompletedCount) * 0.05f;
        }
    }

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

        UpdateNameAndHint(GameProgress.CompletedCount);
    }

    private void HideNameAndHint()
    {
        levelNameSR.gameObject.SetActive(false);
        //levelHintSR.gameObject.SetActive(false);
    }

    private void ShowNameAndHint()
    {
        levelNameSR.gameObject.SetActive(true);
        //levelHintSR.gameObject.SetActive(false);
    }

    private void UpdateNameAndHint(int buttonIndex)
    {
        ShowNameAndHint();
        if (buttonViews[buttonIndex] != null)
        {
            if (buttonViews[buttonIndex].levelData != null)
            {
                levelNameSR.gameObject.SetActive(true);
                levelHintSR.gameObject.SetActive(true);
                levelNameSR.sprite = buttonViews[buttonIndex].levelData.LevelNameSprite;
                levelHintSR.sprite = buttonViews[buttonIndex].levelData.LevelHintSprite;
            }
        }
    }

    private IEnumerator ActivateEarnedButton()
    {
        HideNameAndHint();
        yield return new WaitForSeconds(activateDelay);

        NumberBlockView earned = buttonViews[GameProgress.CompletedCount];
        earned.IsPressed = true;
        earned.PlayPressedEffects();

        GameProgress.CompletedCount++;

        if (GameProgress.CompletedCount >= buttonViews.Count)
        {
            yield return new WaitForSeconds(0.8f);
            TransitionManager.Open("EndingScene", null);
            OnRunCompleted?.Invoke();
        }
        else
        {
            UpdateNameAndHint(GameProgress.CompletedCount);
        }
    }

    private IEnumerator RollbackAndShuffle()
    {
        HideNameAndHint();
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
        SoundManager.PlaySound(shuffleSound, volume: 0.6f);
        yield return null;

        // shuffle everything
        for (int s = 0; s < shuffleSwaps; s++)
        {
            int a = Random.Range(0, buttonViews.Count);
            int b = Random.Range(0, buttonViews.Count - 1);
            if (b >= a) b++;   // distinct pair

            //NumberBlockData dataA = buttonViews[a].BlockData;
            //buttonViews[a].SetData(buttonViews[b].BlockData, buttonViews[b].BlockData);
            //buttonViews[b].SetData(dataA, dataA);
            //LevelData dataA = buttonViews[a].levelData;
            buttonViews[a].SetLevelData(levelPool[Random.Range(0, levelPool.Count-1)]);
            buttonViews[b].SetLevelData(levelPool[Random.Range(0, levelPool.Count - 1)]);

            //SetLevelData

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
        HideNameAndHint();
        busy = true;
        clickedView.PlayPressedEffects();

        //LevelData level = PickRandomUnusedLevel();
        LevelData level = buttonViews[GameProgress.CompletedCount].levelData;

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
