using UnityEngine;

public class IcyMouse : MonoBehaviour
{
    [Header("Icy Parameters")]
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float maxInputSpeed = 25f;
    [SerializeField] private float maxVelocity = 18f;
    [SerializeField] private float friction = 1.2f;

    [Header("References")]
    [SerializeField] private Camera boundsCamera;

    private Vector2 velocity;
    public Vector2 Velocity => velocity;
    private Collider2D _collider;
    private readonly Collider2D[] _results = new Collider2D[10];
    private ContactFilter2D _filter;

    private void Awake()
    {
        if (boundsCamera == null)
            boundsCamera = Camera.main;

        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MoveCursor();
        ClampToCamera();
        HandleClick();
    }

    private void MoveCursor()
    {
        // Raw mouse delta this frame.
        Vector2 mouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"));

        // Mouse delta is already per-frame (total distance moved), so we add it
        // directly — no deltaTime needed. The clamp caps how hard one frame's
        // flick can push, so the player can't just yank the mouse to go fast.
        Vector2 inputPush = Vector2.ClampMagnitude(mouseDelta * acceleration * 0.01f, maxInputSpeed * Time.deltaTime);

        velocity += inputPush;
        velocity = Vector2.ClampMagnitude(velocity, maxVelocity);

        // Ice: velocity decays slowly instead of stopping when the mouse stops.
        velocity = Vector2.Lerp(velocity, Vector2.zero, friction * Time.deltaTime);

        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void ClampToCamera()
    {
        float halfHeight = boundsCamera.orthographicSize;
        float halfWidth = halfHeight * boundsCamera.aspect;
        Vector3 camPos = boundsCamera.transform.position;

        Vector3 pos = transform.position;
        float clampedX = Mathf.Clamp(pos.x, camPos.x - halfWidth, camPos.x + halfWidth);
        float clampedY = Mathf.Clamp(pos.y, camPos.y - halfHeight, camPos.y + halfHeight);

        // Kill velocity on the axis we slammed into, so the cursor doesn't "stick" to walls.
        if (!Mathf.Approximately(clampedX, pos.x)) velocity.x = 0f;
        if (!Mathf.Approximately(clampedY, pos.y)) velocity.y = 0f;

        transform.position = new Vector3(clampedX, clampedY, pos.z);
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        int count = _collider.Overlap(_filter, _results);

        for (int i = 0; i < count; i++)
        {
            if (_results[i].TryGetComponent(out ICursorClickable clickable))
            {
                clickable.CursorClick();
                break;
            }
        }
    }
}

public interface ICursorClickable
{
    void CursorClick();
}