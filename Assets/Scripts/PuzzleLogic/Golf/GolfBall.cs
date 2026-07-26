using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class GolfBall : MonoBehaviour, ICursorClickable
{
    [Header("Shot")]
    [SerializeField] private float maxDragDistance = 3f;
    [SerializeField] private float maxLaunchSpeed = 16f;
    [SerializeField] private float deadzone = 0.05f;
    [SerializeField] private float stopThreshold = 0.2f;
    [SerializeField] private float dragStopThreshold = 5.0f;

    [Header("Physics")]
    [SerializeField] private float rollingFriction = 1f;
    [SerializeField] private float bounciness = 0.8f;

    [Header("Trajectory Dots")]
    [SerializeField] private SpriteRenderer dotPrefab;
    [SerializeField] private int dotCount = 12;
    [SerializeField] private float dotSpacing = 0.45f;
    [SerializeField] private float dotStartScale = 0.3f;
    [SerializeField] private float dotEndScale = 0.08f;

    public GolfHole CurrentHole { get; private set; }
    public bool IsMoving => body.linearVelocity.magnitude > stopThreshold;

    private Rigidbody2D body;
    private SpriteRenderer SpriteRenderer;
    private SpriteRenderer[] dots;

    private Transform cursor;
    private bool dragging;
    private Vector2 aimDirection;
    private float power01;
    private GolfHole attractingHole;
    private bool inHole;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        var ballCollider = GetComponent<CircleCollider2D>();
        ballCollider.sharedMaterial = new PhysicsMaterial2D("GolfBallBounce")
        {
            bounciness = bounciness,
            friction = 0f
        };

        body.gravityScale = 0f;
        body.linearDamping = rollingFriction;
        body.freezeRotation = false;
    }

    private void Start()
    {
        dots = new SpriteRenderer[dotCount];
        for (int i = 0; i < dotCount; i++)
        {
            dots[i] = Instantiate(dotPrefab);
            dots[i].name = $"AimDot_{i}";
            dots[i].gameObject.SetActive(false);
        }
    }

    public void CursorDragStart()
    {
        if (body.linearVelocity.magnitude > dragStopThreshold && !inHole)
            return;

        cursor = FindFirstObjectByType<RegularCursor>().transform;
        dragging = true;
    }

    private void Update()
    {
        HandleDrag();

        if (inHole && CurrentHole != null && !dragging)
        {
            body.linearVelocity = Vector2.zero;
            body.position = Vector2.Lerp(
                body.position, CurrentHole.holeBallPosition.position, 10f * Time.deltaTime);

            if (Vector2.Distance(transform.position, CurrentHole.holeBallPosition.position) <= 0.1f)
            {
                CurrentHole.BallInHole();
                SpriteRenderer.gameObject.SetActive(false);
            }
        }
    }

    private void HandleDrag()
    {
        if (!dragging || cursor == null)
            return;

        Vector2 pull = (Vector2)transform.position - (Vector2)cursor.position;
        power01 = Mathf.Clamp01(pull.magnitude / maxDragDistance);
        aimDirection = pull.sqrMagnitude > 0.0001f ? pull.normalized : Vector2.zero;

        UpdateDots();

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            cursor = null;
            HideDots();

            if (power01 > deadzone && aimDirection != Vector2.zero)
                Launch();
        }
    }

    private void Launch()
    {
        inHole = false;

        ClearHole(CurrentHole);
        body.linearVelocity = aimDirection * (power01 * maxLaunchSpeed);
    }

    public void CaptureInHole(GolfHole hole)
    {
        CurrentHole = hole;
        inHole = true;
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;

    }

    public void ClearHole(GolfHole hole)
    {
        SpriteRenderer.gameObject.SetActive(true);
        if (CurrentHole != null)
        {
            CurrentHole.BallExitHole();
            CurrentHole = null;
        }
    }

    private void UpdateDots()
    {
        int visible = Mathf.CeilToInt(power01 * dotCount);

        for (int i = 0; i < dotCount; i++)
        {
            bool show = i < visible && power01 > deadzone;
            dots[i].gameObject.SetActive(show);
            if (!show)
                continue;

            float along = (i + 1) * dotSpacing;
            dots[i].transform.position =
                (Vector2)transform.position + aimDirection * along;

            float t = dotCount > 1 ? i / (float)(dotCount - 1) : 0f;
            float scale = Mathf.Lerp(dotStartScale, dotEndScale, t);
            dots[i].transform.localScale = Vector3.one * scale;

            Color c = dots[i].color;
            c.a = Mathf.Lerp(0.25f, 1f, power01);
            dots[i].color = c;
        }
    }

    private void HideDots()
    {
        if (dots == null)
            return;
        foreach (var dot in dots)
            if (dot != null)
                dot.gameObject.SetActive(false);
    }

    public void CursorClick()
    {
        CursorDragStart();
    }
}
