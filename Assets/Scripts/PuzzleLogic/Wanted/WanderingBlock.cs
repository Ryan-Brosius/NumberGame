using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WanderingBlock : MonoBehaviour
{
    [Tooltip("Random speed range")]
    [SerializeField] private Vector2 speedRange = new Vector2(1.5f, 3.5f);

    [SerializeField] private Camera boundsCamera;   // defaults to Camera.main

    private Transform visuals;
    private Vector2 velocity;
    private BoxCollider2D box;

    private Transform ghost;
    private SpriteRenderer[] sourceRenderers;
    private SpriteRenderer[] ghostRenderers;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        if (boundsCamera == null)
            boundsCamera = Camera.main;

        if (visuals == null)
        {
            var view = GetComponentInChildren<NumberBlockView>();
            if (view != null)
                visuals = view.transform;
        }
    }

    private void Start()
    {
        if (velocity == Vector2.zero)
            velocity = Random.insideUnitCircle.normalized *
                       Random.Range(speedRange.x, speedRange.y);

        CreateGhost();
    }

    public void SetVelocity(Vector2 newVelocity) => velocity = newVelocity;

    private void CreateGhost()
    {
        if (visuals == null)
        {
            return;
        }

        ghost = Instantiate(visuals, transform);
        ghost.name = $"{name}_Ghost";
        ghost.localScale = visuals.localScale;

        // lol destroy the un-needed stuff here
        foreach (var mb in ghost.GetComponentsInChildren<MonoBehaviour>(true))
            Destroy(mb);
        foreach (var col in ghost.GetComponentsInChildren<Collider2D>(true))
            Destroy(col);
        foreach (var ps in ghost.GetComponentsInChildren<ParticleSystem>(true))
            Destroy(ps.gameObject == ghost.gameObject ? (Object)ps : ps.gameObject);
        foreach (var audio in ghost.GetComponentsInChildren<AudioSource>(true))
            Destroy(audio);

        sourceRenderers = visuals.GetComponentsInChildren<SpriteRenderer>(true);
        ghostRenderers = ghost.GetComponentsInChildren<SpriteRenderer>(true);

        if (TryGetComponent(out ICursorClickable clickable))
        {
            BoxCollider2D ghostCol = ghost.gameObject.AddComponent<BoxCollider2D>();
            ghostCol.size = box.size;
            ghostCol.offset = box.offset;
            ghostCol.isTrigger = box.isTrigger;

            ghost.gameObject.AddComponent<GhostClickForwarder>().Init(clickable);
        }

        ghost.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
        TeleportIfCenterCrossed();
        UpdateGhost();
    }

    private void GetCameraBounds(out float minX, out float maxX, out float minY, out float maxY)
    {
        float halfHeight = boundsCamera.orthographicSize;
        float halfWidth = halfHeight * boundsCamera.aspect;
        Vector3 camPos = boundsCamera.transform.position;

        minX = camPos.x - halfWidth;
        maxX = camPos.x + halfWidth;
        minY = camPos.y - halfHeight;
        maxY = camPos.y + halfHeight;
    }

    private void TeleportIfCenterCrossed()
    {
        GetCameraBounds(out float minX, out float maxX, out float minY, out float maxY);
        float screenW = maxX - minX;
        float screenH = maxY - minY;

        Vector3 pos = transform.position;

        if (pos.x > maxX) pos.x -= screenW;
        else if (pos.x < minX) pos.x += screenW;

        if (pos.y > maxY) pos.y -= screenH;
        else if (pos.y < minY) pos.y += screenH;

        transform.position = pos;
    }

    private void GetLiveBounds(out Vector2 min, out Vector2 max)
    {
        Vector2 center = transform.TransformPoint(box.offset);
        Vector3 scale = transform.lossyScale;
        Vector2 extents = new Vector2(
            box.size.x * 0.5f * Mathf.Abs(scale.x),
            box.size.y * 0.5f * Mathf.Abs(scale.y));

        min = center - extents;
        max = center + extents;
    }

    private void UpdateGhost()
    {
        if (ghost == null)
            return;

        GetCameraBounds(out float minX, out float maxX, out float minY, out float maxY);
        float screenW = maxX - minX;
        float screenH = maxY - minY;

        GetLiveBounds(out Vector2 bMin, out Vector2 bMax);
        float offsetX = 0f;
        float offsetY = 0f;

        if (bMax.x > maxX) offsetX = -screenW;
        else if (bMin.x < minX) offsetX = screenW;

        if (bMax.y > maxY) offsetY = -screenH;
        else if (bMin.y < minY) offsetY = screenH;

        bool overlappingEdge = offsetX != 0f || offsetY != 0f;
        if (ghost.gameObject.activeSelf != overlappingEdge)
            ghost.gameObject.SetActive(overlappingEdge);

        if (!overlappingEdge)
            return;

        ghost.position = visuals.position + new Vector3(offsetX, offsetY, 0f);
        ghost.rotation = visuals.rotation;

        for (int i = 0; i < sourceRenderers.Length && i < ghostRenderers.Length; i++)
        {
            ghostRenderers[i].sprite = sourceRenderers[i].sprite;
            ghostRenderers[i].color = sourceRenderers[i].color;
            ghostRenderers[i].sortingOrder = sourceRenderers[i].sortingOrder;
        }
    }
}

public class GhostClickForwarder : MonoBehaviour, ICursorClickable
{
    private ICursorClickable target;
    public void Init(ICursorClickable target) => this.target = target;
    public void CursorClick() => target?.CursorClick();
}