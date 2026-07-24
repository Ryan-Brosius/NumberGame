using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class NumberBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private bool draggable = true;

    [SerializeField] private float swapArcHeight = 60f;
    [SerializeField] private float swapDuration = 0.35f;

    private Transform cachedTransform;
    private Camera mainCamera;

    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private SpriteRenderer foregroundRenderer;

    [Header("Pressed Colors (non-draggable only)")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.gray;
    private bool pressed;

    public NumberBlockData item;

    [HideInInspector]public Transform parentAfterDrag;

    private bool dragging;

    private Vector3 targetPos, currentPos;
    private float targetRot, currentRot;
    private Vector3 normalScale;

    private void Awake()
    {
        cachedTransform = transform;
        mainCamera = Camera.main;
        normalScale = transform.localScale;
    }
    public void InitializeItem(NumberBlockData newItem)
    {
        item = newItem;
        backgroundRenderer.sprite = newItem.Sprite1;
        foregroundRenderer.sprite = newItem.Sprite2;
    }

    public void AnimateArcSwapTo(Transform newParent)
    {
        var startWorldPos = cachedTransform.position;

        var flightParent = cachedTransform.root;
        cachedTransform.SetParent(newParent);
        cachedTransform.localPosition = Vector3.zero;
        var endWorldPos = cachedTransform.position;

        cachedTransform.SetParent(flightParent);
        cachedTransform.position = startWorldPos;

        var midWorldPos = Vector3.Lerp(startWorldPos, endWorldPos, 0.5f);
        var arcDir = Vector3.Cross(endWorldPos - startWorldPos, Vector3.forward).normalized;
        midWorldPos += arcDir * swapArcHeight;

        float t = 0f;
        DOTween.To(() => t, x => t = x, 1f, swapDuration)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                var a = Vector3.Lerp(startWorldPos, midWorldPos, t);
                var b = Vector3.Lerp(midWorldPos, endWorldPos, t);
                cachedTransform.position = Vector3.Lerp(a, b, t);
            })
            .OnComplete(() =>
            {
                cachedTransform.SetParent(newParent);
                cachedTransform.localPosition = Vector3.zero;
            });
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        dragging = true;
        parentAfterDrag = cachedTransform.parent;
        cachedTransform.SetParent(cachedTransform.root);

        currentPos = cachedTransform.position;
        targetPos = ScreenToWorldPoint(eventData);
        targetRot = currentRot = 0;
        //anim
        cachedTransform.localScale = normalScale;
        cachedTransform.DOPunchScale(Vector3.one * -0.2f, 0.15f);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //unity won't let me remove this method.
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        dragging = false;
        cachedTransform.SetParent(parentAfterDrag);
        cachedTransform.localPosition = Vector3.zero;
        cachedTransform.rotation = Quaternion.Euler(0,0,currentRot = targetRot = 0);
        //anim
        cachedTransform.localScale = normalScale;
        cachedTransform.DOPunchScale(Vector3.one * 0.2f, 0.15f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (draggable) return;

        pressed = !pressed;
        backgroundRenderer.color = pressed ? pressedColor : normalColor;
        foregroundRenderer.color = pressed ? pressedColor : normalColor;
    }
    private void Update()
    {
        if (!dragging) return;

        targetPos = ScreenToWorldPoint(null);
        currentPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * 10f);
        cachedTransform.position = currentPos;

        targetRot = Mathf.Clamp((targetPos.x - currentPos.x) * 2.5f, -15f, 15f);
        currentRot = Mathf.Lerp(currentRot, targetRot, Time.deltaTime * 5f);
        cachedTransform.rotation = Quaternion.Euler(new Vector3(0, 0, currentRot));
    }

    private Vector3 ScreenToWorldPoint(PointerEventData eventData)
    {
        Vector3 screenPoint = eventData != null ? (Vector3)eventData.position : Input.mousePosition;
        screenPoint.z = mainCamera.WorldToScreenPoint(cachedTransform.position).z;
        return mainCamera.ScreenToWorldPoint(screenPoint);
    }
}
