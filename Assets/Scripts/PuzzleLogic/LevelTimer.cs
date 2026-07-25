using System.Collections;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private float timeLimit = 10f;

    [SerializeField] private PuzzleLogicController controller;

    [SerializeField] private float loseDelay = 1f;

    public float TimeRemaining { get; private set; }
    private bool ended;

    private void Start()
    {
        TimeRemaining = timeLimit;

        if (controller == null)
            controller = FindAnyObjectByType<PuzzleLogicController>();
    }

    private void Update()
    {
        if (ended)
            return;

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
            StartCoroutine(LevelLose());
    }

    private IEnumerator LevelLose()
    {
        if (ended)
            yield break;

        ended = true;

        yield return new WaitForSeconds(loseDelay);
        MainMenuManager.OpenLevelSelectOnNextLoad();
        GameProgress.LastResult = GameProgress.Result.Failed;
        SceneLoader.Instance.ReturnToLevelSelect();
    }
}
