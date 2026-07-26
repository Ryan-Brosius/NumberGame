using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

[RequireComponent(typeof(BoxCollider2D))]
public class MenuClickableButtons : MonoBehaviour, ICursorClickable
{
    public UnityEvent OnClicked;
    [SerializeField] private AudioResource startSound;

    public void CursorClick()
    {
        SoundManager.PlaySound(startSound, volume: 1f);
        OnClicked?.Invoke();
    }
}
