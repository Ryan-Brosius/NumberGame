using System.Collections;
using UnityEngine;

public class MemoryCardController : MonoBehaviour
{
    [Header("puzzle")]
    [SerializeField] private PuzzleLogicController puzzleController;
    [Header("corresponding objects list")]
    [SerializeField] private NumberBlockView[] numberBlocks;
    [SerializeField] private GameObject[] memoryCards;
    [Header("cursor")]
    [SerializeField] private Transform cursorObject;
    [Header("wrong snswer")]
    [SerializeField] private float disableTime = 1f;
    private bool isTemporarilyDisabled = false;
    private Collider2D cursorCollider;

    private void Awake()
    {
        if (cursorObject != null)
        {
            cursorCollider = cursorObject.GetComponent<Collider2D>();
        }
    }

    private void OnEnable()
    {
        if (puzzleController == null)
            return;
        puzzleController.OnStepCompleted.AddListener(OnCorrectCard);
        puzzleController.OnSequenceFailed.AddListener(OnWrongCard);
        puzzleController.OnSequenceReset.AddListener(OnPuzzleReset);
    }

    private void OnDisable()
    {
        if (puzzleController == null)
            return;
        puzzleController.OnStepCompleted.RemoveListener(OnCorrectCard);
        puzzleController.OnSequenceFailed.RemoveListener(OnWrongCard);
        puzzleController.OnSequenceReset.RemoveListener(OnPuzzleReset);
    }

    private void OnCorrectCard(int stepIndex)
    {
        if (isTemporarilyDisabled)
            return;
        for (int i = 0; i < numberBlocks.Length; i++)
        {
            if (numberBlocks[i] == null)
                continue;

            if (numberBlocks[i].Value == stepIndex)
            {
                if (i < memoryCards.Length && memoryCards[i] != null)
                {
                    memoryCards[i].SetActive(false);
                }

                return;
            }
        }
    }

    private void OnWrongCard(int expected, int received)
    {
        if (isTemporarilyDisabled)
            return;

        StartCoroutine(WrongAnswerRoutine());
    }

    private IEnumerator WrongAnswerRoutine()
    {
        isTemporarilyDisabled = true;
        SetCursorCollision(false);
        SetAllCardsActive(false);
        yield return new WaitForSeconds(disableTime);
        ResetAllCards();
        SetCursorCollision(true);
        isTemporarilyDisabled = false;
    }

    private void OnPuzzleReset()
    {
        if (isTemporarilyDisabled)
            return;
        ResetAllCards();
    }

    private void ResetAllCards()
    {
        for (int i = 0; i < memoryCards.Length; i++)
        {
            if (memoryCards[i] == null)
                continue;
            memoryCards[i].SetActive(true);
            if (i < numberBlocks.Length && numberBlocks[i] != null)
            {
                numberBlocks[i].IsPressed = false;
            }
        }
    }

    private void SetAllCardsActive(bool active)
    {
        foreach (GameObject card in memoryCards)
        {
            if (card != null)
            {
                card.SetActive(active);
            }
        }
    }

    private void SetCursorCollision(bool active)
    {
        if (cursorCollider != null)
        {
            cursorCollider.enabled = active;
        }
    }
}