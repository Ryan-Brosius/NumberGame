using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(BoxCollider2D))]
public class RegularCursor : MonoBehaviour
{
    [Header("References (auto-resolved, override if needed)")]
    [Tooltip("Pixel-perfect camera rendering into the RenderTexture. Defaults to Camera.main.")]
    [SerializeField] private Camera renderCamera;
    [Tooltip("Camera viewing the quad. Defaults to the only camera without a targetTexture.")]
    [SerializeField] private Camera displayCamera;
    [Tooltip("Quad showing the RenderTexture. Defaults to the child of displayCamera using it.")]
    [SerializeField] private Transform displayQuad;
    [SerializeField] private PuzzleLogicController controller;

    [Header("Raycast")]
    [SerializeField] private LayerMask clickMask = ~0;
    [SerializeField] private float rayDistance = 100f;

    [Header("Pixel Snapping")]
    [SerializeField] private bool snapToPixelGrid = true;
    [SerializeField] private float pixelsPerUnit = 16f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem clickParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceClick;
    [SerializeField] private AudioResource clickSound;

    private BoxCollider2D _collider;
    private float _z;

    // Debug state
    private Vector2 _uv;
    private bool _uvValid;
    private Ray _ray;
    private RaycastHit2D[] _lastHits = new RaycastHit2D[0];
    private Vector3 _lastClickPoint;
    private bool _hasClicked;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _z = transform.position.z;
        ResolveReferences();
    }

    private void Start()
    {
        if (controller == null)
            controller = FindAnyObjectByType<PuzzleLogicController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    [ContextMenu("Resolve References")]
    private void ResolveReferences()
    {
        if (renderCamera == null)
            renderCamera = Camera.main;

        if (renderCamera == null)
        {
            Debug.LogError("[RegularCursor] No MainCamera-tagged camera found.", this);
            return;
        }

        RenderTexture target = renderCamera.targetTexture;
        if (target == null)
            Debug.LogWarning("[RegularCursor] renderCamera has no targetTexture — check the tag is on the right camera.", this);

        if (displayCamera == null)
        {
            // The display camera is the one drawing to the actual screen, so it has no targetTexture.
            // If several qualify, the highest depth is the one the player sees.
            foreach (Camera cam in FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (cam == renderCamera || cam.targetTexture != null)
                    continue;
                if (displayCamera == null || cam.depth > displayCamera.depth)
                    displayCamera = cam;
            }
        }

        if (displayCamera == null)
        {
            Debug.LogError("[RegularCursor] No display camera found.", this);
            return;
        }

        if (displayQuad == null)
        {
            Transform fallback = null;
            foreach (Renderer r in displayCamera.GetComponentsInChildren<Renderer>(true))
            {
                fallback ??= r.transform;
                if (target != null && r.sharedMaterial != null && r.sharedMaterial.mainTexture == target)
                {
                    displayQuad = r.transform;
                    break;
                }
            }

            displayQuad ??= fallback;

            if (displayQuad == null)
                Debug.LogError("[RegularCursor] No renderer found under the display camera.", this);
        }
    }

    private void Update()
    {
        if (renderCamera == null || displayCamera == null || displayQuad == null)
            return;

        MoveCursor();
        HandleClick();
    }

    private bool TryGetSurfaceUV(out Vector2 uv)
    {
        uv = Vector2.zero;

        Ray screenRay = displayCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(displayQuad.forward, displayQuad.position);

        if (!plane.Raycast(screenRay, out float dist))
        {
            plane = new Plane(-displayQuad.forward, displayQuad.position);
            if (!plane.Raycast(screenRay, out dist))
                return false;
        }

        Vector3 local = displayQuad.InverseTransformPoint(screenRay.GetPoint(dist));
        uv = new Vector2(local.x + 0.5f, local.y + 0.5f);
        return true;
    }

    private void MoveCursor()
    {
        _uvValid = TryGetSurfaceUV(out _uv);
        if (!_uvValid)
            return;

        _uv.x = Mathf.Clamp01(_uv.x);
        _uv.y = Mathf.Clamp01(_uv.y);

        _ray = renderCamera.ViewportPointToRay(_uv);

        Vector3 world = renderCamera.ViewportToWorldPoint(new Vector3(_uv.x, _uv.y, 1f));
        world.z = _z;
        world = InsetFromEdges(world);

        if (snapToPixelGrid && pixelsPerUnit > 0f)
        {
            world.x = Mathf.Round(world.x * pixelsPerUnit) / pixelsPerUnit;
            world.y = Mathf.Round(world.y * pixelsPerUnit) / pixelsPerUnit;
        }

        transform.position = world;
    }

    private Vector3 InsetFromEdges(Vector3 pos)
    {
        float halfHeight = renderCamera.orthographicSize;
        float halfWidth = halfHeight * renderCamera.aspect;
        Vector3 camPos = renderCamera.transform.position;
        Vector3 ext = _collider.bounds.extents;

        pos.x = Mathf.Clamp(pos.x, camPos.x - halfWidth + ext.x, camPos.x + halfWidth - ext.x);
        pos.y = Mathf.Clamp(pos.y, camPos.y - halfHeight + ext.y, camPos.y + halfHeight - ext.y);
        return pos;
    }

    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0) || !_uvValid)
            return;

        _hasClicked = true;
        _lastClickPoint = transform.position;
        _lastHits = Physics2D.GetRayIntersectionAll(_ray, rayDistance, clickMask);

        clickParticles.Play();
        audioSourceClick.pitch = Random.Range(0.9f, 1.1f);
        audioSourceClick.Play();

        ICursorClickable firstClickable = null;

        foreach (RaycastHit2D hit in _lastHits)
        {
            if (hit.transform == transform)
                continue;

            if (!hit.collider.TryGetComponent(out ICursorClickable clickable))
                continue;

            firstClickable ??= clickable;

            if (controller != null &&
                hit.collider.TryGetComponent(out WorldButton button) &&
                button.StepIndex == controller.NextExpectedStep)
            {
                button.CursorClick();
                return;
            }
        }

        firstClickable?.CursorClick();
    }
}