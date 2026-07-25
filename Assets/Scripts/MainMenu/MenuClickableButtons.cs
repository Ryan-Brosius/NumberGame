using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class MenuClickableButtons : MonoBehaviour, ICursorClickable
{
    public UnityEvent OnClicked;

    public void CursorClick() => OnClicked?.Invoke();
}
