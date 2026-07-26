using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class LevelTimer : MonoBehaviour
{
    public float timeLimit = 25f;

    [SerializeField] private PuzzleLogicController controller;

    [SerializeField] private float loseDelay = 1f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer timeBar;
    [SerializeField] private SpriteRenderer stopwatch;
    [SerializeField] private List<Sprite> stopwatchFrames;
    [SerializeField] private float stopwatchAnimSpeed = 1.5f;
    [SerializeField] private float timeBarWidthMax = 8f;
    [SerializeField] private float timeBarSliceWidth = 14f;

    public float TimeRemaining;
    [Header("Time Regain")]
    [SerializeField] private float regainAmount = 1f;
    [SerializeField] private float regainDecay = 0.1f;

    [SerializeField] private AudioResource failSound;

    private int timesRegained = 0;
    private bool ended;
    private bool pauseTimer = false;
    private float timeBarWidth = 0f;
    private Vector2 timeBarDefaultSize = new Vector2(0f, 0f);
    private Transform stopwatchTransform;
    private float stopwatchFrameTime = 0f;
    private int stopwatchFrameIndex = 0;

    private const float pixel_size = 0.0625f;

    private void Awake()
    {
        timeBarDefaultSize = timeBar.size;
        stopwatchTransform = stopwatch.gameObject.transform;
    }

    private void Start()
    {
        TimeRemaining = timeLimit;
        timeBarWidth = timeBarWidthMax;

        if (controller == null)
            controller = FindAnyObjectByType<PuzzleLogicController>();

        controller.OnSequenceCompleted.AddListener(() => pauseTimer = true);
    }

    private void Update()
    {
        if (ended)
            return;

        TimeRemaining -= pauseTimer ? 0.0f : Time.deltaTime;

        
        var ratio = TimeRemaining / timeLimit;

        var tickSpeed = stopwatchAnimSpeed;
        if (ratio < 0.35f)
        {
            tickSpeed = stopwatchAnimSpeed * 0.4f;
        }
        else if (ratio < 0.15f)
        {
            tickSpeed = stopwatchAnimSpeed * 0.15f;
        }

        timeBar.size = new Vector2(timeBarWidthMax * ratio, timeBarDefaultSize.y);
        stopwatchTransform.localPosition = new Vector3(ratio * (timeBarWidthMax) + (pixel_size * 4f), 0f, 0f);

        stopwatchFrameTime += Time.deltaTime;
        if (stopwatchFrameTime > tickSpeed)
        {
            stopwatchFrameTime -= tickSpeed;
            stopwatchFrameIndex++;
            if (stopwatchFrameIndex > 1){
                stopwatchFrameIndex = 0;
            }
            stopwatch.sprite = stopwatchFrames[stopwatchFrameIndex];
        }

        if (TimeRemaining <= 0f)
            StartCoroutine(LevelLose());
    }

    private IEnumerator LevelLose()
    {
        if (ended)
            yield break;

        ended = true;
        SoundManager.PlaySound(failSound, volume: 1f);

        yield return new WaitForSeconds(loseDelay);
        MainMenuManager.OpenLevelSelectOnNextLoad();
        GameProgress.LastResult = GameProgress.Result.Failed;
        SceneLoader.Instance.ReturnToLevelSelect();
    }

    public void RegainTime()
    {
        if (ended)
            return;
        float amountToRegain = regainAmount - (timesRegained * regainDecay);
        amountToRegain = Mathf.Max(0f, amountToRegain);
        TimeRemaining = Mathf.Min(
            TimeRemaining + amountToRegain,
            timeLimit
        );
        timesRegained++;
    }
}
