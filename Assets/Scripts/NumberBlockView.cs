using UnityEngine;
using UnityEngine.Audio;

public class NumberBlockView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private NumberBlockData upData;
    [SerializeField] private NumberBlockData pressedData;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer renderer1;
    [SerializeField] private SpriteRenderer renderer2;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSourceClick;
    [SerializeField] private AudioSource audioSourceTone;

    const float pitchVariation = 0.1f;

    private bool isPressed = false;
    public bool IsPressed
    {
        get { return isPressed; }
        set { SetState(value); }
    }
    public int Value => upData != null ? upData.Value : 0;

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

    private void ApplyState()
    {
        NumberBlockData data = isPressed ? pressedData : upData;

        if (renderer1 != null)
            renderer1.sprite = data != null ? data.Sprite1 : null;

        if (renderer2 != null)
            renderer2.sprite = data != null ? data.Sprite2 : null;

        if (audioSourceTone != null)
            audioSourceTone.resource = data != null ? data.ToneSound : null;
    }

    public void PlaySfx()
    {
        // randomize pitch
        audioSourceClick.pitch = Random.Range(1.0f - pitchVariation, 1.0f + pitchVariation);
        audioSourceClick.Play();
        audioSourceTone.Play();
    }
}
