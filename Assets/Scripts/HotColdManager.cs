using UnityEngine;
using UnityEngine.UI;

public class HotColdManager : MonoBehaviour
{
    [Header("cursor")]
    [SerializeField] private Transform cursor;
    [Header("hot / cold slider")]
    [SerializeField] private SpriteRenderer hotColdSlider;
    [Header("puzzle controller")]
    [SerializeField] private PuzzleLogicController puzzleController;
    [Header("hot / cold distance")]
    [SerializeField] private float coldDistance = 10f;
    [SerializeField] private float hotDistance = 2f;
    [Header("slider smoothing")]
    [SerializeField] private float sliderSmoothSpeed = 5f;
    private NumberBlockView[] numberBlocks;

    private float sliderMinHeight = 0f;
    private float sliderMaxHeight = 5.1f;
    private float sliderValue = 0f;

    private void Start()
    {
        numberBlocks = FindObjectsByType<NumberBlockView>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        sliderValue = 0f;
        if (hotColdSlider != null)
        {
            //hotColdSlider.minValue = 0f;
            //hotColdSlider.maxValue = 1f;
            //hotColdSlider.value = 0f;
            hotColdSlider.size = new Vector2(0.84f, sliderMinHeight);
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


        hotColdSlider.size = new Vector2(0.84f, Mathf.Lerp(
            sliderMinHeight,
            sliderMaxHeight,
            sliderValue)
        );

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
            sliderValue = Mathf.Lerp(sliderValue, 0f, Time.deltaTime * sliderSmoothSpeed);
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

        sliderValue = Mathf.Lerp(sliderValue, hotValue, Time.deltaTime * sliderSmoothSpeed);
        /*hotColdSlider.value = Mathf.Lerp(
            hotColdSlider.value,
            hotValue,
            Time.deltaTime * sliderSmoothSpeed
        );*/
    }
}