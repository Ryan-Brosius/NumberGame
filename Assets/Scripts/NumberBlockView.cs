using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class NumberBlockView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private NumberBlockData BlockData;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer renderer1;
    [SerializeField] private SpriteRenderer renderer2;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceClick;
    [SerializeField] private AudioSource audioSourceTone;
    [SerializeField] private AudioResource clickSound;
    [SerializeField] private AudioResource incorrectSound;

    [Header("Effects")]
    [SerializeField] private ParticleSystem starBurst;

    const float pitchVariation = 0.1f;

    private bool isPressed = false;
    public bool IsPressed
    {
        get { return isPressed; }
        set { SetState(value); }
    }
    public int Value => BlockData != null ? BlockData.Value : 0;

    private void OnValidate()
    {
        ApplyState();
    }

    private void Awake()
    {
        ApplyState();
    }

    private void SetState(bool isPressed)
    {
        this.isPressed = isPressed;
        ApplyState();
    }

    public void SetData(NumberBlockData up, NumberBlockData pressed)
    {
        BlockData = up;
        ApplyState();
    }

    public void SetSortingOrders(int topOrder, int bottomOrder)
    {
        if (renderer2 != null) renderer2.sortingOrder = topOrder;
        if (renderer1 != null) renderer1.sortingOrder = bottomOrder;
    }

    private void ApplyState()
    {
        if (isPressed)
        {
            if (renderer1 != null)
                renderer1.sprite = BlockData.ButtonDownSpriteBottom != null ? BlockData.ButtonDownSpriteBottom : null;

            if (renderer2 != null)
                renderer2.sprite = BlockData.ButtonDownSpriteTop != null ? BlockData.ButtonDownSpriteTop : null;
        }
        else
        {
            if (renderer1 != null)
                renderer1.sprite = BlockData.ButtonUpSpriteBottom != null ? BlockData.ButtonUpSpriteBottom : null;

            if (renderer2 != null)
                renderer2.sprite = BlockData.ButtonUpSpriteTop != null ? BlockData.ButtonUpSpriteTop : null;
        }

        if (audioSourceTone != null)
            audioSourceTone.resource = BlockData != null ? BlockData.ToneSound : null;
    }

    public void PlayPressedEffects()
    {
        audioSourceClick.resource = clickSound;
        audioSourceClick.pitch = Random.Range(1.0f - pitchVariation, 1.0f + pitchVariation);
        audioSourceClick.Play();
        audioSourceTone.Play();

        Sequence pressedSequence = DOTween.Sequence();
        pressedSequence.Append(transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f))
        .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

        starBurst.Play();
    }

    public void PlayIncorrectEffects()
    {
        audioSourceClick.resource = incorrectSound;
        audioSourceClick.pitch = 1f;
        audioSourceClick.Play();

        var shakeSpeed = 0.06f;
        Sequence incorrectSequence = DOTween.Sequence();
        incorrectSequence.Append(transform.DORotate(new Vector3(0f, 0f, 15f), shakeSpeed))
        .Append(transform.DORotate(new Vector3(0f, 0f, -15f), shakeSpeed))
        .Append(transform.DORotate(new Vector3(0f, 0f, 15f), shakeSpeed))
        .Append(transform.DORotate(new Vector3(0f, 0f, -15f), shakeSpeed))
        .Append(transform.DORotate(new Vector3(0f, 0f, 0f), shakeSpeed));
    }

    public void PlayResetEffects()
    {
        Sequence pressedSequence = DOTween.Sequence();
        pressedSequence.Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.25f))
        .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
    }
}
