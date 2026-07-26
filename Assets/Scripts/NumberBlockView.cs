using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class NumberBlockView : MonoBehaviour
{
    [Header("Data")]
    public NumberBlockData BlockData;

    [Header("Renderers")]
    public SpriteRenderer renderer1;
    public SpriteRenderer renderer2;

    [Header("Audio")]
    public AudioSource audioSourceClick;
    public AudioSource audioSourceTone;
    public AudioResource clickSound;
    public AudioResource incorrectSound;

    [Header("Effects")]
    public ParticleSystem starBurst;

    const float pitchVariation = 0.1f;

    [HideInInspector]
    public bool isPressed = false;
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

    public virtual void ApplyState()
    {
        if (BlockData.IsDoppleganger)
        {
            renderer1.sprite = BlockData.DopplegangerSpritesBottom[Random.Range(0, BlockData.DopplegangerSpritesBottom.Count - 1)];
            renderer2.sprite = BlockData.DopplegangerSpritesTop[Random.Range(0, BlockData.DopplegangerSpritesTop.Count - 1)];
            return;
        }

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

    public virtual void PlayPressedEffects()
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

    public virtual void PlayIncorrectEffects()
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
