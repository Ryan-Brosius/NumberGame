using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GolfHole : MonoBehaviour
{
    [SerializeField] private int value = 1;

    [SerializeField] private PuzzleLogicController controller;
    [SerializeField] public Transform holeBallPosition;
    [SerializeField] private NumberBlockView blockView;

    [SerializeField] private SpriteRenderer poleSprite;
    [SerializeField] private SpriteRenderer flagSprite;
    [SerializeField] private Sprite noBallSprite;
    [SerializeField] private Sprite ballSprite;

    public int Value => value;

    private void Start()
    {
        if (controller == null)
            controller = FindAnyObjectByType<PuzzleLogicController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GolfBall ball = other.GetComponentInParent<GolfBall>();
        if (ball == null)
            return;

        if (ball.CurrentHole == this)
            return;

        ball.CaptureInHole(this);

        bool correct = controller.ReportAction(Value);
        if (correct)
        {
            blockView.IsPressed = true;
            blockView.PlayPressedEffects();
        }
        else
        {
            blockView.PlayIncorrectEffects();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GolfBall ball = other.GetComponentInParent<GolfBall>();
        BallExitHole();
        if (ball != null)
            ball.ClearHole(this);
    }

    public void BallInHole()
    {
        poleSprite.sprite = ballSprite;
        //flagSprite.gameObject.SetActive(false);
    }

    public void BallExitHole()
    {
        poleSprite.sprite = noBallSprite;
    }
}
