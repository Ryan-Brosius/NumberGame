using UnityEngine;

public class NumberBlockBulletTrigger : MonoBehaviour
{
    [Header("puzzle")]
    [SerializeField] private PuzzleLogicController puzzleController;
    [Header("settings")]
    [SerializeField] private bool DinoLevel = false;
    private NumberBlockView numberBlock;

    private void Awake()
    {
        numberBlock = GetComponentInChildren<NumberBlockView>();

        if (puzzleController == null)
        {
            puzzleController = FindFirstObjectByType<PuzzleLogicController>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TurretBullet>() != null)
        {
            PressNumberBlock();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<TurretBullet>() != null)
        {
            PressNumberBlock();
        }
    }

    private void PressNumberBlock()
    {
        if (numberBlock == null || puzzleController == null)
            return;
        if (numberBlock.IsPressed)
            return;
        int hitValue = numberBlock.Value;
        if (hitValue == puzzleController.NextExpectedStep)
        {
            puzzleController.ReportAction(hitValue);

            numberBlock.IsPressed = true;
            numberBlock.PlayPressedEffects();

            if (DinoLevel)
            {
                Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

                foreach (Collider2D collider in colliders)
                {
                    collider.enabled = false;
                }
            }
        }
        else
        {
            numberBlock.PlayIncorrectEffects();
            puzzleController.ReportAction(hitValue);
            puzzleController.ResetSequence();
        }
    }
}