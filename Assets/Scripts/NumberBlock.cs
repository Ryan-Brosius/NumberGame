using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class NumberBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private bool draggable = true;

    [SerializeField] private float swapArcHeight = 60f;
    [SerializeField] private float swapDuration = 0.35f;

    private RectTransform rectTransform;
    private Canvas canvas;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image foregroundImage;

    public NumberBlockData item;

    [HideInInspector]public Transform parentAfterDrag;

    private bool dragging;

    private Vector2 targetPos, currentPos;
    private float targetRot, currentRot;
    private Vector3 normalScale;

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
        canvas = GetComponentInParent<Canvas>();
        normalScale = transform.localScale;
    }
    public void InitializeItem(NumberBlockData newItem)
    {
        item = newItem;
        backgroundImage.sprite = newItem.ButtonUpSpriteBottom;
        foregroundImage.sprite = newItem.ButtonUpSpriteTop;
    }

    public void AnimateArcSwapTo(Transform newParent)
    {
        var startWorldPos = rectTransform.position;

        // Determine the target slot's position without letting its LayoutGroup
        // reposition this item yet -- reparent transiently to read the slot's
        // resting spot, then pull back out to animate freely above all layout groups.
        var flightParent = transform.root;
        transform.SetParent(newParent);
        rectTransform.anchoredPosition = Vector2.zero;
        var endWorldPos = rectTransform.position;

        transform.SetParent(flightParent);
        rectTransform.position = startWorldPos;

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
                rectTransform.position = Vector3.Lerp(a, b, t);
            })
            .OnComplete(() =>
            {
                transform.SetParent(newParent);
                rectTransform.anchoredPosition = Vector2.zero;
            });
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        dragging = true;
        backgroundImage.raycastTarget = false;
        foregroundImage.raycastTarget = false;
        parentAfterDrag = transform.parent;
        var worldPos = rectTransform.position;
        transform.SetParent(transform.root);
        rectTransform.position = worldPos;

        currentPos = rectTransform.anchoredPosition;
        targetPos = ScreenToLocalPoint(eventData);
        targetRot = currentRot = 0;
        //anim
        transform.localScale = normalScale;
        transform.DOPunchScale(Vector3.one * -0.2f, 0.15f);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //unity won't let me remove this method.
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggable) return;

        dragging = false;
        backgroundImage.raycastTarget = true;
        foregroundImage.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
        rectTransform.anchoredPosition = Vector2.zero;
        transform.rotation = Quaternion.Euler(0,0,currentRot = targetRot = 0);
        //anim
        transform.localScale = normalScale;
        transform.DOPunchScale(Vector3.one * 0.2f, 0.15f);
    }
    private void Update()
    {
        if (!dragging) return;

        targetPos = ScreenToLocalPoint(null);
        currentPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * 10f);
        rectTransform.anchoredPosition = currentPos;

        targetRot = Mathf.Clamp((targetPos.x - currentPos.x) * 2.5f, -15f, 15f);
        currentRot = Mathf.Lerp(currentRot, targetRot, Time.deltaTime * 5f);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentRot));
    }

    private Vector2 ScreenToLocalPoint(PointerEventData eventData)
    {
        var screenPoint = eventData != null ? eventData.position : (Vector2)Input.mousePosition;
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, screenPoint, cam, out var localPoint);
        return localPoint;
    }
}
