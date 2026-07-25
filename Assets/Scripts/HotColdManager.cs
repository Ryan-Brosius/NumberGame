using UnityEngine;
using UnityEngine.UI;

public class HotColdManager : MonoBehaviour
{
    [Header("cursor")]
    [SerializeField] private Transform cursor;
    [Header("hot / cold slider")]
    [SerializeField] private Slider hotColdSlider;
    [Header("puzzle controller")]
    [SerializeField] private PuzzleLogicController puzzleController;
    [Header("hot / cold distance")]
    [SerializeField] private float coldDistance = 10f;
    [SerializeField] private float hotDistance = 1f;
    [Header("slider smoothing")]
    [SerializeField] private float sliderSmoothSpeed = 5f;
    private NumberBlockView[] numberBlocks;

    private void Start()
    {
        numberBlocks = FindObjectsByType<NumberBlockView>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        if (hotColdSlider != null)
        {
            hotColdSlider.minValue = 0f;
            hotColdSlider.maxValue = 1f;
            hotColdSlider.value = 0f;
        }
    }

    private void Update()
    {
        if (cursor == null ||
            hotColdSlider == null ||
            puzzleController == null ||
            numberBlocks == null)
        {
            return;
        }
        int expectedValue = puzzleController.NextExpectedStep;
        Transform correctBlockParent = null;
        foreach (NumberBlockView numberBlock in numberBlocks)
        {
            if (numberBlock == null)
                continue;
            if (numberBlock.Value == expectedValue)
            {
                if (numberBlock.transform.parent != null)
                {
                    correctBlockParent = numberBlock.transform.parent;
                }

                break;
            }
        }
        if (correctBlockParent == null)
        {
            hotColdSlider.value = Mathf.Lerp(
                hotColdSlider.value,
                0f,
                Time.deltaTime * sliderSmoothSpeed
            );

            return;
        }
        float distance = Vector3.Distance(
            cursor.position,
            correctBlockParent.position
        );
        float hotValue = Mathf.InverseLerp(
            coldDistance,
            hotDistance,
            distance
        );
        hotColdSlider.value = Mathf.Lerp(
            hotColdSlider.value,
            hotValue,
            Time.deltaTime * sliderSmoothSpeed
        );
    }
}