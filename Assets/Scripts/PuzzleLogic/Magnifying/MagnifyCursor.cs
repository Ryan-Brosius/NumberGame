using UnityEngine;

public class MagnifyCursor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float sensitivity = 1f;

    [Header("Magnification")]
    [SerializeField] private float magnification = 6.25f;
    [SerializeField] private Camera lensCamera;
    [SerializeField] private RectTransform lensImageRect;

    [Header("Clicking")]
    [SerializeField] private LayerMask clickableLayers = ~0;

    [Header("References")]
    [SerializeField] private Camera boundsCamera;   // defaults to Camera.main
    [SerializeField] private PuzzleLogicController controller;

    private readonly Collider2D[] _results = new Collider2D[16];

    private void Awake()
    {
        if (boundsCamera == null)
            boundsCamera = Camera.main;

        if (lensCamera != null && lensImageRect != null)
        {
            float lensWorldDiameter =
                lensImageRect.rect.width * lensImageRect.lossyScale.x;

            lensCamera.orthographicSize =
                lensWorldDiameter / (2f * magnification);
        }
    }

    private void Start()
    {
        if (controller == null)
            controller = FindAnyObjectByType<PuzzleLogicController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        MoveCursor();
        ClampToCamera();
        HandleClick();
    }

    private void MoveCursor()
    {
        Vector2 mouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"));

        transform.position += (Vector3)(mouseDelta * sensitivity);
    }

    private void ClampToCamera()
    {
        float halfHeight = boundsCamera.orthographicSize;
        float halfWidth = halfHeight * boundsCamera.aspect;
        Vector3 camPos = boundsCamera.transform.position;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, camPos.x - halfWidth, camPos.x + halfWidth);
        pos.y = Mathf.Clamp(pos.y, camPos.y - halfHeight, camPos.y + halfHeight);
        transform.position = pos;
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position, 0.0f, _results, clickableLayers);

        ICursorClickable best = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            if (!_results[i].TryGetComponent(out ICursorClickable clickable))
                continue;

            if (controller != null &&
                _results[i].TryGetComponent(out WorldButton button) &&
                button.StepIndex == controller.NextExpectedStep)
            {
                button.CursorClick();
                return;
            }

            float dist = Vector2.Distance(_results[i].bounds.center, transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = clickable;
            }
        }

        best?.CursorClick();
    }
}
