using UnityEngine;
using DG.Tweening;

public class MemoryCard : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Open()
    {
        Debug.Log("Open");
        spriteRenderer.sprite = openSprite;

        Sequence openSequence = DOTween.Sequence();
        openSequence.Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.0f), 0.1f))
        .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
    }

    public void Close()
    {
        Debug.Log("Close");
        spriteRenderer.sprite = closedSprite;

        Sequence closeSequence = DOTween.Sequence();
        closeSequence.Append(transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f))
        .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));
    }
}
