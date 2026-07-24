using UnityEngine;

public class NumberBlockView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private NumberBlockData upData;
    [SerializeField] private NumberBlockData pressedData;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer renderer1;
    [SerializeField] private SpriteRenderer renderer2;

    public bool IsPressed = false;
    public int Value => upData != null ? upData.Value : 0;

    private void OnValidate()
    {
        ApplyState();
    }

    private void Awake()
    {
        ApplyState();
    }

    public void SetPressed(bool pressed)
    {
        SetState(IsPressed);
    }

    public void SetState(bool isPressed)
    {
        IsPressed = isPressed;
        ApplyState();
    }

    private void ApplyState()
    {
        NumberBlockData data = IsPressed ? pressedData : upData;

        if (renderer1 != null)
            renderer1.sprite = data != null ? data.Sprite1 : null;

        if (renderer2 != null)
            renderer2.sprite = data != null ? data.Sprite2 : null;
    }
}
