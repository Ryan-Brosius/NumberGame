using UnityEngine;
using UnityEngine.Audio;

public class RegularCursor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float sensitivity = 1f;

    [Header("References")]
    [SerializeField] private Camera boundsCamera;
    [SerializeField] private PuzzleLogicController controller;

    [Header("Effects")]
    [SerializeField] private ParticleSystem clickParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceClick;
    [SerializeField] private AudioResource clickSound;

    private BoxCollider2D _collider;
    private readonly Collider2D[] _results = new Collider2D[10];
    private ContactFilter2D _filter;

    private void Awake()
    {
        if (boundsCamera == null)
            boundsCamera = Camera.main;

        _collider = GetComponent<BoxCollider2D>();
        _filter.useTriggers = true;
    }

    private void Start()
    {
        if (controller == null)
            controller = FindAnyObjectByType<PuzzleLogicController>();

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

        Vector3 ext = _collider.bounds.extents;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, camPos.x - halfWidth + ext.x, camPos.x + halfWidth - ext.x);
        pos.y = Mathf.Clamp(pos.y, camPos.y - halfHeight + ext.y, camPos.y + halfHeight - ext.y);

        transform.position = pos;
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        int count = _collider.Overlap(_filter, _results);

        clickParticles.Play();
        audioSourceClick.pitch = Random.Range(0.9f, 1.1f);
        audioSourceClick.Play();

        ICursorClickable firstClickable = null;
        for (int i = 0; i < count; i++)
        {
            if (!_results[i].TryGetComponent(out ICursorClickable clickable))
                continue;

            firstClickable ??= clickable;

            if (controller != null &&
                _results[i].TryGetComponent(out WorldButton button) &&
                button.StepIndex == controller.NextExpectedStep)
            {
                button.CursorClick();
                return;
            }
        }

        firstClickable?.CursorClick();
    }
}